using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace tcp
{
	class file_client
	{
		/// <summary>
		/// The PORT.
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;	// Change ReceiveBufferSize of TcpListener accordingly if changed

		public static bool messageSent = false;
		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		private file_client (string[] args)
		{
			string spacer = "--------------------";
			// Sets up connection and calls receiveFile

			UdpClient clientSocket = new UdpClient();
			clientSocket.Client.Connect(args [0], PORT);
			while (true) 
			{
				try
				{
					// Setting up connection and calling receiveFile

					SendChar(args[1],clientSocket);
					break;
				}
				catch (Exception ex)
				{
					// If connection failed
					Console.WriteLine (ex.ToString ());
					Console.WriteLine (spacer);
					Console.WriteLine ("Could not connect to IP adress\nPress t to try again, e to exit");
					ConsoleKeyInfo input = Console.ReadKey(true);

					while (input.KeyChar != 't' && input.KeyChar != 'e') 
					{
						Console.WriteLine ("Invalid input, try again");
						input = Console.ReadKey(true);
					}

					if (input.KeyChar == 't')
						continue;
					else
						break;
				}
				
			}
		}
		public static void SendCallback(IAsyncResult ar)
		{
			UdpClient u = (UdpClient)ar.AsyncState;

			Console.WriteLine("number of bytes sent: {0}", u.EndSend(ar));
			messageSent = true;
		}
		private void SendChar(string charToSend,UdpClient udpclient)
		{
			

			byte[] sendBytes = Encoding.ASCII.GetBytes(charToSend);

			udpclient.BeginSend (sendBytes, sendBytes.Length,new AsyncCallback(SendCallback),udpclient);
			while (!messageSent) {
				Thread.Sleep (100);
			}
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");

			if (args.Length != 2) {
			
				Console.Write ("Enter file path: ");
				string arg1 = Console.ReadLine ();

				string[] arg = { "10.0.0.2", arg1 };
				new file_client (arg);

			}
			else
				new file_client(args);
		}
	}
}
