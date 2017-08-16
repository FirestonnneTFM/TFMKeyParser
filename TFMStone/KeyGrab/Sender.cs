using System;
using System.Net.Sockets;
using System.IO;

namespace TFMStone.KeyGrab
{
    internal static class Sender
    {
        public static void SendKeys(string host)
        {
            byte[] buf = null;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter w = new BinaryWriter(ms))
            {
                w.Write(TFMKeys.HashKey);
                w.Write(TFMKeys.LoginKey);
                w.Write((uint)TFMKeys.HandshakeNumber);
                w.Write(TFMKeys.HandshakeString);
                buf = ms.ToArray();
            }
            try
            {
                TcpClient client = new TcpClient(host, 7777);
                using (BinaryWriter w = new BinaryWriter(client.GetStream()))
                {
                    w.Write(buf.Length);
                    w.Write(buf);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
