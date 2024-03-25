using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    internal class Server
    {

        


        ///// <summary>
        ///// Bu Thread mesajları dinler
        ///// </summary>
        ///// <returns></returns>
        //public async Task<Manager.Signal> ListenForMessages()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            TcpListener server = new TcpListener(IPAddress.Any, 8080);
        //            server.Start();

        //            TcpClient client = await server.AcceptTcpClientAsync();
        //            NetworkStream stream = client.GetStream();

        //            byte[] data = new byte[256];
        //            int bytesRead = await stream.ReadAsync(data, 0, data.Length);
        //            string jsonMessage = Encoding.UTF8.GetString(data, 0, bytesRead);
        //            server.Stop();

        //            Manager.Package jsonPackage = JsonConvert.DeserializeObject<Manager.Package>(jsonMessage);
        //            return await ControlMessage(jsonPackage);
        //            //return jsonPackage;
        //        }
        //        catch (Exception ec)
        //        {
        //            Debug.WriteLine($"Hata oluştu: {ec.Message}");
        //        }
        //    }
        //}


        ///// <summary>
        ///// Bu mesajı kontrol eder. tipi sinyal mi yoksa mesaj mı diye
        ///// </summary>
        //private async Task<Manager.Signal> ControlMessage(Manager.Package jsonPackage)
        //{
        //    if(jsonPackage.msgContent == Manager.PackageContent.DiffiHellmanKey)
        //    {

        //    }
        //    else if (jsonPackage.msgContent == Manager.PackageContent.PartnerPublicKey)
        //    {

        //    }

        //    return Manager.Signal.StringMessage;
        //}

        
    }
}
