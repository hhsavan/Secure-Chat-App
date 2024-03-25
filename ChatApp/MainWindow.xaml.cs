using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ChatApp.ECC;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.IO.Packaging;
using ChatApp;
using System.Windows.Input;
using System.Drawing;
namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string parnerIP = "172.20.10.11";
        public ObservableCollection<Message> Messages { get; set; }
        private Timer timer;
        int i = 0;
        //Diffie-Hellman Keys
        private long publicServerDHKey = 10;  //burası random prime number olacak
        private long privateDHKey = 2;         //burası random prime number olacak


        private long DHKey = 0; // final DHKey. privateDHKey1 * privateDHKey * publicServerDHKey
                                // Dh key public keyleri karşı makineye ulaştırırken kullanılacak key

        private long ownPublicKey = 20; //DH Key ile şifrelenerek karşı makineye gönderilecek. Her oturumda random oluşturulacak
        private long ownPrivateKey = 30; //kimseyle paylaşılmaz

        private long partnerPublicKey = -1;  //Diffie Helman ile gelecek

        private bool keyExchanged = false;

        private bool isSecured = false;


        private bool isDragging = false;
        private System.Windows.Point startPoint;
        private enum PackageContent
        {
            DiffiHellmanKey,
            PartnerPublicKey,
            StringMessage
        }

        private struct Package
        {
            public string messageString;
            public PackageContent msgContent;
            public bool isSecured = false;

            public Package()
            {
                messageString = string.Empty;
                msgContent = PackageContent.StringMessage;
            }
            public Package(string message, PackageContent msgP = PackageContent.StringMessage)
            {
                messageString = message;
                msgContent = msgP;
            }
        }

        private Package startingPackage = new Package(string.Empty, PackageContent.DiffiHellmanKey);   //mesaj gönderilme sürecine geçene kadar bu gönderilir.
        Manager manager { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            partnerName.Text = parnerIP;
            Messages = new ObservableCollection<Message>();
            lstMessages.ItemsSource = Messages;
            manager = new Manager(Dispatcher, Messages);



            manager.Start();
            Task.Run(() => OnlineOrOfline());


        }

        private async void OnlineOrOfline()
        {
            while (true)
            {
                if (this.Dispatcher.Invoke(() => manager.online)) break;
            }
            //this.onileneOrOffline.Fill = Brushes.Green;
            this.Dispatcher.Invoke(() => this.onileneOrOffline.Fill = Brushes.Green);

        }

        //TODO Gönderilen mesajın sol altında karşıya ulaşıp ulaşmadığını belirten ufak bir yazı olacak
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string messageContent = txtMessage.Text;
            Package newPackage = new Package();
            newPackage.messageString = messageContent;

            if (!string.IsNullOrEmpty(messageContent))
            {
                Task.Run(() => SendMessage(newPackage));

                Messages.Add(new Message { Sender = "You", Content = messageContent });
                txtMessage.Clear();
            }
        }

        //public class Message
        //{
        //    public string Sender { get; set; }
        //    public string Content { get; set; }
        //}



        /// <summary>
        /// mesaj gönderir
        /// </summary>
        /// <param name="message"></param>
        /// <param name="initializeFlag">Bu flag true gelirse diffie-hellman uygulanır</param>
        private async Task SendMessage(Package package)
        {
            try
            {
                
                package.messageString = Manager.EncryptMessage(package.messageString, manager.mySharedSecret);
                TcpClient client = new TcpClient(parnerIP, 8080);
                NetworkStream stream = client.GetStream();
                //onileneOrOffline.Fill = Brushes.Green;
                string jsonPackage = JsonConvert.SerializeObject(package);
                byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
                stream.Write(data, 0, data.Length);

                // İstemciyi kapat
                client.Close();
            }
            catch (Exception ex)
            {
                // Hata yönetimi burada yapılabilir
                //MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                //Dispatcher.Invoke(() => Messages.Add(new Message { Sender = "INFO", Content = "Partner offline" }));
                onileneOrOffline.Fill = Brushes.Red;
                //SendMessage(message, initializeFlag);
            }
        }





        private async Task SendMessage()
        {
            try
            {
                TcpClient client = new TcpClient(parnerIP, 8080);
                NetworkStream stream = client.GetStream();
                //if(startingPackage.msgContent == PackageContent.PartnerPublicKey)   startingPackage.isSecured = true;
                string jsonPackage = JsonConvert.SerializeObject(startingPackage);
                byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
                await stream.WriteAsync(data, 0, data.Length);

                // Close the client
                client.Close();
            }
            catch (Exception ex)
            {
                // Handle the error here
                //onileneOrOffline.Fill = Brushes.Red;
            }
        }






        #region other methods


        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = true;
                startPoint = e.GetPosition(this);
            }
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                System.Windows.Point currentPosition = e.GetPosition(this);
                double deltaX = currentPosition.X - startPoint.X;
                double deltaY = currentPosition.Y - startPoint.Y;

                this.Left += deltaX;
                this.Top += deltaY;

                startPoint = currentPosition;
            }
        }

        private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = false;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }


        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Enter tuşuna basıldığında mesajı gönder
                this.SendButton.Click += new System.Windows.RoutedEventHandler(this.btnSend_Click);

                // Tuş olayını işle ve işlenmiş olarak işaretle
                e.Handled = true;
            }
        }

        private void txtMessage_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Maximize(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        //private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Left)
        //        DragMove();
        //}

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }
        #endregion
    }
}