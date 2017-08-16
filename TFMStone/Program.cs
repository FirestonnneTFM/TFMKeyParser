#define HANDLE

using System;

using TFMStone.KeyGrab;

namespace TFMStone
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Which host to send data to?");
                return;
            }
            if (FetchKeys())
            {
                Sender.SendKeys(args[0]);
            }
        }

        private static bool FetchKeys()
        {
#if HANDLE
            try
            {
#endif
            TFMKeys.GetKeys();
            Console.WriteLine("Handshake: {0:x04} + {1}", TFMKeys.HandshakeNumber, TFMKeys.HandshakeString);
            Console.WriteLine("Login key: {0:x08}", TFMKeys.LoginKey);
            Console.Write("Hash key: ");
            foreach (byte b in TFMKeys.HashKey)
                Console.Write("{0:x02} ", b);
            Console.WriteLine();
#if HANDLE
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
                return false;
            }
#endif
            return true;
        }
    }
}
