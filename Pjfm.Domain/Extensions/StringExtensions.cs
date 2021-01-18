using System;

namespace Pjfm.Domain.ValueObjects
{
    public static class StringExtensions
    {
        public static string WithMaxLength(this string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }
            if (maxLength < 0)
            {
                return "";
            }
            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}