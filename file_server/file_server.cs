using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tcp
{
	class file_server
	{
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;
		// Change ReceiveBufferSize of TcpListener accordingly if BUFSIZE is changed

		static string spacer = "--------------------";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
		/// </summary>
		private file_server ()
		{
			


			Console.WriteLine (" >> Server Started");
			Console.WriteLine (spacer);



			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, PORT);
			UdpClient udpClient = new UdpClient (PORT);
			while (true) {
				byte[] receiveByes = udpClient.Receive (ref RemoteIpEndPoint);


				char returnData = Encoding.ASCII.GetString (receiveByes)[0];
				switch (returnData) {
				case 'l':
				case 'L':
					Console.WriteLine ("Sending /proc/loadavg..");
					SendLoadAvg (udpClient);

					break;
				case 'u':
				case 'U':
					Console.WriteLine ("Sending /proc/uptime..");
					SendUptime (udpClient);
					break;
				}
					
			}
			Console.WriteLine ("Press s to stop server, this will stop the server immediately");
			Console.WriteLine (spacer);
			ConsoleKeyInfo input = Console.ReadKey(true);

			while (input.KeyChar != 's') 
			{
				input = Console.ReadKey(true);
			}
			
			Console.WriteLine (" >> Stopping server...");

		/*	if (clientSocket != null && clientSocket.Connected)
				clientSocket.Close (); */
			Console.WriteLine (" >> exit");




		}

		private void SendLoadAvg(UdpClient udpclient)
		{
			string loadAvg = File.ReadAllText ("/proc/loadavg");
			Console.WriteLine (loadAvg);
			IPEndPoint iep = new IPEndPoint (IPAddress.Parse ("10.0.0.1"), PORT);
			byte[] sendBytes = Encoding.ASCII.GetBytes(loadAvg);
			udpclient.Send (sendBytes, sendBytes.Length, iep);
		}
			
		private void SendUptime (UdpClient udpclient)
		{
			//udpclient.Connect ("10.0.0.1", PORT);
			string uptime = File.ReadAllText ("/proc/uptime");
			Console.WriteLine (uptime); 
			IPEndPoint iep = new IPEndPoint (IPAddress.Parse ("10.0.0.1"), PORT);
			byte[] sendBytes = Encoding.ASCII.GetBytes(uptime);
			udpclient.Send (sendBytes, sendBytes.Length, iep);
		}

	

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new file_server ();
		}
	}
}
