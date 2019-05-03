using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SampleFunctions.Extensions
{
    public static class StringExtensions
    {
        public static bool CompareMultiple(this string self, StringComparison compareType, params string[] compareValues)
        {
            foreach (var s in compareValues)
                if (self.Equals(s, compareType))
                    return true;

            return false;
        }

        public static bool IsEmptyUpload(this string self)
        {
            if (string.IsNullOrEmpty(self) || self == "No File Uploaded")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static bool IsEmpty(this Guid self)
        {
            return self == Guid.Empty;
        }

        public static string[] SplitText(this string self, string symbol)
        {
            return self.IsEmpty() ? new string[] { } : self.Split(new[] { symbol }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string FormatText(this string self, params object[] args)
        {
            return string.Format(self, args);
        }

        public static bool EqualsIgnoringCase(this string self, string value)
        {
            if (self == null)
                return value == null;

            return self.Equals(value, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Parses a camel cased or pascal cased string and returns a new
        /// string with spaces between the words in the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string SplitUpperCase(this string self)
        {
            return Regex.Replace(self, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        public static string FirstLetterToLowerCase(this string self, string separator = null)
        {
            if (self.IsEmpty())
                return null;

            self = self.Trim();
            if (self.Length == 1)
                return self.ToLower();

            if (!separator.IsEmpty())
                return string.Join(separator, self.SplitText(separator).Select(s => s.FirstLetterToLowerCase()));

            return Char.ToLower(self[0]) + self.Substring(1);
        }

        public static string FirstLetterToUpperCase(this string self)
        {
            if (self.IsEmpty())
                return null;

            self = self.Trim();
            if (self.Length == 1)
                return self.ToUpper();

            return Char.ToUpper(self[0]) + self.Substring(1);
        }

        public static string TrimText(this string self, string symbols)
        {
            return self.IsEmpty() ? null : self.Trim(symbols.ToCharArray());
        }

        public static string TrimText(this string self, int maxLength)
        {
            if (self.IsEmpty())
                return null;

            return self.Length <= maxLength ? self : self.Substring(0, maxLength) + " ...";
        }

        public static string ToCommaSeparated(this IEnumerable<int> self)
        {
            return (self ?? Enumerable.Empty<int>()).Select(v => v.ToString()).ToCommaSeparated();
        }

        public static string ToCommaSeparated(this IEnumerable<string> self)
        {
            return self == null || !self.Any() ? string.Empty : string.Join(",", self);
        }

        public static IEnumerable<string> FromCommaSeparated(this string self)
        {
            return self.IsEmpty() ? Enumerable.Empty<string>() : self.Split(',');
        }

        public static string Remove(this string self, char[] chars)
        {
            return chars.Aggregate(self, (current, item) => current.Replace(item.ToString(), string.Empty));
        }

        public static string ToUtf8String(this byte[] self, bool removeEncodingInfo = false)
        {
            if (self == null || self.Length == 0)
                return null;

            var result = Encoding.UTF8.GetString(self);
            if (removeEncodingInfo)
                result = result.Replace(Encoding.UTF8.GetPreamble().ToUtf8String(), "");

            return result;
        }

        public static string ToAsciiString(this string self, string nonAsciiSymbolReplacement = "_")
        {
            if (self.IsEmpty())
                return self;

            var asciiEncoding = Encoding.GetEncoding(Encoding.ASCII.EncodingName,
                new EncoderReplacementFallback(nonAsciiSymbolReplacement),
                new DecoderExceptionFallback());

            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, asciiEncoding, Encoding.UTF8.GetBytes(self)));
        }

        public static byte[] ToBytes(this string self)
        {
            return self.IsEmpty() ? new byte[] { } : Encoding.UTF8.GetBytes(self);
        }

        public static string Shorten(this Guid self)
        {
            return Convert.ToBase64String(self.ToByteArray()).
                Replace("/", "x").
                Replace("+", "y").
                Replace("=", "").
                Substring(0, 22);
        }


        public static int GetPrecision(this decimal self)
        {
            self = decimal.Parse(self.ToString("G29"));
            return (decimal.GetBits(self)[3] >> 16) & 0x000000FF;
        }



        public static string BytesToHexString(this byte[] self)
        {
            if (self == null)
                return null;

            if (self.Length == 0)
                return "";

            return BitConverter.ToString(self).Replace("-", "");
        }

        public static T FromJson<T>(this string self)
        {
            return (T)FromJson(self, typeof(T));
        }

        public static object FromJson(this string self, Type type)
        {
            if (self.IsEmpty())
                return null;

            return JsonConvert.DeserializeObject(self, type);
        }

        public static string ToJson(this object self, bool prettyPrint = false)
        {
            if (self == null)
                return null;

            var formatting = Formatting.None;
            if (prettyPrint)
                formatting = Formatting.Indented;

            var result = JsonConvert.SerializeObject(self, formatting);
            return result;
        }

        public static string Truncate(this string self, int maxLength)
        {
            if (string.IsNullOrEmpty(self))
                return self;

            return self.Length <= maxLength ? self : self.Substring(0, maxLength);
        }

        public static string AddSlashes(this string self)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            var Result = self;

            try
            {
                Result = Regex.Replace(self, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }


        #region - AsBool -

        public static bool AsBool(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToBoolean(self);
            }

            if (self is string)
            {
                var str = (string)self;
                if (str.IsEmpty())
                    return false;

                bool value;
                return bool.TryParse(str, out value) && value;
            }

            try
            {
                return Convert.ToBoolean(self);
            }
            catch
            {
                return false;
            }
        }

        public static string NullIfEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            return str;
        }

        #endregion

        #region - ToBase64 -

        public static string ToBase64(this string self, bool safe = false)
        {
            if (!safe)
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(self));

            if (self == null)
                return null;

            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(self));
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region - FromBase64 -

        public static string FromBase64(this string self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Encoding.UTF8.GetString(Convert.FromBase64String(self));
            }

            if (self.IsEmpty())
                return null;

            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(self));
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region - AsDateTime -

        public static DateTime AsDateTime(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToDateTime(self);
            }

            if (self is string)
            {
                var str = (string)self;
                if (str.IsEmpty())
                    return DateTime.MinValue;

                DateTime value;
                return !DateTime.TryParse(str, out value) ? DateTime.MinValue : value;
            }

            try
            {
                return Convert.ToDateTime(self);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        #endregion

        #region - AsGuid -

        public static Guid AsGuid(this object self)
        {
            if (self is Guid)
                return (Guid)self;
            if (self is string)
                return new Guid((string)self);
            if (self is byte[])
                return new Guid((byte[])self);

            return Guid.Empty;
        }

        #endregion

        #region - AsInt -

        public static int AsInt(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToInt32(self);
            }

            if (self is string)
            {
                int value;
                return !int.TryParse((string)self, out value) ? 0 : value;
            }

            try
            {
                return Convert.ToInt32(self);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region - AsLong -

        public static long AsLong(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToInt64(self);
            }

            if (self is string)
            {
                long value;
                return !long.TryParse((string)self, out value) ? 0 : value;
            }

            try
            {
                return Convert.ToInt64(self);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region - AsDouble -

        public static double AsDouble(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToDouble(self);
            }

            if (self is string)
            {
                double value;
                return !double.TryParse((string)self, out value) ? 0 : value;
            }

            try
            {
                return Convert.ToDouble(self);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region - AsFloat -

        public static float AsFloat(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToSingle(self);
            }

            if (self is string)
            {
                float value;
                return !float.TryParse((string)self, out value) ? 0 : value;
            }

            try
            {
                return Convert.ToSingle(self);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region - AsDecimal -

        public static decimal AsDecimal(this object self, bool safe = false)
        {
            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return Convert.ToDecimal(self);
            }

            if (self is string)
            {
                decimal value;
                return !decimal.TryParse((string)self, out value) ? 0 : value;
            }

            try
            {
                return Convert.ToDecimal(self);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region - AsString -

        public static string AsString(this object self)
        {
            return Convert.ToString(self);
        }

        #endregion

        #region - AsEnum -

        public static T AsEnum<T>(this object self, bool safe = false)
        {
            if (self is int) return (T)self;

            if (!safe)
            {
                if (self == null)
                    throw new ArgumentNullException("self");

                return (T)Enum.Parse(typeof(T), (string)self, true);
            }

            try
            {
                return (T)Enum.Parse(typeof(T), (string)self, true);
            }
            catch
            {
                return default(T);
            }
        }

        #endregion

        #region - AsTimeSpan -

        /// <summary>
        /// Converts the string into TimeSpan value.
        /// </summary>
        /// <param name="self">The string to convert.</param>
        /// <returns>
        /// The converted value or the default value if the conversion can not be made.
        /// </returns>
        public static TimeSpan AsTimeSpan(this string self)
        {
            if (self.IsEmpty())
                return TimeSpan.Zero;

            var units = self.Trim().Where(char.IsLetter);
            var value = self.TrimEnd(units.ToArray());
            switch (String.Join("", units))
            {
                default:
                    return new TimeSpan(0, 0, AsInt(value));
                case "ms":
                    return new TimeSpan(0, 0, 0, 0, AsInt(value));
                case "s":
                    return new TimeSpan(0, 0, AsInt(value));
                case "m":
                    return new TimeSpan(0, AsInt(value), 0);
                case "h":
                    return new TimeSpan(AsInt(value), 0, 0);
            }
        }

        #endregion
    }
}
