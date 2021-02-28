using System;
using System.Collections.Generic;
using System.Linq;

namespace Pjfm.Domain.ValueObjects
{
    public static class StringExtensions
    {
        // will subString the string if the string length exceeds the maxLength
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
        
        // converts pascalCase to snakeCase
        public static string PascalToSnakeCase(this string value)
        {
            int substringStart = 0;
            var substrings = new List<string>();
            var result = String.Empty;
            
            for (int i = 0; i < value.Length; i++)
            {
                // Checks to see if value[i] is an uppercase char
                if (value[i] >= 'A' && value[i] <= 'Z' && i != 0)
                {
                    substrings.Add(value.Substring(substringStart, i - substringStart));
                    substringStart = i;
                }
            }
			
            // substring the last value
            substrings.Add(value.Substring(substringStart, value.Length - substringStart));

            // joins all the substrings by concatenating it to result
            for (int i = 0; i < substrings.Count; i++)
            {
                if (i == 0)
                {
                    result = substrings[i].ToLower();
                }
                else
                {
                    result += "_" + substrings[i].ToLower();
                }
            }

            return result;
        }
    }
}