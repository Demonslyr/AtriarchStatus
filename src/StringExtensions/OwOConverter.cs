using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OwOConverter.StringExtensions.OwOConverter
{
    public static class OwOConverter
    {
        private static readonly Random RandomInt = new Random((int)DateTime.UtcNow.Ticks);
        private static readonly List<string> Faces = new List<string> { ";w;", "owo", "UwU", ">w<", "^w^", "*w*" };//"(`・ω・´)(・`ω´・)" doesn't encode right :C
        private static readonly MatchEvaluator FaceEvaluator = _ => " " + Faces[RandomInt.Next(Faces.Count)] + " ";
        public static string ConvertToOwO(this string input)
        {

            input = Regex.Replace(input, @"(r|l)", "w", RegexOptions.Multiline);
            input = Regex.Replace(input, @"(R|L)", "W", RegexOptions.Multiline);
            input = Regex.Replace(input, "n([aeiou])", @"ny$1", RegexOptions.Multiline);
            input = Regex.Replace(input, "N([aeiou])", @"Ny$1", RegexOptions.Multiline);
            input = Regex.Replace(input, "N([AEIOU])", @"Ny$1", RegexOptions.Multiline);
            input = Regex.Replace(input, @"(ove)", "uv", RegexOptions.Multiline);
            input = Regex.Replace(input, @"(ou)", "ew", RegexOptions.Multiline);
            input = Regex.Replace(input, @"\!+", FaceEvaluator);
            return input;
        }
    }
}
