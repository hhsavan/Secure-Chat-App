using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace ChatApp
{
    internal class Temp
    {
        Keys keys = new Keys();
        Manager manager { get; set; }

        //public static TaskCompletionSource<bool> dhkeySentCompletion = new TaskCompletionSource<bool>();
        //public static TaskCompletionSource<bool> publicKeySentCompletion = new TaskCompletionSource<bool>();
        //public static TaskCompletionSource<bool> dhkeyReceivedCompletion = new TaskCompletionSource<bool>();
        //public static TaskCompletionSource<bool> publicKeyReceivedCompletion = new TaskCompletionSource<bool>();
        public static bool dhKeyTaken = false;
        public static bool partnerKeyTaken = false;

        static int dhKey = 13;
        static int partnerKey = 0;
        public Temp(Manager _manager)
        {
            manager = _manager;
            keys = manager.keys;
        }

        public static bool GetKeyBool(int key)
        {
            if (key == 1) return dhKeyTaken;
            return partnerKeyTaken;
        }
        public static int GetKey(int key)
        {
            if (key == 1) return dhKey;
            return partnerKey;
        }
        public static async Task Send(int key = 1)
        {
            try
            {
                TcpClient client = null;
                bool connected = false;

                while (!connected)
                {
                    try
                    {
                        client = new TcpClient(Manager.partnerIP, Convert.ToInt16(Manager.portNumber));
                        connected = true; // Set the flag to true if connection is successful
                    }
                    catch (Exception)
                    {
                        // Connection failed, wait for a while before retrying
                        await Task.Delay(10); // You can adjust the delay as needed
                    }
                }

                // The rest of your code to send the message goes here
                while (!GetKeyBool(key))
                {
                    NetworkStream stream = client.GetStream();

                    string jsonPackage = JsonConvert.SerializeObject($"Sent Message: {key}");
                    byte[] data = Encoding.UTF8.GetBytes(jsonPackage);
                    await stream.WriteAsync(data, 0, data.Length);
                }

                // Close the client after sending the message
                client.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in 'Send()' function: " + ex.ToString());
            }
        }


        public async Task ListenForMessages()
        {
            UdpClient udpListener = new UdpClient(8080); // Replace with the appropriate port number

            try
            {
                while (true)
                {
                    UdpReceiveResult result = await udpListener.ReceiveAsync();
                    byte[] data = result.Buffer;

                    string jsonMessage = Encoding.UTF8.GetString(data);

                    Package jsonPackage;
                    try
                    {
                        jsonPackage = JsonConvert.DeserializeObject<Package>(jsonMessage);

                        // Process the received message
                        //await Task.Run(() => MessageDecoder(jsonPackage));

                        // Send acknowledgment back to the sender
                        string acknowledgmentMessage = "ACK";
                        byte[] acknowledgmentData = Encoding.UTF8.GetBytes(acknowledgmentMessage);
                        await udpListener.SendAsync(acknowledgmentData, acknowledgmentData.Length, result.RemoteEndPoint);
                    }
                    catch (JsonException ex)
                    {
                        Debug.WriteLine("Error deserializing JSON: " + ex.Message);
                    }
                }
            }
            catch (Exception ec)
            {
                Debug.WriteLine($"Error: {ec.Message}");
            }
            finally
            {
                udpListener.Close();
            }
        }
























        public static async Task SendWithAcknowledgmentTcp(string message, string receiverIpAddress, int receiverPort, int retryCount = 3, int retryDelayMs = 1000)
        {
            TcpClient tcpClient = new TcpClient();

            try
            {
                await tcpClient.ConnectAsync(IPAddress.Parse(receiverIpAddress), receiverPort);

                using (NetworkStream stream = tcpClient.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    for (int attempt = 1; attempt <= retryCount; attempt++)
                    {
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();

                        Debug.WriteLine($"Message sent (Attempt {attempt}): {message}. Waiting for acknowledgment...");

                        // Wait for acknowledgment with a timeout (adjust the timeout as needed)
                        var acknowledgmentTask = reader.ReadLineAsync();
                        if (await Task.WhenAny(acknowledgmentTask, Task.Delay(retryDelayMs)) == acknowledgmentTask)
                        {
                            // Acknowledgment received
                            string acknowledgmentMessage = acknowledgmentTask.Result;
                            if (acknowledgmentMessage == "ACK")
                            {
                                Debug.WriteLine("Acknowledgment received successfully.");
                                //dhkeySentCompletion.SetResult(true);
                                return;
                            }
                        }

                        Debug.WriteLine("Acknowledgment not received. Retrying...");
                    }
                }

                Debug.WriteLine($"Failed to receive acknowledgment after {retryCount} attempts.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
            }
        }

        public static async Task ListenWithAcknowledgmentTcp(int listenPort, int acknowledgmentTimeoutMs = 5000)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, listenPort);

            try
            {
                tcpListener.Start();

                while (true)
                {
                    using (TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync())
                    using (NetworkStream stream = tcpClient.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string receivedMessage = await reader.ReadLineAsync();
                        Debug.WriteLine($"Received message: {receivedMessage}");

                        // Process the received message

                        // Send acknowledgment back to the sender
                        await writer.WriteLineAsync("ACK");
                        await writer.FlushAsync();

                        // Wait for acknowledgment with a timeout
                        var acknowledgmentTask = reader.ReadLineAsync();
                        if (await Task.WhenAny(acknowledgmentTask, Task.Delay(acknowledgmentTimeoutMs)) == acknowledgmentTask)
                        {
                            string acknowledgmentMessage = acknowledgmentTask.Result;
                            if (acknowledgmentMessage == "ACK")
                            {
                                Debug.WriteLine("Acknowledgment received successfully.");
                                //dhkeyReceivedCompletion.SetResult(true);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Failed to receive acknowledgment within the timeout.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                tcpListener.Stop();
            }
        }








        public static async Task SendWithAcknowledgment(string message, string receiverIpAddress, int receiverPort, int retryCount = 30, int retryDelayMs = 1000)
        {
            UdpClient udpClient = new UdpClient();

            try
            {
                IPEndPoint receiverEndPoint = new IPEndPoint(IPAddress.Parse(receiverIpAddress), receiverPort);

                byte[] data = Encoding.UTF8.GetBytes(message);

                for (int attempt = 1; attempt <= retryCount; attempt++)
                {
                    await udpClient.SendAsync(data, data.Length, receiverEndPoint);

                    Debug.WriteLine($"Message sent (Attempt {attempt}): {message}. Waiting for acknowledgment...");

                    // Wait for acknowledgment with a timeout (adjust the timeout as needed)
                    var acknowledgmentTask = udpClient.ReceiveAsync();
                    if (await Task.WhenAny(acknowledgmentTask, Task.Delay(retryDelayMs)) == acknowledgmentTask)
                    {
                        // Acknowledgment received
                        string acknowledgmentMessage = Encoding.UTF8.GetString(acknowledgmentTask.Result.Buffer);
                        if (acknowledgmentMessage == "ACK")
                        {
                            Debug.WriteLine("Acknowledgment received successfully.");
                            //dhkeySentCompletion.SetResult(true);
                            return;
                        }
                    }

                    Debug.WriteLine("Acknowledgment not received. Retrying...");
                }

                Debug.WriteLine($"Failed to receive acknowledgment after {retryCount} attempts.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                udpClient.Close();
            }
        }


        public static async Task ListenWithAcknowledgment(int listenPort, int acknowledgmentTimeoutMs = 5000)
        {
            UdpClient udpListener = new UdpClient(listenPort);

            try
            {
                while (true)
                {
                    UdpReceiveResult result = await udpListener.ReceiveAsync();
                    byte[] data = result.Buffer;

                    string receivedMessage = Encoding.UTF8.GetString(data);
                    Debug.WriteLine($"Received message: {receivedMessage}");

                    // Process the received message

                    // Send acknowledgment back to the sender
                    UdpClient acknowledgmentClient = new UdpClient();
                    IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.92"), result.RemoteEndPoint.Port);
                    byte[] acknowledgmentData = Encoding.UTF8.GetBytes("ACK");
                    await acknowledgmentClient.SendAsync(acknowledgmentData, acknowledgmentData.Length, senderEndPoint);

                    // Wait for acknowledgment with a timeout
                    var acknowledgmentTask = acknowledgmentClient.ReceiveAsync();
                    if (await Task.WhenAny(acknowledgmentTask, Task.Delay(acknowledgmentTimeoutMs)) == acknowledgmentTask)
                    {
                        string acknowledgmentMessage = Encoding.UTF8.GetString(acknowledgmentTask.Result.Buffer);
                        if (acknowledgmentMessage == "ACK")
                        {
                            Debug.WriteLine("Acknowledgment received successfully.");
                            //dhkeyReceivedCompletion.SetResult(true);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Failed to receive acknowledgment within the timeout.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                udpListener.Close();
            }
        }

    }
}
