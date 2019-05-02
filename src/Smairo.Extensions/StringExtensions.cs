using System;
namespace Smairo.Extensions
{
    /// <summary>
    /// Contains general extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Make Contains type of check with provided StringComparison
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comparison"></param>
        /// <returns>bool</returns>
        // ReSharper disable once InconsistentNaming
        public static bool Contains(this string source, string toCheck, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return source.IndexOf(toCheck, comparison) >= 0;
        }
    }
}