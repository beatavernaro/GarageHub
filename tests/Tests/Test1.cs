using FluentAssertions;

namespace Tests
{
    public class ExampleTests
    {
        [Fact]
        public void Should_Add_Two_Numbers()
        {
            // Arrange
            var firstNumber = 2;
            var secondNumber = 3;

            // Act
            var result = firstNumber + secondNumber;

            // Assert
            result.Should().Be(5);
        }
    }
}