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
			TcpListener serverSocket = new TcpListener (IPAddress.Parse ("10.0.0.1"), PORT); //new TcpListener(IPAddress.Parse("10.0.0.1"), PORT);
			TcpClient clientSocket = default(TcpClient);
			serverSocket.Start ();
			Console.WriteLine (" >> Server Started");
			Console.WriteLine (spacer);

			var thread = new Thread (() => AsyncStream (serverSocket, clientSocket));
			thread.Start();

			Console.WriteLine ("Press s to stop server, this will stop the server immediately");
			Console.WriteLine (spacer);
			ConsoleKeyInfo input = Console.ReadKey(true);

			while (input.KeyChar != 's') 
			{
				input = Console.ReadKey(true);
			}
			
			Console.WriteLine (" >> Stopping server...");

			if (clientSocket != null && clientSocket.Connected)
				clientSocket.Close ();
			serverSocket.Stop ();
			Console.WriteLine (" >> exit");
			thread.Join ();
		}

		private static void AsyncStream (TcpListener serverSocket, TcpClient clientSocket)
		{
			try {
				while (true){
					// Wait for client to connect
					// while (!serverSocket.Pending ()); // Used for debugging
					clientSocket = serverSocket.AcceptTcpClient ();
					clientSocket.ReceiveBufferSize = 2240;
					Console.WriteLine (" >> Connected to " + clientSocket.Client.RemoteEndPoint.ToString ());

					// Read path for file to send back
					NetworkStream networkStream = clientSocket.GetStream ();

					var path = ReceiveFilePath(networkStream);

					if (!File.Exists (path))
					{
						FileDoesNotExistReponse(networkStream);
						clientSocket.Close ();
						continue;	// Goes to start of while loop
					}


					using (FileStream fs = new FileStream (path, FileMode.Open)) {

						// For testing purposes
						//using (var fs = new FileStream(@"c:\temp\onegigabyte.bin", FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
						//	fs.SetLength(1024*1024*1024); // Equal to about 1 GB

						SendFile(networkStream, fs);

						clientSocket.Close ();
					}

				} 
			}
			catch (Exception ex) 
			{
				Console.WriteLine (ex.ToString()); // For debugging
				if (clientSocket != null && clientSocket.Connected)
					clientSocket.Close ();
				
			}
		}

		static void SendFile(NetworkStream networkStream, FileStream fs)
		{
			SendSizeToClient(networkStream, fs);

			// Send file in chunks
			Console.WriteLine (" >> Sending file...");

			byte[] sendBytes = new byte[BUFSIZE];
			int count;

			while ((count = fs.Read (sendBytes, 0, BUFSIZE)) > 0) {
				networkStream.Write (sendBytes, 0, count);
			}

			//networkStream.Flush(); // As of right now, this does nothing on a NetworkStream

			Console.WriteLine (" >> Send complete");
			Console.WriteLine (spacer);
		}

		static string ReceiveFilePath(NetworkStream networkStream)
		{
			byte[] bytesFrom = new byte[BUFSIZE];

			networkStream.Read (bytesFrom, 0, BUFSIZE);
			string dataFromClient = System.Text.Encoding.ASCII.GetString (bytesFrom);
			dataFromClient = dataFromClient.Substring (0, dataFromClient.IndexOf ("$"));
			Console.WriteLine (" >> Data from client - " + dataFromClient);
			return dataFromClient;
		}


		static void FileDoesNotExistReponse(NetworkStream networkStream)
		{
			byte[] lengthBytes = BitConverter.GetBytes ((long)-1);

			networkStream.Write (lengthBytes, 0, lengthBytes.Length);
			Console.WriteLine (" >> Requested file not found");
			Console.WriteLine (spacer);
		}

		static void SendSizeToClient(NetworkStream networkStream, FileStream fs)
		{
			//write length/size of file to client
			long filesize = fs.Length;
			byte[] lengthBytes = BitConverter.GetBytes (filesize);

			networkStream.Write (lengthBytes, 0, lengthBytes.Length);
			Console.WriteLine (" >> Size of file: " + filesize);
		}


		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			// TO DO Your own code
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
