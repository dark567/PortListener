using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleServer
{
    class Program
    {
        //const int port = 5000;
        static TcpListener listener;

        public static int port;

        static void Main(string[] args)
        {
            // Get normal filepath of this assembly's permanent directory
            var path = Environment.CurrentDirectory;

            #region read ini
            try
            {
                //Создание объекта, для работы с файлом
                INIManager manager = new INIManager(path + @"\set.ini");
                //Получить значение по ключу name из секции main
                port = int.Parse(manager.GetPrivateString("workstation", "Port"));

                Console.WriteLine("Port - " + port);

                File.AppendAllText(path + @"\appEx.log", "путь к db:" + port + "\n");
                //Записать значение по ключу age в секции main
                // manager.WritePrivateString("main", "age", "21");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ini не прочтен" + ex.Message);
            }
            #endregion

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
}
