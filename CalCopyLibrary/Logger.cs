using System;

namespace CalCopyLibrary
{
    public static class Logger
    {
        public static bool TimeStamp = true;
        public static void Log(string msg)
        {
            if (TimeStamp)
            {
                Console.WriteLine(String.Format("[{0}] {1}", DateTime.Now.ToLocalTime(), msg));
            }
            else 
            {
                Console.WriteLine(msg);
            }
        }
    }
}