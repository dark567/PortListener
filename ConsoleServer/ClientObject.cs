using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ConsoleServer
{
    class ClientObject
    {
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    Console.WriteLine(message);
                    // отправляем обратно сообщение в верхнем регистре
                    string mes = message.Substring(message.IndexOf(':') + 1).Trim();
                    if (!string.IsNullOrEmpty(mes))
                    {
                        message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
                    }
                    //message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();

                    File.AppendAllText(Environment.CurrentDirectory + @"\data.log", $"{DateTime.Now}:" + message + "\n");

                    data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
