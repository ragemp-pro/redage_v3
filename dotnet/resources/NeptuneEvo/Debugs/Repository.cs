using System;
using Redage.SDK;

namespace NeptuneEvo.Debugs
{
    public class Repository
    {
        private static readonly nLog Log = new nLog("Debugs.Repository");
        public static void Exception(Exception e)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH':'mm':'ss.fff")} | Exception: {e.ToString()}");
        }
    }
}