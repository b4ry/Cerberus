using Cerberus.Api.Attributes;

namespace Tests.Attributes
{
    public class DevaultValueValidationAttributeTests
    {
        [Theory]
        [InlineData(null, false)] // Null value
        [InlineData("", false)] // Empty string
        [InlineData("test", true)] // Non-empty string
        [InlineData(0, false)] // Default int value
        [InlineData(1, true)] // Non-default int value
        public void IsValid_ShouldValidateCorrectly(object value, bool expected)
        {
            // Arrange
            var attribute = new DevaultValueValidationAttribute("Invalid value");

            // Act
            var result = attribute.IsValid(value);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
