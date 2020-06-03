namespace Cached.Tests.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Moq;
    using Xunit;

    public sealed class InMemoryCachedTests
    {
        //public sealed class Constructor
        //{
        //    public sealed class WillThrowException
        //    {
        //        [Fact]
        //        public void If_Memory_Cacher_Argument_Is_Null()
        //        {
        //            Assert.Throws<ArgumentNullException>(() =>
        //                new InMemoryCached<object, object>(
        //                    null,
        //                    o => "", 
        //                    (s, o) => Task.FromResult(o)));
        //        }

        //        [Fact]
        //        public void If_Memory_Fetch_Func_Is_Null()
        //        {
        //            Assert.Throws<ArgumentNullException>(() =>
        //                new InMemoryCached<object, object>(
        //                    new Mock<IInMemoryCacher>().Object,
        //                    o => "",
        //                    null));
        //        }

        //        [Fact]
        //        public void If_Memory_Key_Func_Is_Null()
        //        {
        //            Assert.Throws<ArgumentNullException>(() =>
        //                new InMemoryCached<object, object>(
        //                    new Mock<IInMemoryCacher>().Object,
        //                    null,
        //                    (key, arg) => Task.FromResult(arg)));
        //        }
        //    }
        //}

        //public sealed class GetOrFetchAsyncMethod
        //{
        //    [Fact]
        //    public async Task Will_Pass_The_Key_That_Was_Provided()
        //    {
        //        // Arrange
        //        var cacherMock = new Mock<IInMemoryCacher>();
        //        cacherMock.Setup(c => c.GetOrFetchAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<string>>>()))
        //            .Returns((string key, Func<string, Task<string>> fetch) => Task.FromResult(key));

        //        var memoryCached = new InMemoryCached<string, int>(
        //            cacherMock.Object,
        //            arg => arg.ToString(),
        //            (key, arg) => Task.FromResult(key));

        //        // Act
        //        var response = await memoryCached.GetOrFetchAsync(22);

        //        // Assert
        //        Assert.Equal("22", response);
        //    }

        //    [Fact]
        //    public async Task Will_Fetch_Data_Using_MemoryCacher_And_Argument()
        //    {
        //        // Arrange
        //        var cacherMock = new Mock<IInMemoryCacher>();
        //        cacherMock.Setup(c => c.GetOrFetchAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<string>>>()))
        //            .Returns((string key, Func<string, Task<string>> fetch) => Task.FromResult(fetch(key).Result + key));

        //        var memoryCached = new InMemoryCached<string, int>(
        //            cacherMock.Object,
        //            arg => "key_" + arg,
        //            (key, arg) => Task.FromResult("fetch_" + arg));

        //        // Act
        //        var response = await memoryCached.GetOrFetchAsync(22);

        //        // Assert
        //        Assert.Equal("fetch_22key_22", response);
        //    }
        //}
    }
}