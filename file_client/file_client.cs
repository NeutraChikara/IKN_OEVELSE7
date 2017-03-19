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


		public static bool messageSent = false;
		string spacer = "--------------------";
		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		private file_client (string[] args)
		{

			// Sets up connection and calls receiveFile

			UdpClient clientSocket = new UdpClient (PORT);
			IPEndPoint iep = new IPEndPoint (IPAddress.Parse("10.0.0.2"), PORT);

			while (true) 
			{
				try
				{
					// Setting up connection and calling receiveFile

					SendChar(args[1],clientSocket,ref iep);
					string output = Recieve(clientSocket);
					char cmd = args[1][args[1].Length-1];
					if( (cmd == 'L') || (cmd == 'l'))
						PrintLoadavg(output);
					else if( (cmd == 'U') || (cmd == 'u'))
						PrintUpstream(output);
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
		private void SendChar(string charToSend,UdpClient udpclient,ref IPEndPoint iep)
		{
			

			byte[] sendBytes = Encoding.ASCII.GetBytes(charToSend);
			udpclient.Send (sendBytes, sendBytes.Length, iep);

	
		}
		private string Recieve(UdpClient udpclient)
		{
			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte [] receiveBytes = udpclient.Receive(ref RemoteIpEndPoint);
			string upstream = Encoding.ASCII.GetString (receiveBytes);
			return upstream;
		}

		private void PrintUpstream(string upstream)
		{
			string[] command = upstream.Split (new char[] {' '},  2);

			Console.WriteLine (spacer);
			Console.Write("Uptime for server: ");
			Console.WriteLine(upstream.Split(' ')[0] + " sec");
			Console.Write("Idle time for server: ");
			Console.WriteLine(command [1].TrimEnd (new char[] {'\n'}) + " sec");
			Console.WriteLine (spacer);
		}

		private void PrintLoadavg(string loadavg)
		{
			string[] spaceSplit = loadavg.Split (new char[] {' '}, 5); //splits the string up in 5 pieces seperated by whitespace.
			string[] crossSplit = spaceSplit[3].Split(new char[] {'/'},2);

			Console.WriteLine (spacer);
			Console.WriteLine ("CPU and I/O utilization in different intervals");
			Console.WriteLine ("1 minute: " +spaceSplit [0]);
			Console.WriteLine ("5 minutes: " + spaceSplit [1]);
			Console.WriteLine ("15 minutes: " + spaceSplit [2]);
			Console.WriteLine ("number of currently executing entites: " + crossSplit[0]);
			Console.WriteLine ("number of scheduling entities: " + crossSplit[1]);
			Console.Write ("Last process ID: " + spaceSplit [4]);
			Console.WriteLine (spacer);
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
