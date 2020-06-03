namespace Cached.Tests.Locking
{
    using System;
    using Cached.Locking;
    using Xunit;

    public class ReservableTests
    {
        public class Constructor
        {
            public class WillThrowException
            {
                [Fact]
                public void When_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new Reservable<object>(null));
                }
            }
        }

        public class ReserveMethod
        {
            [Fact]
            public void Will_Increase_Reservation_By_One()
            {
                // Arrange
                var item = new Reservable<object>(new object());

                // Act
                var preReserve = item.Reservations;
                item.Reserve();
                var midReserveCount = item.Reservations;
                item.Reserve();
                var postReserveCount = item.Reservations;

                // Assert
                Assert.Equal(0, preReserve);
                Assert.Equal(1, midReserveCount);
                Assert.Equal(2, postReserveCount);
            }
        }

        public class TryReleaseMethod
        {
            public class Throws
            {
                [Fact]
                public void When_Releasing_An_Already_Released_Object()
                {
                    // Arrange
                    var reserved = new Reservable<int>(1);

                    // Act, Assert
                    Assert.Throws<InvalidOperationException>(() => reserved.TryRelease());
                }
            }

            [Fact]
            public void Releases_Correctly_And_Return_True_If_Fully_Released()
            {
                // Arrange
                var item = new Reservable<object>(new object());
                item.Reserve();
                item.Reserve();

                // Act, Assert
                Assert.Equal(2, item.Reservations);
                Assert.False(item.TryRelease());
                Assert.Equal(1, item.Reservations);
                Assert.True(item.TryRelease());
                Assert.Equal(0, item.Reservations);
            }
        }
    }
}