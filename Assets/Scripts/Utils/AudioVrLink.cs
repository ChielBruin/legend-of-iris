using System;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

using Message = System.Collections.Generic.KeyValuePair<float, string>;

namespace AudioVR
{
    public class Link {
        private static Vector3 forward = Vector3.back;
        private static float moveSpeed = 0;
        private static Socket socket = null;
        private static Thread t;
        private static ConversationPlayer notificationPlayer = new ConversationPlayer(new Conversation());
        private static List<Message> notifications = new List<Message>();

        /// <summary>
        /// Starts the server that communicates wih the AudioVRLink-app
        /// </summary>
		public static void startServer() {
            t = new Thread(() => start());
            t.Start();
            while (!t.IsAlive) ;    // wait for thread to become active
        }

        /// <summary>
        /// Closes the socket and stops the thread.
        /// </summary>
        public static void stopServer() {
            if (socket != null) socket.Close();
            t.Abort();
        }

        /// <summary>
        /// Returns the forward direction vector.
        /// </summary>
        public static Vector3 getForward() {
            return forward;
        }

        /// <summary>
        /// Returns the relative movement speed, where positive resembles forward and negative backwards.
        /// </summary>
        public static float getMove() {
            return moveSpeed;
        }

        /// <summary>
        /// Starts the connection with the AudioVRLink-app and parses the incoming data.
        /// </summary>
        private static void start() {
            Debug.Log("starting the AudioVRLink connection..");

            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 6000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

           try { 
                // Bind the socket to the local endpoint and listen for incoming connections.
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true) {
                    notifyUser(20f, "Please connect the AudioVRLink-app.\nConnect code: " + getChecksum(localEndPoint.ToString()));
                    Socket handler = listener.Accept();
                    notifyUser(2f, "Connection established");
                    socket = handler;
                    handler.ReceiveTimeout = 1000;
                    while (true) {
                        try {
                            bytes = new byte[1024];
                            //if (!handler.Connected) break;
                            int bytesRec = handler.Receive(bytes);
                            String str = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            String[] values = str.Split('\n');
                            forward = new Vector3(Single.Parse(values[0]), 0f, Single.Parse(values[2]));
                            moveSpeed = Single.Parse(values[3]);
                            moveSpeed = Math.Max(-1f, Math.Min(moveSpeed, 1f));     // limit
                            VectorExtension.limit(forward, -1f, 1f);
                        } catch (Exception e) {  //Timeout
                            handler.Close();
                           notifyUser(1f, "connection closed");

                            // reset values
                            socket = null;
                            moveSpeed = 0f;
                            forward = Vector3.forward;

                            break;
                        }
                    }
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Notify the user by showing a subtitle element with specified timing and message.
        /// </summary>
        private static void notifyUser(float time, string msg) {
            notifications.Add(new Message(time, msg));
        }

        /// <summary>
        /// Calculate the checksum of the ip address.
        /// </summary>
        private static string getChecksum(string adress) {
            string[] parts = adress.Split('.');
            int sum = Int32.Parse(parts[0]) + Int32.Parse(parts[1]) * 3 + Int32.Parse(parts[2]);
            string res = parts[3].Split(':')[0];
            while(res.Length < 3) {
                res = "0" + res;
            }
            res += sum % 10;
            return res;
        }

        /// <summary>
        /// Checks if the given socket is connected.
        /// </summary>
        private static bool isConnected(Socket socket) {
            try {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            } catch (SocketException) { return false; }
        }

        /// <summary>
        ///  Checks if a connection is made with the AudioVRLink-app.
        /// </summary>
        public static bool isConnected() {
            if (socket == null) return false;
            return isConnected(socket);
        }

        /// <summary>
        /// Called once per frame.
        /// Shows all buffered notifications to the player
        /// Note: These messages are not spoken with sound as the connection code needs to be enterd by a sighted person.
        /// </summary>
        public static void Update() {
            foreach(Message msg in notifications) {
                notificationPlayer.addSubtitle(SubtitlesManager.ShowSubtitle(msg.Key, "AudioVRLink", msg.Value));
            }
            notifications.Clear();
        }
    }
}

