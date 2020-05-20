using DuplicateFinder;
using Xunit;

namespace DuplicateFinderTests
{
    public class ConfigServiceTest
    {
        [Fact]
        public void FilterExtension()
        {
            // Given
            var configService = new ConfigService();

            var expected = new[] {".jpg", ".txt"};

            configService.SetFilterExtension(expected);

            // When
            var actual = configService.GetFilterExtension();

            // Then
            Assert.Equal(expected, actual);
        }
    }
}
