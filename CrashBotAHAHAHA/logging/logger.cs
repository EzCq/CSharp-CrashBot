using System;
using System.Collections.Generic;
using System.Text;

namespace CrashBotAHAHAHA.logging
{
    public class logger
    {
        public static void succesLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }
        public static void errorLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }
    }
}
