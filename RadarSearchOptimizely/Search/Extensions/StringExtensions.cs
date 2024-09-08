using System.Text.RegularExpressions;
using RadarSearchOptimizely.Search.Models;

namespace RadarSearchOptimizely.Search.Extensions
{
    public static class StringExtensions
    {
        public static string HtmlStrip(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            input = Regex.Replace(input, "<style>(.|\n)*?</style>", " ");
            input = Regex.Replace(input, @"<xml>(.|\n)*?</xml>", " ");
            input = Regex.Replace(input, @"<(.|\n)*?>", " ");
            input = input.Replace("&nbsp;", " ");
            input = input.Replace("|", " ");
            input = Regex.Replace(input, @"\s+", " ");
            input = Regex.Replace(input, @"\s{2,}", " ");
            return input;
        }

        public static string CleanInput(this string input)
        {
            try
            {
                input = Regex.Replace(input, @"[^\w\.@-]", " ", RegexOptions.None, TimeSpan.FromSeconds(10.0));
                input = Regex.Replace(input, "[^A-Za-z0-9æøåÆØÅ]", " ");
                input = Regex.Replace(input, "&nbsp;", " ");
                return input;
            }
            // If we timeout when replacing invalid characters,  
            // we should return Empty. 
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }

        public static string RemoveWordSpesific(this string input)
        {
            var s = input;
            // smart single quotes and apostrophe
            s = Regex.Replace(s, "[\u2018\u2019\u201A]", " ");
            // smart double quotes
            s = Regex.Replace(s, "[\u201C\u201D\u201E]", " ");
            // ellipsis
            s = Regex.Replace(s, "\u2026", " ");
            // dashes
            s = Regex.Replace(s, "[\u2013\u2014]", " ");
            // circumflex
            s = Regex.Replace(s, "\u02C6", " ");
            // open angle bracket
            s = Regex.Replace(s, "\u2039", " ");
            // close angle bracket
            s = Regex.Replace(s, "\u203A", " ");
            // spaces
            s = Regex.Replace(s, "[\u02DC\u00A0]", " ");
            return s;
        }

        public static string RemoveDuplcateSpaces(this string input)
        {
            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        public static string FilterAclNames(this string entry)
        {
            var newEntry = Regex.Replace(entry, ".*\\\\(.*)", "$1", RegexOptions.None);
            return RadarDictionary.Instance.ReplaceAclChars(newEntry);
        }
    }
}