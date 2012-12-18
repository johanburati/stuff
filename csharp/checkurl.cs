// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
// fetch the wssdb via HTTP

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

class WebFetch {
    static void Main(string[] args) {
        const string BASE_URL = "";

        if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");

        try {
            // Create the web request
            HttpWebRequest request = WebRequest.Create(BASE_URL+args[0]) as HttpWebRequest;

            // Add authentication to request
            //request.Credentials = new NetworkCredential("user", "password");
            request.Credentials =	CredentialCache.DefaultCredentials;

            // Get response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
                // Get the response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output
                Console.WriteLine(StripHTML(reader.ReadToEnd()));
            }
        } catch (Exception ex) {
            Console.WriteLine("{0}",ex.Message);
        }



    }

    static string StripHTML (string inputString) {
        //const string HTML_TAG_PATTERN = "<.*?>";


        //Replace newline with exclamation mark
        inputString = inputString.Replace(System.Environment.NewLine,"!");

        inputString = Regex.Replace(inputString,"close window|-->|<.*?>",String.Empty);
        //inputString = Regex.Replace(inputString,HTML_TAG_PATTERN,String.Empty);

        //Replace multiple exclamation mark with newline
        inputString = Regex.Replace(inputString,"!+",System.Environment.NewLine);
        //To get question and answer on the same line
        inputString = inputString.Replace("\x3a\x20\x0d\x0a","\x3a\x20");
        return inputString;
    }

}
