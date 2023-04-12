using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OtpNet;
using Redage.SDK;

namespace NeptuneEvo.Chars
{
    class Generate
    {
        private static readonly nLog Log = new nLog("Chars.Generate");

        private static Random random = new Random();

        private const string DefaultPassword = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        private const string OneTimePassword = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static string RandomString(int length, bool type = false)
        {
            try
            {
                if(!type) return new string(Enumerable.Repeat(DefaultPassword, length).Select(s => s[random.Next(s.Length)]).ToArray());
                else return new string(Enumerable.Repeat(OneTimePassword, length).Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch (Exception e)
            {
                Log.Write($"RandomString Exception: {e.ToString()}");
                return null;
            }
        }

        public static string RandomOneTimePassword()
        {
            try
            {
                string randomstring = RandomString(random.Next(6, 10), true);
                if(randomstring == null) return null;
                byte[] secretkey = Base32Encoding.ToBytes(randomstring);
                Totp totp = new Totp(secretkey);
                return totp.ComputeTotp();
            }
            catch (Exception e)
            {
                Log.Write($"RandomOneTimePassword Exception: {e.ToString()}");
                return null;
            }
        }

        public static string ObfuscateEmail(string email)
        {
            try
            {
                string displayCase = email;

                string partToBeObfuscated = Regex.Match(displayCase, @"[^@]*").Value;
                if (partToBeObfuscated.Length - 3 > 0)
                {
                    string obfuscation = "";
                    for (int i = 0; i < partToBeObfuscated.Length - 3; i++) obfuscation += "*";
                    displayCase = string.Format("{0}{1}{2}{3}", displayCase[0], displayCase[1], obfuscation, displayCase.Substring(partToBeObfuscated.Length - 1));
                }
                else if (partToBeObfuscated.Length - 3 == 0)
                {
                    displayCase = string.Format("{0}*{1}", displayCase[0], displayCase.Substring(2));
                }

                return displayCase;
            }
            catch (Exception e)
            {
                Log.Write($"ObfuscateEmail Exception: {e.ToString()}");
                return "error";
            }
        }
    }
}
