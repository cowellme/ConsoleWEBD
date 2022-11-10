using System;

namespace ConsoleWEBD
{
    public class Logger
    {
        public static void Error(Exception ex)
        {
            string time = DateTime.Now.ToString("hh-mm dd.MM.yyyy");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message + time);
            Console.ResetColor();
        }
        public static void Info(string txt)
        {
            string time = DateTime.Now.ToString("hh-mm dd.MM.yyyy");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(txt + time);
            Console.ResetColor();
        }
    }
}
