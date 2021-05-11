
#region References
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Fintek.Common.IMAP
{
    internal class ImapDecode
    {
        /// <summary>
        /// Decodes from ASCII representations of characters in the given encoding
        /// </summary>
        /// <param name="input">The input, can be many lines long, but only in one encoding.</param>
        /// <param name="enc">The encoding the input is coming from</param>
        /// <returns>The decoded string, suitable for .Net use</returns>
        internal static string Decode(string input, Encoding enc)
        {
            if (input == "" || input == null)
                return "";
            string decoded = "";
            //byte[] bytes;
            MatchCollection matches = Regex.Matches(input, @"\=(?<num>[0-9A-Fa-f]{2})");// Substring(input.IndexOf('=') + 1, 2);
            foreach (Match match in matches) //while (input.Contains("="))
            {
                //string ttr = Regex.Match("input", @"=(?<num>[0-9A-Fa-f]{2})").Groups[num].Substring(input.IndexOf('=') + 1, 2);
                //int i = int.Parse(ttr, System.Globalization.NumberStyles.HexNumber);
                int i = int.Parse(match.Groups["num"].Value, System.Globalization.NumberStyles.HexNumber);
                char str = (char)i;
                input = input.Replace(match.Groups[0].Value, str.ToString());
            }
            /*bytes = System.Text.Encoding.Default.GetBytes(input);
            decoded = enc.GetString(bytes);*/
            
            
            System.Text.Encoding encodIso = System.Text.Encoding.GetEncoding("ISO-8859-9");
            byte[] t = encodIso.GetBytes(input);
            decoded = enc.GetString(t);
            
            return decoded;
        }
         /// <summary>
        /// Decodes a string to it's native encoding 
        /// Pulls from the string correcting multiple =?ENC?METHOD?TEXT?=
        /// </summary>
        /// <param name="input">The string with embedded encoding(s)</param>
        /// <returns>The decoded string, suitable for .Net use</returns>
        internal static string Decode(string input)
        {
            if (input == "" || input == null)
                return "";
            Regex regex = new Regex(@"=\?(?<Encoding>[^\?]+)\?(?<Method>[^\?]+)\?(?<Text>[^\?]+)\?=");
            MatchCollection matches = regex.Matches(input);
            string decoded = "";
            string ret = input;
            foreach (Match match in matches)
            {
                string encoding = match.Groups["Encoding"].Value;
                string method = match.Groups["Method"].Value;
                string text = match.Groups["Text"].Value;

                try
                {
                    byte[] b = System.Convert.FromBase64String(text);
                    text = Encoding.GetEncoding(encoding).GetString(b);
                    decoded = text;
                }
                catch
                {
                    text = text.Replace("=FC", "�");
                    text = text.Replace("=DC", "�");
                    text = text.Replace("=F0", "�");
                    text = text.Replace("=D0", "�");
                    text = text.Replace("=F6", "�");
                    text = text.Replace("=D6", "�");
                    text = text.Replace("=E7", "�");
                    text = text.Replace("=C7", "�");
                    text = text.Replace("=FD", "�");
                    text = text.Replace("=DD", "�");
                    text = text.Replace("=FE", "�");
                    text = text.Replace("=DE", "�");
                    text = text.Replace("=3D", "=");
                    text = text.Replace("_", " ");
                    text = text.Replace("=5F", "_");

                    decoded = Decode(text, Encoding.GetEncoding(encoding));
                    
                }
                ret = ret.Replace(match.Groups[0].Value, decoded);
            }
            return ret;
        }
    }
}
