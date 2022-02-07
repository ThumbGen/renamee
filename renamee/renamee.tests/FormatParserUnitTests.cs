using FluentAssertions;
using renamee.Server.Helpers;
using System;
using Xunit;

namespace renamee.tests
{
    public class FormatParserUnitTests
    {
        static DateTimeOffset DateTimeOffsetToTest = new DateTimeOffset(2022, 02, 07, 18, 15, 22, TimeSpan.Zero);

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "filename.jpg")]
        [InlineData("YEAR", null)]
        public void FormatParser_ShouldThrowArgumentNullExceptionOnMissingArguments(string format, string originalFilename)
        {
            var parser = new FormatParser();

            Assert.Throws<ArgumentNullException>(() =>
            {
                parser.TryParse(DateTimeOffsetToTest, format, originalFilename, out string result);
            });
        }

        [Theory]
        [InlineData("YEAR", "original.jpg", "2022")]
        [InlineData("YEAR|YEAR.MONTH.DAY|YEAR.MONTH.DAY-HOUR.MIN.SEC-ORG", "original.jpg", "2022|2022.02.07|2022.02.07-18.15.22-original")]
        [InlineData("YEAR.MONTH.DAY-HOUR.MIN.SEC", "original.jpg", "2022.02.07-18.15.22")]
        public void FormatParser_ShouldReturnCorrectResults(string format, string originalFilename, string expectedResult)
        {
            var parser = new FormatParser();
            parser.TryParse(DateTimeOffsetToTest, format, originalFilename, out string result);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("\\/:*?”<>")]
        [InlineData("B")]
        [InlineData("FOO")]
        [InlineData("SOME|NOT|supported.format")]
        [InlineData("YEAR.MONTH.DAY-HOUR.MIN.SEX")]
        [InlineData("YEAR.MONTH|ORIGINAL||HOUR")]
        public void FormatParser_ValidateReturnsFalseForNotAcceptedCharacters(string format)
        {
            var parser = new FormatParser();
            var result = parser.Validate(format);
            result.Should().Be(false);
        }

        [Theory]
        [InlineData("YEAR.MONTH|ORG|HOUR")]
        [InlineData("YEAR.MONTH.DAY-HOUR.MIN.SEC")]
        [InlineData("YEAR|YEAR.MONTH.DAY|YEAR.MONTH.DAY-HOUR.MIN.SEC-ORG")]
        public void FormatParser_ValidateReturnsTrueForCorrectFormats(string format)
        {
            var parser = new FormatParser();
            var result = parser.Validate(format);
            result.Should().Be(true);
        }

        [Fact]
        public void FormatParser_AcceptedTokensListIsCorrect()
        {
            FormatParser.AcceptedTokens.Should().BeEquivalentTo(new[] { FormatParser.Year, FormatParser.Month, FormatParser.Day, FormatParser.Hours, FormatParser.Minute, FormatParser.Seconds, FormatParser.OriginalName });
        }
    }
}