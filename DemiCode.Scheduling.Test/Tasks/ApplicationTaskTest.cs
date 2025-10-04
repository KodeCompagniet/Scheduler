using DemiCode.Logging;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Tasks.ApplicationTask and is intended
    ///to contain all DemiCode.Scheduling.Tasks.ApplicationTask Unit Tests
    ///</summary>
    [TestFixture]
    public class ApplicationTaskTest
    {

        /// <summary>
        ///A test for Execute (IDictionary)
        ///</summary>
        [Test]
        public void ExecuteTest()
        {
            var task = new ApplicationTask("cmd", new[] { "/C \"echo fisk\"" });
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            task.Execute(loggerFactory);
        }

        #region QuoteArguments Tests

        /// <summary>
        /// Test that arguments without special characters are not quoted
        /// </summary>
        [Test]
        public void QuoteArguments_SimpleArguments_NotQuoted()
        {
            var args = new[] { "simple", "arg", "test" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("simple arg test", result);
        }

        /// <summary>
        /// Test that arguments with spaces are quoted
        /// </summary>
        [Test]
        public void QuoteArguments_ArgumentsWithSpaces_Quoted()
        {
            var args = new[] { "hello world", "test arg" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"hello world\" \"test arg\"", result);
        }

        /// <summary>
        /// Test that empty arguments are quoted as empty strings
        /// </summary>
        [Test]
        public void QuoteArguments_EmptyArgument_QuotedEmptyString()
        {
            var args = new[] { "" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"\"", result);
        }

        /// <summary>
        /// Test that arguments with embedded quotes are properly escaped
        /// </summary>
        [Test]
        public void QuoteArguments_ArgumentsWithQuotes_Escaped()
        {
            var args = new[] { "say \"hello\"" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"say \\\"hello\\\"\"", result);
        }

        /// <summary>
        /// Test that arguments with backslashes before quotes are properly escaped
        /// </summary>
        [Test]
        public void QuoteArguments_BackslashBeforeQuote_DoubledAndEscaped()
        {
            var args = new[] { "path\\\"file" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"path\\\\\\\"file\"", result);
        }

        /// <summary>
        /// Test that trailing backslashes in quoted arguments are doubled
        /// </summary>
        [Test]
        public void QuoteArguments_TrailingBackslashes_Doubled()
        {
            var args = new[] { "path with space\\" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"path with space\\\\\"", result);
        }

        /// <summary>
        /// Test that multiple trailing backslashes are not quoted if no spaces
        /// </summary>
        [Test]
        public void QuoteArguments_MultipleTrailingBackslashes_AllDoubled()
        {
            var args = new[] { "path\\\\" };
            var result = ApplicationTask.QuoteArguments(args);
            // No spaces, so no quoting needed - backslashes stay as-is
            Assert.AreEqual("path\\\\", result);
        }

        /// <summary>
        /// Test that backslashes before normal characters are not escaped
        /// </summary>
        [Test]
        public void QuoteArguments_BackslashBeforeNormalChar_NotEscaped()
        {
            var args = new[] { "path\\to\\file with space" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"path\\to\\file with space\"", result);
        }

        /// <summary>
        /// Test that arguments with tabs are quoted
        /// </summary>
        [Test]
        public void QuoteArguments_ArgumentsWithTabs_Quoted()
        {
            var args = new[] { "hello\tworld" };
            var result = ApplicationTask.QuoteArguments(args);
            Assert.AreEqual("\"hello\tworld\"", result);
        }

        /// <summary>
        /// Test complex real-world scenario with mixed arguments
        /// </summary>
        [Test]
        public void QuoteArguments_MixedComplexArguments_ProperlyQuoted()
        {
            var args = new[] {
                "simple",
                "with space",
                "with\"quote",
                "path\\to\\file",
                "trailing\\",
                ""
            };
            var result = ApplicationTask.QuoteArguments(args);
            // "simple" - no special chars, no quotes
            // "with space" - has space, quoted
            // "with\"quote" - has quote, quoted and escaped
            // "path\\to\\file" - no special chars, no quotes
            // "trailing\\" - no special chars, no quotes
            // "" - empty, quoted as ""
            Assert.AreEqual("simple \"with space\" \"with\\\"quote\" path\\to\\file trailing\\ \"\"", result);
        }

        /// <summary>
        /// Test that null array returns empty string
        /// </summary>
        [Test]
        public void QuoteArguments_NullArray_ReturnsEmpty()
        {
            var result = ApplicationTask.QuoteArguments(null);
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Test that empty array returns empty string
        /// </summary>
        [Test]
        public void QuoteArguments_EmptyArray_ReturnsEmpty()
        {
            var result = ApplicationTask.QuoteArguments(new string[0]);
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Test shell-sensitive characters that should not trigger shell interpretation
        /// </summary>
        [Test]
        public void QuoteArguments_ShellSensitiveCharacters_SafelyQuoted()
        {
            var args = new[] { "file|pipe", "redirect>out", "input<file", "cmd&next" };
            var result = ApplicationTask.QuoteArguments(args);
            // These should NOT be interpreted as shell operators when UseShellExecute=false
            Assert.AreEqual("file|pipe redirect>out input<file cmd&next", result);
        }

        #endregion

    }


}
