using System;

namespace ConsoleWEBD
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                string test = "12345 ";
                
                if (LastSymbol(test))
                {
                    Logger.Info("Space ");
                }
                else
                {
                    Logger.Info("Pere ");
                }
                TelegramDriver.Start();
                Logger.Info("Bot started, all good ");
                Logger.Info("Get Task ");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static bool LastSymbol(string str)
        {
            int i = str.Length;

            string buffer = str.Remove(i - 1);

            str = str.Replace(buffer, "");

            if (str == " ")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

    }
}
