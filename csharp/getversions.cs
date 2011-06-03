//make csc2.bat getversions.cs /r:com.burati.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using com.burati;

public class MyClass {
    public static void Main(string[] args) {
        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");
            string community = burati.getReadCommunity();

            if(File.Exists(args[0])) {

                StreamReader sr = File.OpenText(args[0]);
                string hostname;
                while((hostname=sr.ReadLine())!=null) {
                    Console.WriteLine("{0},{1}",hostname,burati.getSysDescrShort(hostname,community));
                }
                sr.Close();

            } else {
                for(int i=0; i<args.Length; i++) {
                    Console.WriteLine("{0},{1}",args[i],burati.getSysDescrShort(args[i],community));
                }
            }


        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
        //RL();
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
