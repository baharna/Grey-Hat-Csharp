using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace post_fuzzer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string[] requestLines = File.ReadAllLines(args[0]);
            string[] parms = requestLines[requestLines.Length - 1].Split('&');
            string host = string.Empty;
            StringBuilder requestBuilder = new StringBuilder();
            int count = 0;
            foreach (string ln in requestLines)
            {
                if (ln.StartsWith("Host:"))
                    host = ln.Split(' ')[1].Replace("\r", string.Empty);
                if (count == (requestLines.Length - 2))
                    requestBuilder.Append(ln + "\r\n\r\n");
                else
                    requestBuilder.Append(ln + "\n");
                count += 1;
            }

            string request = requestBuilder.ToString();

            IPEndPoint rhost = new IPEndPoint(IPAddress.Parse(host), 80);
            foreach (string parm in parms)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                sock.Connect(rhost);

                string val = parm.Split('=')[1];
                string req = request.Replace("=" + val, "=" + val + "'");

                byte[] reqBytes = Encoding.ASCII.GetBytes(req);
                sock.Send(reqBytes);

                string response = string.Empty;
                byte[] buf = new byte[sock.ReceiveBufferSize];

                sock.Receive(buf);
                response = Encoding.ASCII.GetString(buf);
                if (response.Contains("error in your SQL syntax"))
                    Console.WriteLine("Parameter " + parm + " seems vulnerable to SQL injection with value: " + val + "'\n");
                sock.Close();


            }
        }
    }
}
