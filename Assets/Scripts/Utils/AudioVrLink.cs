using System;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace AudioVR
{
    public class Link {
        private static Vector3 forward = Vector3.back;
        private static float moveSpeed = 0;
        private static Socket socket = null;
        private static Thread t;

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
            Debug.LogError("starting the AudioVRLink connection..");

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
                    Debug.LogError("Waiting for connection on " + localEndPoint);
                    Debug.LogError(getChecksum(localEndPoint.ToString()));
                    Socket handler = listener.Accept();
                    Debug.LogError("Connection made");
                    socket = handler;
                    while (true) {
                        bytes = new byte[1024];
                        //if (!handler.Poll(1000, SelectMode.SelectRead)) break;
                        int bytesRec = handler.Receive(bytes);
                        String str = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        String[] values = str.Split('\n');
                        forward = new Vector3(Single.Parse(values[0]), 0f, Single.Parse(values[2]));
                        moveSpeed = Single.Parse(values[3]);
                        moveSpeed = Math.Max(-1f, Math.Min(moveSpeed, 1f));     // limit
                        VectorExtension.limit(forward, -1f, 1f);
                    }
                    Debug.LogError("connection closed");
                    // reset values
                    socket = null;
                    moveSpeed = 0f;
                    forward = Vector3.forward;
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
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
    }
}

