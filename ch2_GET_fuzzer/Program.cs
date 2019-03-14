using System;
using System.Net;
using System.IO;

namespace initial_fuzzer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Takes a url in from the command line
            // usage: ./fuzzer.exe "http://www.example.com/page.php?arg1=thing&arg2=things"
            string url = args[0];
            // Sets index to the position of ? in the url in order to skip past that url portion
            int index = url.IndexOf("?");
            // Breaks the portion of the URL past the ? into parameters by splitting on &
            string[] parms = url.Remove(0, index + 1).Split('&');
            // Cycle through parameters
            foreach (string parm in parms)
            {
                Console.WriteLine("[+] Crafting SQLi and XSS test strings...");
                // Append parameter with XSS and SQLi test strings
                string xssUrl = url.Replace(parm, parm + "fd<xss>sa");
                string sqlUrl = url.Replace(parm, parm + "fd'sa");

                Console.WriteLine("[+] Sending SQLi on param " + parm);
                // Create the web request for the SQL test, using the payload created above
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sqlUrl);
                request.Method = "GET";
                // Collect the HTTP response in sqlresp
                string sqlresp = string.Empty;
                Console.WriteLine("[+] Collecting SQLi test response...");
                using (StreamReader rdr = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    sqlresp = rdr.ReadToEnd();
                }
                // Test sqlresp to see if an SQL error appears in it
                Console.WriteLine("[+] Checking for SQLi in param " + parm);
                if (sqlresp.Contains("error in your SQL syntax"))
                {
                    Console.WriteLine("[!!!!] Possible SQL Injection point found in parameter: " + parm);
                }

                Console.WriteLine("[+] Sending XSS on param " + parm);
                // Create the web request for the XSS test, using the payload created above
                request = (HttpWebRequest)WebRequest.Create(xssUrl);
                request.Method = "GET";
                // Collect the HTTP response in xssresp
                string xssresp = string.Empty;
                Console.WriteLine("[+] Collecting XSS test response...");
                using (StreamReader rdr = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    xssresp = rdr.ReadToEnd();
                }
                // Test xssresp to see if the <xss> text appears in it
                Console.WriteLine("[+] Checking for XSS on param " + parm);
                if (xssresp.Contains("<xss>"))
                {
                    Console.WriteLine("[!!!!] Possible XSS point found in parameter: " + parm);
                }
                Console.WriteLine("\n");

            }
        }
    }
}
