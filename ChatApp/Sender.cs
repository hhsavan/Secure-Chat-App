using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    internal class Sender
    {
        Keys keys = new Keys();
        Manager manager { get; set; }
        public Sender(Manager _manager)
        {
            manager = _manager;
            keys = manager.keys;
        }

        /// <summary>
        /// Sends given package to connected machine
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public async Task Send(Package package)
        {
            try
            {
                TcpClient client = new TcpClient(Manager.partnerIP, Convert.ToInt16(Manager.portNumber)); //8080

                while (!keys.DHKeyAssigned)
                {
                    try
                    {
                        //Thread.Sleep(1000);
                        NetworkStream stream = client.GetStream();
                        //string jsonPackage = JsonConvert.SerializeObject(package);
                        string jsonPackage = JsonConvert.SerializeObject(package, Formatting.Indented, Manager.settings);
                        byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
                        stream.Write(data, 0, data.Length);

                        // If the write is successful, break out of the loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error in 'Send' function: " + ex.ToString());

                        // Wait for a short duration before retrying (you can adjust the delay)
                        await Task.Delay(1000); // 1 second delay
                    }
                }

                // Close the client after successful write or repeated attempts
                client.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in 'Send' function (outer catch block): " + ex.ToString());
            }
        }

        /// <summary>
        /// Sends given package to connected machine
        /// </summary>
        /// <returns></returns>
        public async Task Send()
        {
            try
            {
                TcpClient client = new TcpClient(Manager.partnerIP, Convert.ToInt16(Manager.portNumber)); //8080

                while (!keys.DHKeyAssigned)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();

                        Package package = new Package(keys.ownKeyForDHKeyCreation, PackageContent.DiffiHellmanKey);
                        string jsonPackage = JsonConvert.SerializeObject(package, Formatting.Indented, Manager.settings);

                        //string jsonPackage = JsonConvert.SerializeObject(new Package(keys.ownKeyForDHKeyCreation, PackageContent.DiffiHellmanKey));
                        byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
                        stream.Write(data, 0, data.Length);

                        // If the write is successful, break out of the loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error in 'Send' function: " + ex.ToString());

                        // Wait for a short duration before retrying (you can adjust the delay)
                        await Task.Delay(1000); // 1 second delay
                    }
                }

                // Close the client after successful write or repeated attempts
                client.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in 'Send' function (outer catch block): " + ex.ToString());
            }
        }


    }
}
