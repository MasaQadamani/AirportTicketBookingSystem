using System;
using System.Collections.Generic;
using System.Text;

namespace AirportTicketBookingSystem.Tests
{
    public class FutureDataAttributeShould
    {

        [Fact]
        public void ReturnFalse_When_DepartureDateIsInThePast()
        {
            // Arrange
            var attribute = new FutureDateAttribute();
            var pastDate = DateTime.Today.AddDays(-1);

            // Act
            bool result = attribute.IsValid(pastDate);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public void ReturnTrue_When_DepartureDateIsInTheFuture()
        {
            // Arrange
            var attribute = new FutureDateAttribute();
            var futureDate = DateTime.Today.AddDays(5);

            // Act
            bool result = attribute.IsValid(futureDate);

            // Assert
            Assert.True(result);
        }
    }
}
