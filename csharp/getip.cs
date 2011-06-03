// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

public class MyClass {
    public static void Main(string[] args) {
        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");

            if(File.Exists(args[0])) {
                StreamReader sr = File.OpenText(args[0]);
                string hostname;
                while((hostname=sr.ReadLine())!=null) {
                    Console.WriteLine("{0}\t[{1}]", hostname,getip(hostname));
                }
                sr.Close();
            } else {
                for(int i=0; i<args.Length; i++)
                    Console.WriteLine("{0}\t[{1}]", args[i],getip(args[i]));
            }



        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
        //RL();
    }
    private static string getip(string hostname) {
        string ipaddress = "";

        IPHostEntry host = Dns.GetHostEntry(hostname);
        ipaddress = Convert.ToString(host.AddressList[0]);
        return(ipaddress);

    }

    #region Helper methods

    private static void WL(object text, params object[] args) {
        Console.WriteLine(text.ToString(), args);
    }

    private static void RL() {
        Console.ReadLine();
    }

    private static void Break() {
        System.Diagnostics.Debugger.Break();
    }

    #endregion
}