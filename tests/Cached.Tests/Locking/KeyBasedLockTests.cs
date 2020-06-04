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

    public class KeyBasedLockTests
    {
        public class LockAsyncMethod
        {
            internal static Dictionary<object, Reservable<SemaphoreSlim>> GetReflectedLockList(KeyBasedLock lck) 
                => (Dictionary<object, Reservable<SemaphoreSlim>>)typeof(KeyBasedLock)
                .GetField("Reserved", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(lck);

            public class Throws
            {
                [Fact]
                public async Task When_Key_Is_Null()
                {
                    await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                        await new KeyBasedLock().LockAsync(null));
                }

                [Fact]
                public async Task If_Internal_Lock_Is_Missing_During_Disposal()
                {
                    // Arrange
                    var keyBasedLock = new KeyBasedLock();                    
                    
                    // Act, Assert
                    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    {
                        using (await keyBasedLock.LockAsync("test_lock_missing"))
                        {
                            GetReflectedLockList(keyBasedLock).Remove("test_lock_missing");
                        }
                    });
                }
            }

            [Fact]
            public async Task Creates_Separate_Locks_Based_On_Key_And_Dispose_Them_Correctly()
            {
                // Arrange
                var keyBasedLock = new KeyBasedLock();
                const string testKey = "test_lock_by_key";
                bool KeyExists(string key) => GetReflectedLockList(keyBasedLock)
                    .Any(l => l.Key.Equals(key));
                SemaphoreSlim GetByKey(string key) => GetReflectedLockList(keyBasedLock)
                    .First(l => l.Key.Equals(key)).Value.Value;
                SemaphoreSlim lck1;
                SemaphoreSlim lck2;

                // Act, Assert
                using (await keyBasedLock.LockAsync(testKey))
                {
                    Assert.True(KeyExists(testKey));
                    lck1 = GetByKey(testKey);
                    using (await keyBasedLock.LockAsync(testKey + "_2"))
                    {
                        Assert.True(KeyExists(testKey));
                        Assert.True(KeyExists(testKey + "_2"));
                        lck2 = GetByKey(testKey + "_2");
                    }
                    Assert.True(KeyExists(testKey));
                    Assert.False(KeyExists(testKey + "_2"));
                }
                Assert.False(KeyExists(testKey));
                Assert.True(lck1.CurrentCount == 1);
                Assert.True(lck2.CurrentCount == 1);
            }

            [Fact]
            public async Task Reuse_Same_Lock_For_All_Queued_Tasks_With_Same_Key()
            {
                // Arrange
                var keyBasedLock = new KeyBasedLock();
                var tasks = Enumerable.Repeat(LockTestTask(keyBasedLock), 10);

                // Act
                var result = await Task.WhenAll(tasks);

                // Assert
                Assert.Equal(10, result.Length);
                Assert.True(result.All(l => l != null));
                Assert.True(result.All(l => l.Equals(result[0])));
                Assert.DoesNotContain(GetReflectedLockList(keyBasedLock), l => l.Key.Equals("test_lock_stack"));
            }

            private static async Task<SemaphoreSlim> LockTestTask(KeyBasedLock keyBasedLock)
            {
                SemaphoreSlim lck;
                using (await keyBasedLock.LockAsync("test_lock_stack"))
                {
                    lck = GetReflectedLockList(keyBasedLock)
                        .FirstOrDefault(l => l.Key.Equals("test_lock_stack"))
                        .Value
                        .Value;

                    await Task.Delay(10);
                }

                return lck;
            }
        }
    }
}