namespace N88.Logging.Spec
{
    using System.Collections;
    using System.Text;
    using FluentAssertions;
    using NUnit.Framework;

    public class ConsoleLogger
    {
        private const string AnyText = "anyText";
        private readonly StringBuilder _sb = new();
        private StringWriter _writer;
        private const string AnyColor = "#ffffff";
        private const string AnyCategory = "AnyCategory";
        
        [SetUp]
        public void Setup()
        {
            _sb.Clear();
            _writer = new StringWriter(_sb);
            Console.SetOut(_writer);
        }

        [TearDown]
        public void TearDown()
        {
            _writer.Dispose();
        }

        [TestCase(AnyText)]
        [TestCase("tnfeuayduyf$}(^llfuayt")]
        public void Logs_exact_text_to_console(string text)
        {
            var logger = new N88.Logging.ConsoleLogger();
            logger.Log(text);

            _sb.ToString().Should().Be(text);
        }

        [TestCase("Default", "Default")]
        [TestCase("Debug", "Debug")]
        [TestCase("Debug,Warning", "Warning")]
        public void Only_logs_to_enabled_categories(string categories, string loggedCategory)
        {
            string?[] enabledCategories = categories.Split(',');
            const string UnexpectedText = "unexpected text";
            const string DisabledCategory = "DisabledCategory";
            
            var loggingCategories = new LoggingSettings[enabledCategories.Length];
            for (var i = 0; i < enabledCategories.Length; i++)
            {
                loggingCategories[i] = new LoggingSettings(enabledCategories[i]!, string.Empty, AnyColor, string.Empty);
            }

            var logger = new N88.Logging.ConsoleLogger(loggingCategories);
            logger.Log(AnyText, loggedCategory);
            logger.Log(UnexpectedText, DisabledCategory);

            _sb.ToString().Should().Be(AnyText);
        }

        [TestCase("Debug", "[D]:")]
        [TestCase("Whatever", "whtev-")]
        public void Prepends_display_name_if_present(string category, string displayName)
        {
            var loggingCategory = new LoggingSettings(category, displayName, AnyColor, string.Empty);
            var logger = new N88.Logging.ConsoleLogger(loggingCategory);
            logger.Log(AnyText, category);
            
            _sb.ToString().Should().StartWith(displayName);
        }

        

        [Test]
        public void Prepends_time_if_enabled()
        {
            const string TimeFormat = "HH:mm:ss z";
            var loggingCategory = new LoggingSettings(AnyCategory, string.Empty, AnyColor, TimeFormat);
            var logger = new N88.Logging.ConsoleLogger(loggingCategory);

            var expectedTime = $"[{DateTime.Now.ToString(TimeFormat)}]: ";
            logger.Log(AnyText, AnyCategory);
            
            _sb.ToString().Should().StartWith(expectedTime);
        }

        

        [TestCaseSource(typeof(TestData), nameof(TestData.ColorCategoryTestCases))]
        public void Logs_color_assigned_to_category(string category, string color, ConsoleColor expectedColor)
        {
            var loggingCategory = new LoggingSettings(category, string.Empty, color, string.Empty);
            var logger = new N88.Logging.ConsoleLogger(loggingCategory);
            logger.Log(AnyText, category);

            Console.ForegroundColor.Should().Be(expectedColor);
            _sb.ToString().Should().Be(AnyText);
        }

        private class TestData
        {
            public static IEnumerable ColorCategoryTestCases()
            {
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Black,
                        ConsoleColor.Black)
                    .SetName("Debug Black");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkBlue,
                        ConsoleColor.DarkBlue)
                    .SetName("Debug DarkBlue");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkGreen,
                        ConsoleColor.DarkGreen)
                    .SetName("Debug DarkGreen");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkCyan,
                        ConsoleColor.DarkCyan)
                    .SetName("Debug DarkCyan");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkRed,
                        ConsoleColor.DarkRed)
                    .SetName("Debug DarkRed");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkMagenta,
                        ConsoleColor.DarkMagenta)
                    .SetName("Debug DarkMagenta");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkYellow,
                        ConsoleColor.DarkYellow)
                    .SetName("Debug DarkYellow");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Gray,
                        ConsoleColor.Gray)
                    .SetName("Debug Gray");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.DarkGray,
                        ConsoleColor.DarkGray)
                    .SetName("Debug DarkGray");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Blue,
                        ConsoleColor.Blue)
                    .SetName("Debug Blue");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Green,
                        ConsoleColor.Green)
                    .SetName("Debug Blue");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Cyan,
                        ConsoleColor.Cyan)
                    .SetName("Debug Cyan");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Red,
                        ConsoleColor.Red)
                    .SetName("Debug Red");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Magenta,
                        ConsoleColor.Magenta)
                    .SetName("Debug Magenta");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.Yellow,
                        ConsoleColor.Yellow)
                    .SetName("Debug Yellow");
                yield return new TestCaseData("Debug", 
                        Logging.ConsoleLogger.White,
                        ConsoleColor.White)
                    .SetName("Debug White");
            }
        }
    }
}