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



			IPEndPoint e = new IPEndPoint(IPAddress.Any,0);
			UdpClient udpClient = new UdpClient (PORT);

			byte[] receiveByes = udpClient.Receive (ref e);

			string returnData = Encoding.ASCII.GetString (receiveByes);

			Console.WriteLine (returnData.ToString());

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
