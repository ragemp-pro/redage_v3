using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace NeptuneEvo.Utils.Base36
{
    public class Reposotory
    {
        
        private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const string Min = "-1Y2P0IJ32E8E8";

        private const string Max = "1Y2P0IJ32E8E7";

        /// <summary>
        /// Checks if the given value would cause a overflow (by being out of the long.MinValue to long.MaxValue range).
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value would cause an overflow, false otherwise</returns>
        public static bool WouldOverflow(string value)
        {
            return _Compare(Min, value) < 0 ||_Compare(value, Max) < 0;
        }

        /// <summary>
        /// Compare two specified base 36 values and returns an integer that indicates their relative position in the
        /// sort order, similar to the string.Compare method.
        /// <param name="valueA">First value of the comparison</param>
        /// <param name="valueB">Second value of the comparison</param>
        /// <returns>A integer indicating how the two values relate together</returns>
        public static int Compare(string valueA, string valueB)
        {
            return _Compare(Sanitize(valueA), Sanitize(valueB));
        }

        /// <summary>
        /// Converts from base 36 to base 10.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Value in base 10</returns>
        public static long Decode(string value)
        {
            value = Sanitize(value);

            CheckOverflow(value);

            var negative = value[0] == '-';

            value = Abs(value);

            var decoded = 0L;

            for (var i = 0; i < value.Length; ++i)
                decoded += Digits.IndexOf(value[i]) * (long)BigInteger.Pow(Digits.Length, value.Length - i - 1);

            return negative ? decoded * -1 : decoded;
        }

        /// <summary>
        /// Converts from base 10 to base 36.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Value in base 36</returns>
        public static string Encode(long value)
        {
            // hard coded value due to "Negating the minimum value of a twos complement number is invalid."
            if (value == long.MinValue)
                return Min;

            var negative = value < 0;

            value = Math.Abs(value);

            var encoded = string.Empty;

            do
                encoded = Digits[(int)(value % Digits.Length)] + encoded;
            while ((value /= Digits.Length) != 0);

            return negative ? "-" + encoded : encoded;
        }

        private static string Abs(string value)
        {
            return value[0] == '-' ? value.Substring(1, value.Length - 1) : value;
        }

        private static void CheckOverflow(string value)
        {
            if (_Compare(Min, value) < 0)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "Value \"{0}\" would overflow since it's less than long.MinValue.", value));

            if (_Compare(value, Max) < 0)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "Value \"{0}\" would overflow since it's greater than long.MaxValue.", value));
        }

        private static string Sanitize(string value)
        {
            if (value == null)
                throw new ArgumentNullException("An null string was passed.", (Exception)null);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("An empty string was passed.");

            value = value.Trim().ToUpperInvariant();

            if (Abs(value).Any(c => !Digits.Contains(c)))
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Invalid value: \"{0}\".", value));

            return value;
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes",
            Justification = "Exception thrown only if there's a logic error below, the user should never catch it.")]
        private static int _Compare(string valueA, string valueB)
        {
            if (valueA == valueB)
                return 0;

            var negativeA = valueA[0] == '-';
            var negativeB = valueB[0] == '-';

            var bothNegative = negativeA && negativeB;

            if (!bothNegative && negativeA)
                return 1;

            if (!bothNegative && negativeB)
                return -1;

            valueA = Abs(valueA);
            valueB = Abs(valueB);

            if (valueA.Length < valueB.Length)
                return bothNegative ? -1 : 1;

            if (valueA.Length > valueB.Length)
                return bothNegative ? 1 : -1;

            for (var i = 0; i < valueA.Length; ++i)
            {
                var digitA = Digits.IndexOf(valueA[i]);
                var digitB = Digits.IndexOf(valueB[i]);

                if (digitA != digitB)
                    return (digitA < digitB ? 1 : -1) * (bothNegative ? -1 : 1);
            }

            throw new Exception("Logic error in the library, please contact the library author.");
        }
    }
}