using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

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
			TcpClient clientSocket = new TcpClient();
			clientSocket.ReceiveBufferSize = 2240;

			while (true) 
			{
				try
				{
					// Setting up connection and calling receiveFile
					clientSocket.Connect (args [0], PORT);
					NetworkStream serverStream = clientSocket.GetStream ();
					receiveFile (args [1], serverStream);
					clientSocket.Close ();
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

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		private void receiveFile (String fileName, NetworkStream io)
		{
			fileName = fileName + "$";

			// Send text with path and name of file
			byte[] outStream = System.Text.Encoding.ASCII.GetBytes(fileName);
			io.Write(outStream, 0, outStream.Length);
			// io.Flush();		// As of right now, this does nothing on a NetworkStream

			// Receive size of file
			byte[] msgLengthBytes = new byte[sizeof(long)];	// Or hardcode size of a double in bytes manually
			io.Read(msgLengthBytes, 0, msgLengthBytes.Length);
			long msgLength = BitConverter.ToInt64(msgLengthBytes, 0);

			// If file was not found
			if (msgLength == -1)
			{
				Console.WriteLine ("ERROR: File was not found on server");
				return;
			}
				
			// Commence download
			Console.WriteLine("Downloading...");

			byte[] inStream = new byte[BUFSIZE];
			long totalReceived = 0;

			// Setup FileStream which writes to desktop
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + LIB.extractFileName(fileName);
			FileStream fs = new FileStream (path, FileMode.Create);

			// Download file chunk by chunk and write it to FileStream path
			do
			{
				int count = io.Read (inStream, 0, BUFSIZE);
			

				fs.Write(inStream, 0, count);
				totalReceived += count;

				string diff = totalReceived + "/" + msgLength;

				Console.Write("\r{0}%   " + diff + " bytes", String.Format("{0:0}", 100*totalReceived/msgLength));
			}
			while (totalReceived != msgLength);

			Console.WriteLine ("\nDownload complete");
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
			string[] s = { "10.0.0.1", "/root/Desktop/file" };

			new file_client(s);
		}
	}
}
