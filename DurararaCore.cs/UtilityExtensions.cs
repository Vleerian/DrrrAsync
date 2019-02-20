using System;

namespace DrrrAsync
{
    /// <summary>
    /// Holding class for Utility Extensions
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// Pads both sides of a string to center it.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="length">How long you want the end string to be.</param>
        /// <returns>The padded string.</returns>
        public static string PadBoth(this string source, int length)
        {
            if (source.Length > length)
                throw new ArgumentException("Source string is longer than provided length.");
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, ' ').PadRight(length, ' ');
        }
    }
}
