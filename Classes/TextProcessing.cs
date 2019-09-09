using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIC.Classes
{
    public class TextProcessing
    {
        public static List<string> UnusedWords = new List<string>()
        {
            " a ", " o ", " e ", " ao ", " aos ", " as ", " do ", " dos ", " da ", " das ", " no ", " nos ", " na ", " nas ", " pela", " num ", " numa ", " de "
        };
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
        public async static Task<string> GetTextProcessed(string text, int startFromIndex)
        {
            await Task.Run(() =>
            {
                text = text.Substring(startFromIndex);
                text = TextProcessing.RemoveUnusedUtterances(text);
                if (!String.IsNullOrEmpty(text))
                {
                    text = text.First() == ' ' ? text.Substring(1, text.Length - 1) : text;
                }
                text = text.ToLower();
            });
            return text;
        }
        public static string RemoveUnusedUtterances(string text)
        {
            foreach (string w in UnusedWords)
            {
                text = text.Replace(w, " ");
            }
            return text;
        }
    }
}
