using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChatApp
{
    internal class Client
    {

        
        //private string parnerIP = "172.20.10.11";

        

        ///// <summary>
        ///// mesaj gönderir. tipi her zaman "StringMessage" dir
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="initializeFlag">Bu flag true gelirse diffie-hellman uygulanır</param>
        //public async Task SendMessage(Manager.Package package)
        //{
        //    try
        //    {
        //        TcpClient client = new TcpClient(parnerIP, 8080);
        //        NetworkStream stream = client.GetStream();
                
        //        string jsonPackage = JsonConvert.SerializeObject(package);
        //        byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
        //        stream.Write(data, 0, data.Length);

        //        // İstemciyi kapat
        //        client.Close();
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //}


        ///// <summary>
        ///// Karşının Server'ına signal yollar. bu sinyalin manası "sana bunu yollayacağım" demek
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="initializeFlag">Bu flag true gelirse diffie-hellman uygulanır</param>
        //private async Task SendSignal(Manager.Signal signal)
        //{
        //    try
        //    { 
        //        TcpClient client = new TcpClient(parnerIP, 8080);
        //        NetworkStream stream = client.GetStream();

        //        string jsonPackage = JsonConvert.SerializeObject(signal);
        //        byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
        //        stream.Write(data, 0, data.Length);

        //        // İstemciyi kapat
        //        client.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}


    }
}
