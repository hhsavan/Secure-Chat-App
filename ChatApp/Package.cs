using ChatApp.ECC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static ChatApp.Manager;

namespace ChatApp
{
    internal class Package
    {

        public string messageString;
        public PackageContent msgContent;
        public bool isSecured = false;
        public Cryptography.KeyPair publicKey;//karşı tarafa publicKey gönderirken bu doldurulup gönderilir 
        public Package()
        {
            messageString = string.Empty;
            msgContent = PackageContent.StringMessage;
        }
        public Package(string message, PackageContent msgP = PackageContent.StringMessage)
        {
            messageString = message;
            //publicKey = null;
            msgContent = msgP;
        }
        public Package(string message, Cryptography.KeyPair p, PackageContent msgP = PackageContent.StringMessage)
        {
            messageString = message;
            publicKey = p;
            msgContent = msgP;
        }
    }
}
