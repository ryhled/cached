namespace Cached.Tests.Locking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Cached.Locking;
    using Xunit;

    public sealed class SemaphoreSlimLockTests
    {
        public sealed class LockAsyncMethod
        {
            public LockAsyncMethod()
            {
                _cacherLock = new SemaphoreSlimLock();

                // Not pretty but keeping this field protected from external tampering is critical.
                // At the same time it needs to be checked that internal state functions correctly.
                // Suggestions for alternative design?
                _activeLocks = (Dictionary<object, Reservable<SemaphoreSlim>>) typeof(SemaphoreSlimLock)
                    .GetField("Reserved", BindingFlags.Static | BindingFlags.NonPublic)
                    ?.GetValue(this);
            }

            private readonly SemaphoreSlimLock _cacherLock;
            private readonly Dictionary<object, Reservable<SemaphoreSlim>> _activeLocks;

            public class WillThrowException
            {
                [Fact]
                public async Task When_key_is_null()
                {
                    await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                        await new SemaphoreSlimLock().LockAsync(null));
                }
            }

            private async Task TestAction(SemaphoreSlimLock lck, string key, int delay, ICollection<int> callback)
            {
                int count;
                using (await lck.LockAsync(key))
                {
                    count = _activeLocks.First(active => active.Key.Equals(key)).Value.Reservations;
                    await Task.Delay(delay);
                }

                callback.Add(count);
            }

            [Fact]
            public async Task Creates_unique_locks_for_each_key_and_dispose_them_after_use()
            {
                // Arrange
                var beforeUsingCount = _activeLocks.Count;
                var testKey = "locktest_unique_1";
                Func<string, bool> keyExists = key => _activeLocks.Any(l => l.Key.Equals(key));

                // Act, Assert
                using (await _cacherLock.LockAsync(testKey))
                {
                    Assert.True(keyExists(testKey));
                    using (await _cacherLock.LockAsync(testKey + "_2"))
                    {
                        Assert.True(keyExists(testKey));
                        Assert.True(keyExists(testKey + "_2"));
                        using (await _cacherLock.LockAsync(testKey + "_3"))
                        {
                            Assert.True(keyExists(testKey));
                            Assert.True(keyExists(testKey + "_2"));
                            Assert.True(keyExists(testKey + "_3"));
                        }

                        Assert.True(keyExists(testKey));
                        Assert.True(keyExists(testKey + "_2"));
                        Assert.False(keyExists(testKey + "_3"));
                    }

                    Assert.True(keyExists(testKey));
                    Assert.False(keyExists(testKey + "_2"));
                }

                Assert.False(keyExists(testKey));
            }

            [Fact]
            public async Task Tracks_lock_reservations_and_removes_lock_only_when_it_is_fully_released()
            {
                // Arrange
                var callLog = new List<int>();
                var tasks = new List<Task>();
                for (var i = 1; i <= 5; i++)
                {
                    tasks.Add(TestAction(_cacherLock, "key", 10, callLog));
                }

                var maxretries = 5;
                while (_activeLocks.Count > 0 && maxretries > 0)
                {
                    await Task.Delay(5);
                    ++maxretries;
                }

                // Act
                await Task.WhenAll(tasks);

                // Assert
                Assert.Equal(1, callLog[0]);
                Assert.Equal(4, callLog[1]);
                Assert.Equal(3, callLog[2]);
                Assert.Equal(2, callLog[3]);
                Assert.Equal(1, callLog[4]);
                Assert.Empty(_activeLocks);
            }
        }
    }
}