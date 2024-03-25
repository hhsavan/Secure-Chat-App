using ChatApp.ECC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
//using static ChatApp.Manager;
//using message;
namespace ChatApp
{
    internal class Reciever
    {
        Mutex mutex = new Mutex();
        Keys keys;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1); // 1 permits, initial count 1

        //Message messages 
        public Reciever(Manager manager)
        {
            this.manager = manager;
            keys = manager.keys;

        }
        private Manager manager { get; set; }
        private bool isDhChecked = false;



        /// <summary>
        /// Decodes the messages: DhKey - PublicKey - Message             
        /// </summary>
        /// <param name="messagePackage"></param>
        /// <returns></returns>
        public async Task MessageDecoder(Package messagePackage)
        {
            try
            {
                await semaphore.WaitAsync();
                Thread.Sleep(1000);
                if (!manager.keys.DHKeyAssigned && messagePackage.msgContent == PackageContent.DiffiHellmanKey) // gelen mesaj dh key. onun içini doldur
                {
                    manager.keys.DHKey = (Convert.ToInt64(messagePackage.messageString)
                        * Convert.ToInt64(manager.keys.ownKeyForDHKeyCreation) 
                        * Convert.ToInt64(Keys.commonKeyForDHKeyCreation)).ToString();
                    isDhChecked = true;
                    // DHKey alındı şimdi karşı tarafa publicKey'imi gönderebilirim
                    
                    Package tempPackage = new Package(string.Empty,manager.SymmetricEncryption(manager.keyPair, Convert.ToInt64(keys.DHKey))
                        ,PackageContent.PartnerPublicKey);

                    await Task.Run(() => manager.messageSender.Send(tempPackage));

                    //await Task.Run(() => manager.messageSender.Send(new Package(manager.SymmetricEncryption(Convert.ToInt64(keys.DHKey),
                    //    Convert.ToInt64(keys.publicKey)).ToString(), PackageContent.PartnerPublicKey)));
                    //printKeys();

                }
                else if (!manager.keys.partnerPublicKeyAssigned && messagePackage.msgContent == PackageContent.PartnerPublicKey && isDhChecked) // gelen mesaj publicKey. partnerPublicKey Doldur
                {
                    manager.partnerKeyPair = manager.SymmetricDecryption(messagePackage.publicKey, Convert.ToInt64(manager.keys.DHKey));
                    //manager.keys.partnerPublicKey = manager.SymmetricDecryption(Convert.ToInt64(messagePackage.messageString)
                    //    , Convert.ToInt64(manager.keys.DHKey)).ToString();
                    manager.keys.partnerPublicKeyAssigned = true;
                    manager.keys.DHKeyAssigned = true;
                    printKeys();
                    await UpdateScreen("Secured!!","System_Info");
                    manager.online = true;
                    manager.mySharedSecret = Cryptography.GetSharedSecret(manager.keyPair.PrivateKey, manager.partnerKeyPair.PublicKey);
                }
                else if (messagePackage.msgContent == PackageContent.StringMessage)   // Gelen normal mesaj ekrana yazdır
                {
                    messagePackage.messageString = Manager.DecryptMessage(messagePackage.messageString, manager.mySharedSecret);
                    await UpdateScreen(messagePackage);
                }
                
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception on MessageDecoder: " + exc.ToString());
            }
            finally { semaphore.Release(); }
        }

        void printKeys()
        {
            Debug.WriteLine("-----------------------KeysGüncel-------------------------");
            Debug.WriteLine($"privateKey: {keys.privateKey}");
            Debug.WriteLine($"publicKey: {keys.publicKey}");
            Debug.WriteLine($"ownDhkey: {keys.ownKeyForDHKeyCreation}");
            Debug.WriteLine($"Dhkey: {keys.DHKey}");
            Debug.WriteLine($"is Dhkey Assigned: {keys.DHKeyAssigned}");
            Debug.WriteLine($"partnerPublicKey: {keys.partnerPublicKey}");
            Debug.WriteLine($"is partnerPublicKey Assigned: {keys.partnerPublicKeyAssigned}");
            Debug.WriteLine("----------------------------------------------------------");



        }
        /// <summary>
        /// Prints the messages to screen
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public async Task UpdateScreen(Package package)
        {
            manager.dispatcher.Invoke(() => manager.MessageCollection.Add(new Message { Sender = "Partner", Content = package.messageString }));
        }
        /// <summary>
        ///  Prints the messages to screen
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task UpdateScreen(string message, string sender = "Partner")
        {
            manager.dispatcher.Invoke(() => manager.MessageCollection.Add(new Message { Sender = sender, Content = message }));
        }

        /// <summary>
        /// Listens messages and sends the MessageDecoder
        /// </summary>
        /// <returns></returns>
        public async Task ListenForMessages()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8080);

            try
            {
                server.Start();

                while (true)
                {
                    try
                    {
                        TcpClient client = await server.AcceptTcpClientAsync();
                        NetworkStream stream = client.GetStream();

                        byte[] data = new byte[2048];
                        int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                        string jsonMessage = Encoding.UTF8.GetString(data, 0, bytesRead);
                        Package jsonPackage;
                        if (jsonMessage != null)
                        {
                            jsonPackage = JsonConvert.DeserializeObject<Package>(jsonMessage);
                            Task.Run(() => MessageDecoder(jsonPackage));
                            // Rest of your code
                        }
                        else Debug.WriteLine("jsonPackage is null");
                    }
                    catch (Exception ec)
                    {
                        Debug.WriteLine($"Hata oluştu: {ec.Message}");
                    }


                }
            }
            catch (Exception ec)
            {
                Debug.WriteLine($"Hata oluştu: {ec.Message}");
            }
            finally
            {
                // Ensure the server is stopped when the loop exits
                server.Stop();
            }
        }



    }
}
