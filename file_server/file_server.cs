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




			UdpClient udpClient = new UdpClient (PORT);
			while (true) {
				
				string returnData = ReceiveCmd (udpClient);
				IPEndPoint iep = new IPEndPoint(IPAddress.Parse(returnData.Split(' ')[0]),PORT);

				switch (returnData[returnData.Length-1] ) {
				case 'l':
				case 'L':
					Console.WriteLine ("Sending /proc/loadavg..");
					SendLoadAvg (udpClient,ref iep);

					break;
				case 'u':
				case 'U':
					Console.WriteLine ("Sending /proc/uptime..");
					SendUptime (udpClient, ref iep);
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

		private string ReceiveCmd(UdpClient udpclient)
		{
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, PORT);

			byte[] receiveByes = udpclient.Receive (ref RemoteIpEndPoint);
			return Encoding.ASCII.GetString(receiveByes);
		}

		private void SendLoadAvg(UdpClient udpclient,ref IPEndPoint iep)
		{
			string loadAvg = File.ReadAllText ("/proc/loadavg");
			Console.WriteLine (loadAvg);

			byte[] sendBytes = Encoding.ASCII.GetBytes(loadAvg);
			udpclient.Send (sendBytes, sendBytes.Length, iep);
		}
			
		private void SendUptime (UdpClient udpclient, ref IPEndPoint iep)
		{
			string uptime = File.ReadAllText ("/proc/uptime");
			Console.WriteLine (uptime); 

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
