using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Beerlibrary;
using Newtonsoft.Json;

namespace EchoServer
{
    class Server
    {
        static List<Beer> beerlist = new List<Beer>
        {
            new Beer {Id = 1, Name = "Tuborg", Pris = 35, Abv = 5},
            new Beer {Id = 2, Name = "Carlsberg", Pris = 29, Abv = 7},
            new Beer {Id = 3, Name = "Grimberg", Pris = 30, Abv = 6},
        };
        public static void AddBeer(Beer beer)
        {
            beerlist.Add(beer);
        }
        private static object GetById(int id)
        {
            return beerlist.Find(x => x.Id == id);
        }
        public static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            TcpListener serverSocket = new TcpListener(ip, port: 4646);
            serverSocket.Start();
            Console.WriteLine(value: "Server Started");

            TcpClient connectSocket = serverSocket.AcceptTcpClient();

            Console.WriteLine(value: "Server Activated");

            Stream ns = connectSocket.GetStream();

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;

            string message = sr.ReadLine();
            string answer = "";
            int live = 1;
            while (live == 1)
            {
                Console.ReadKey();
                message = sr.ReadLine();
                if (message == "GetAll")
                {
                    foreach(Beer beer in beerlist)
                    {
                        Console.WriteLine($"ID {beer.Id}: {beer.Name}, Pris{beer.Pris}, Abv {beer.Abv}");
                    }
                }
                else if (message == "Gem")
                {
                    string jsonString = sr.ReadLine();
                    Beer beer = JsonConvert.DeserializeObject<Beer>(jsonString);
                    AddBeer(beer);
                    Console.WriteLine("Your beer has been saved");
                }
                else if (message == "ID")
                {
                    int id = Int32.Parse(sr.ReadLine());
                    answer = GetById(id).ToString();
                    Console.WriteLine(answer);
                    sw.WriteLine(answer);
                }
                else if (message == "Stop")
                {
                    Console.WriteLine("stopping program now");
                    live = 0;
                }
                else 
                {
                    Console.WriteLine("Please write one of these 3 options  Getall : Gem : ID: Stop");
                }
            }

          

            Console.WriteLine(value: "client: " + message);
            answer = message.ToUpper();
            sw.WriteLine(answer);
            message = sr.ReadLine();

            ns.Close();
            connectSocket.Close();
            serverSocket.Stop();

        }

       
    }
}