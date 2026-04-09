using System;
using System.Collections.Generic;
using System.Text;

namespace SH.DbRepositoryTests
{
    public static class Utils
    {
        public static void WriteLineForColor(string text, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteForColor(string text, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteLineForRed(string text) => WriteLineForColor(text, ConsoleColor.Red);
        public static void WriteLineForGreen(string text) => WriteLineForColor(text, ConsoleColor.Green);
        public static void WriteLineForYellow(string text) => WriteLineForColor(text, ConsoleColor.Yellow);
        public static void WriteLineForBlue(string text) => WriteLineForColor(text, ConsoleColor.Blue);
        public static void WriteLineForCyan(string text) => WriteLineForColor(text, ConsoleColor.Cyan);
        public static void WriteLineForMagenta(string text) => WriteLineForColor(text, ConsoleColor.Magenta);
        public static void WriteForRed(string text) => WriteForColor(text, ConsoleColor.Red);
        public static void WriteForGreen(string text) => WriteForColor(text, ConsoleColor.Green);
        public static void WriteForYellow(string text) => WriteForColor(text, ConsoleColor.Yellow);
        public static void WriteForBlue(string text) => WriteForColor(text, ConsoleColor.Blue);
        public static void WriteForCyan(string text) => WriteForColor(text, ConsoleColor.Cyan);
        public static void WriteForMagenta(string text) => WriteForColor(text, ConsoleColor.Magenta);


    }
}
