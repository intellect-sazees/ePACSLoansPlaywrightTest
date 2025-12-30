using NUnit.Framework;
using NUnit.Framework.Legacy;
using ClassicAssert = NUnit.Framework.Legacy.ClassicAssert;
using System;

namespace ePACSLoans.Utilities.Helpers
{
    public static class AssertHelper
    {
        public static void AssertElementVisible(bool isVisible, string elementName)
        {
            ClassicAssert.IsTrue(isVisible, $"Element '{elementName}' should be visible but was not found.");
        }

        public static void AssertElementNotVisible(bool isVisible, string elementName)
        {
            ClassicAssert.IsFalse(isVisible, $"Element '{elementName}' should not be visible but was found.");
        }

        public static void AssertTextEquals(string actual, string expected, string elementName)
        {
            ClassicAssert.AreEqual(expected, actual, $"Text in '{elementName}' does not match expected value.");
        }

        public static bool AssertTextContains(string actual, string expectedSubstring, string elementName)
        {
            ClassicAssert.IsTrue(actual.Contains(expectedSubstring),$"Text in '{elementName}' does not contain expected substring '{expectedSubstring}'. Actual: '{actual}'");
            return true;
        }

        public static void AssertUrlEquals(string actual, string expected)
        {
            ClassicAssert.AreEqual(expected, actual, $"Current URL does not match expected URL.");
        }

        public static void AssertUrlContains(string actual, string expectedSubstring)
        {
            ClassicAssert.IsTrue(actual.Contains(expectedSubstring),
                $"Current URL does not contain expected substring '{expectedSubstring}'. Actual: '{actual}'");
        }

        public static void AssertElementEnabled(bool isEnabled, string elementName)
        {
            ClassicAssert.IsTrue(isEnabled, $"Element '{elementName}' should be enabled but was disabled.");
        }

        public static void AssertElementDisabled(bool isEnabled, string elementName)
        {
            ClassicAssert.IsFalse(isEnabled, $"Element '{elementName}' should be disabled but was enabled.");
        }
    }
}
