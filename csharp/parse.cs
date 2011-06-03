// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Text.RegularExpressions;


class Parse {
    static void Main(string[] args) {
        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify a text file.");
            if(File.Exists(args[1])) {
                StreamReader sr = File.OpenText(args[1]);
                string line;
                while((line=sr.ReadLine())!=null) {
                    // string soeid = Regex.Match(line, @"([a-zA-Z]{2}[0-9]{5})").Groups[1].ToString();
                    string sub= Regex.Match(line, @args[0]).Groups[1].ToString();
                    if (sub != "") {
                        Console.WriteLine(sub);
                    }
                }
                sr.Close();
            }
        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }

    }
}