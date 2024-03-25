using ChatApp.ECC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static ChatApp.ECC.Cryptography;
//using static ChatApp.MainWindow;

namespace ChatApp
{
    public enum PackageContent
    {
        DiffiHellmanKey,
        PartnerPublicKey,
        StringMessage
    }

    internal class Manager
    {
        public static string partnerIP = "172.20.10.11";
        public static string portNumber = "8080";
        public bool online = false;

        /// <summary>
        /// Şifreleme için gerekli bilgiler
        /// </summary>
        public Keys keys;
        public Message messages;
        public Dispatcher dispatcher;
        public ObservableCollection<Message> MessageCollection { get; set; }
        public Sender messageSender;
        public Reciever messageReciever;

        public Curve curve;
        public Cryptography.KeyPair keyPair;   //My keypair
        public Cryptography.KeyPair partnerKeyPair; //only has publicKey

        public static JsonSerializerSettings settings = new JsonSerializerSettings();
        public ECC.Point mySharedSecret;

        public Manager(Dispatcher disp, ObservableCollection<Message> _messageCollection)
        {
            try
            {
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                MessageCollection = _messageCollection;
                keys = new Keys();
                dispatcher = disp;
                //EccTest();
                curve = new Curve(Curve.CurveName.secp256k1);   //Bitcoinde kullanılan grafik
                keyPair = Cryptography.GetKeyPair(curve);

                messageReciever = new Reciever(this);
                messageSender = new Sender(this);
            }
            catch (Exception ec)
            {

            }
        }

        /// <summary>
        /// Dinleme ve gönderme işlemlerini başlatır
        /// </summary>
        public void Start()
        {
            Task.Run(() => messageReciever.ListenForMessages());
            Task.Run(() => messageSender.Send());

        }




        //private void EccTest()
        //{
        //    //Debug.ForegroundColor = ConsoleColor.Yellow;
        //    //Debug.WriteLine("ECDH TEST");
        //    //Console.ForegroundColor = ConsoleColor.White;
        //    try
        //    {
        //        DateTime start = DateTime.Now;

        //        Curve curve = new Curve(Curve.CurveName.secp256k1);

        //        Cryptography.KeyPair aliceKeyPair = Cryptography.GetKeyPair(curve);
        //        Cryptography.KeyPair bobKeyPair = Cryptography.GetKeyPair(curve);

        //        ECC.Point aliceSharedSecret = Cryptography.GetSharedSecret(aliceKeyPair.PrivateKey, bobKeyPair.PublicKey);
        //        ECC.Point bobSharedSecret = Cryptography.GetSharedSecret(bobKeyPair.PrivateKey, aliceKeyPair.PublicKey);

        //        DateTime finish = DateTime.Now;

        //        string message = "Merhaba";
        //        string encryptedMessage = EncryptMessage(message, aliceSharedSecret);
        //        string decryptedMessage = DecryptMessage(encryptedMessage, bobSharedSecret);


        //        PrintValue($"{Curve.CurveName.secp256k1.ToString()} passed", string.Concat(aliceSharedSecret.X == bobSharedSecret.X && aliceSharedSecret.Y == bobSharedSecret.Y, " (", (int)(finish - start).TotalMilliseconds, " ms)"));
        //    }
        //    catch (Exception exception)
        //    {
        //        PrintValue($"{Curve.CurveName.secp256k1.ToString()} passed", string.Concat("Error (", exception.Message, ")"));
        //    }

        //}

        public static string EncryptMessage(string message, ECC.Point sharedSecret)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] sharedSecretBytes = sharedSecret.X.ToByteArray();

            byte[] encryptedBytes = new byte[messageBytes.Length];

            for (int i = 0; i < messageBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(messageBytes[i] ^ sharedSecretBytes[i % sharedSecretBytes.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptMessage(string encryptedMessage, ECC.Point sharedSecret)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
            byte[] sharedSecretBytes = sharedSecret.X.ToByteArray();

            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ sharedSecretBytes[i % sharedSecretBytes.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        static void PrintValue(string valueName, object value)
        {
            Debug.WriteLine(string.Concat(valueName, ": ", value));
        }





        void printKeys()
        {
            Debug.WriteLine("-----------------------KeysGüncel-------------------------");
            Debug.WriteLine($"privateKey: {keyPair.PublicKey}");
            Debug.WriteLine($"publicKey: {keys.publicKey}");
            Debug.WriteLine($"ownDhkey: {keys.ownKeyForDHKeyCreation}");
            Debug.WriteLine($"Dhkey: {keys.DHKey}");
            Debug.WriteLine($"is Dhkey Assigned: {keys.DHKeyAssigned}");
            Debug.WriteLine($"partnerPublicKey: {keys.partnerPublicKey}");
            Debug.WriteLine($"is partnerPublicKey Assigned: {keys.partnerPublicKeyAssigned}");
            Debug.WriteLine("----------------------------------------------------------");



        }





        public void SendMessage()
        {
            //if(signal == Signal.DiffiHellmanKey) { }
            //Task.Run(() => client.SendMessage());

        }


        //TODO burası gerçek bir symmetric şifreleme yapılacak. şimdilik çarpma şeklinde
        /// <summary>
        /// Verilen Keyi SecretKey Kullanarak şifreler
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public long SymmetricEncryption(long SecretKey, long Key)
        {
            return SecretKey * Key;
        }
        //TODO burası gerçek bir symmetric şifreleme yapılacak. şimdilik çarpma şeklinde
        /// <summary>
        /// Verilen Keyi SecretKey Kullanarak şifreler
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public Cryptography.KeyPair SymmetricEncryption(Cryptography.KeyPair keyp, long dhKey)
        {
            try
            {
                keyp.PublicKey.X *= dhKey;
                keyp.PublicKey.Y *= dhKey;
                return keyp;
            }
            catch (Exception ec)
            {
                return keyp;
            }
        }
        //TODO burası gerçek bir symmetric şifreleme yapılacak. şimdilik çarpma şeklinde
        /// <summary>
        /// Verilen Keyi SecretKey Kullanarak şifreler
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public Cryptography.KeyPair SymmetricDecryption(Cryptography.KeyPair keyp, long dhKey)
        {
            try
            {
                keyp.PublicKey.X /= dhKey;
                keyp.PublicKey.Y /= dhKey;
                return keyp;
            }
            catch (Exception ec)
            {

                return keyp;
            }
        }


        /// <summary>
        /// Verilen Keyin şifresini SecretKey Kullanarak çözer
        /// </summary>
        /// <param name="SecretKey"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public long SymmetricDecryption(long SecretKey, long Key)   //burda secretKey daha büyükmolursa sıkıntı
        {
            if (SecretKey > Key)    return SecretKey / Key; 
                                    return Key / SecretKey;
        }


        //TODO bura long döndürecek
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GenerateRandomLongNum()
        {
            Random random = new Random();
            return random.Next(10, 100);
        }

    }
}
