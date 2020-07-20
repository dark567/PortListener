using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ConsoleClient
{
    class Program
    {
        //const int port = 5000;
        const string address = "127.0.0.1";

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

                File.AppendAllText(path + @"\new_file.txt", "путь к db:" + port + "\n");
                //Записать значение по ключу age в секции main
                // manager.WritePrivateString("main", "age", "21");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ini не прочтен" + ex.Message);
            }
            #endregion

            Console.Write("Введите свое имя:");
            string userName = Console.ReadLine();
            bool pr = false;

            do
            {
                if (!string.IsNullOrEmpty(userName))
                    pr = true;
                else
                {
                    Console.Write("Введите свое имя:");
                    userName = Console.ReadLine();
                }

            } while (!pr);


            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    Console.Write(userName + ": ");
                    // ввод сообщения
                    string message = Console.ReadLine();
                    message = String.Format("{0}: {1}", userName, message);
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    // отправка сообщения
                    stream.Write(data, 0, data.Length);

                    // получаем ответ
                    data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("Сервер: {0}", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }

        }
    }
}
