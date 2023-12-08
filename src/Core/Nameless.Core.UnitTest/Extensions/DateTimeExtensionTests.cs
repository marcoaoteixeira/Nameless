namespace Nameless.Core.UnitTest.Extensions {
    public class DateTimeExtensionTests {
        [TestCase("2000-6-1", "2005-5-1", 4)]
        [TestCase("2000-6-1", "2005-6-1", 5)]
        [TestCase("2005-6-1", "2000-5-1", 5)]
        [TestCase("2000-1-1", "2000-12-30", 0)]
        [TestCase("2000-1-1", "2000-12-31", 1)]
        public void GetYears_Returns_The_Number_Of_Years_Between_Two_Dates(string first, string second, int expectedYears) {
            // arrange
            // < 5 years
            var firstDate = DateTime.Parse(first);
            var secondDate = DateTime.Parse(second);

            // act
            var years = DateTimeExtension.GetYears(firstDate, secondDate);

            // assert
            Assert.That(years, Is.EqualTo(expectedYears));
        }
    }
}
