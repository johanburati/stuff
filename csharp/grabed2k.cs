// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Text.RegularExpressions;


class Parse {
    static void Main(string[] args) {
        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify a file to search.");
            if(File.Exists(args[0])) {
                StreamReader sr = File.OpenText(args[0]);
                string line;
                while((line=sr.ReadLine())!=null) {
                    // string ed2klink = Regex.Match(line, @"([a-zA-Z]{2}[0-9]{5})").Groups[1].ToString();
                    string ed2klink = Regex.Match(line, @"(ed2k://.*?/)").Groups[1].ToString();
                    // cleanup link
                    ed2klink = Regex.Replace(ed2klink,@".\[sharethefiles.com\]","");
                    if (ed2klink != "") {
                        Console.WriteLine(ed2klink);
                    }
                }
                sr.Close();
            }
        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }

    }
}