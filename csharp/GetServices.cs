//make csc2.bat getsrvices.cs
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;

public class MyClass {
    public static void Main(string[] args) {
        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");

            if(File.Exists(args[0])) {

                StreamReader sr = File.OpenText(args[0]);
                string hostname;
                while((hostname=sr.ReadLine())!=null) {
                    ListServices(hostname);
                }
                sr.Close();

            } else {
                for(int i=0; i<args.Length; i++) {
                    ListServices(args[i]);
                }
            }


        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
        //RL();
    }

    private static void ListServices(string hostname) {
        try {
            System.ServiceProcess.ServiceController[] services;
            services = System.ServiceProcess.ServiceController.GetServices(hostname);
            for (int i = 0; i < services.Length; i++) {
                Console.WriteLine("{0},{1},{2}",services[i].DisplayName,services[i].ServiceName,services[i].Status));

            }
        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
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
