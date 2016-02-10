using System;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace AudioVR
{
	public class Link
	{
		private static Vector3 forward = Vector3.back;
		private static float moveSpeed = 0;
		private static Thread t;

		public static void startServer() {
			t = new Thread(() => start ());
			t.Start();
			while (!t.IsAlive);	// wait for thread to become active
		}

		public static void stopServer() {
			t.Abort ();
		}

		public static Vector3 getForward() {
			return forward;
		}

		public static float getMove () {
			return moveSpeed;
		}

		static void limit (Vector3 forward)
		{
			forward.x = Math.Min (Math.Max (-1f, forward.x), 1f); 
			forward.z = Math.Min (Math.Max (-1f, forward.z), 1f); 
		}
		
		private static void start() {
			Debug.LogError("starting thread");
			// Data buffer for incoming data.
			byte[] bytes = new Byte[1024];
			
			// Establish the local endpoint for the socket.
			// Dns.GetHostName returns the name of the host running the application.
			IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 6000);
			
			// Create a TCP/IP socket.
			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			// Bind the socket to the local endpoint and 
			// listen for incoming connections.
			try {
				listener.Bind(localEndPoint);
				listener.Listen(10);
				
				// Start listening for connections.
				while (true) {
					Debug.LogError("wait for connection on " + localEndPoint);
					// Program is suspended while waiting for an incoming connection.
					Socket handler = listener.Accept();
					Debug.LogError("connection made");
					//rotationQuaternion = Quaternion.LookRotation(Vector3.forward);
					// An incoming connection needs to be processed.
					while (true) {
						//Debug.LogError("receiving message");
						bytes = new byte[1024];
						int bytesRec = handler.Receive(bytes);
						String str = Encoding.ASCII.GetString(bytes,0,bytesRec);
						String[] values = str.Split('\n');
						Debug.LogError(values);
						forward = new Vector3(Single.Parse(values[0]), Single.Parse(values[1]),
						                                    Single.Parse(values[2]));
						moveSpeed = Single.Parse(values[3]);
						moveSpeed = Math.Max(-1f, Math.Min(moveSpeed, 1f));
						limit(forward);
						Debug.LogError(forward.x + ", " + forward.y  + ", " + forward.z);
					}
					Debug.LogError("connection closed");
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
				}
				
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}			
		}
	}

}

