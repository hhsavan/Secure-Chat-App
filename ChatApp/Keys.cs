using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    internal class Keys
    {
        public Keys() {
            privateKey = "8";
            publicKey = "25";
            ownKeyForDHKeyCreation = "8";
        }
        public string privateKey { get; set; }
        public string publicKey { get; set; }
        public string partnerPublicKey { get; set; }  //partner Public Key
        public string DHKey { get; set; }
        public string ownKeyForDHKeyCreation { get; set; }

        public const string commonKeyForDHKeyCreation = "11";

        // bu ikisi ilk obje oluşturuluken zaten create edilecğeinden bunların boollarını tutmaya gerek yok
        //public bool privateKeyAssigned { get; set; }
        //public bool publicKeyAssigned { get; set; }
        //public bool ownKeyForDHKeyCreationAssigned { get; set; }

        public bool partnerPublicKeyAssigned { get; set; }  //partner Public Key
        public bool DHKeyAssigned { get; set; }

    }
}
