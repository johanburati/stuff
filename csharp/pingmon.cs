//make csc2.bat pingmon.cs /r:com.burati.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Threading;
using Com.Burati;

public class MyClass {
    public static void Main(string[] args) {

        ConsoleKeyInfo cki = new ConsoleKeyInfo();

        do {
            while (Console.KeyAvailable == false) {
                MainLoop(args);
                Console.WriteLine("\nPress the 'x' key to quit.");
                Thread.Sleep(60000);
            }
            cki = Console.ReadKey(true);
        } while(cki.Key != ConsoleKey.X);
        //RL();
    }

    private static void MainLoop(string[] args) {
        const string logfile = "pingmon.txt";

        foreach(string srv in Burati.SrvList(args)) {
            if(!Burati.IsAlive(srv)) {
                // server is down
                Console.Write('x');
                string msg = String.Format("{0}: {1} is down.", GetNow(),srv);
                AppendToFile(logfile, msg);
            } else {
                // server is up
                Console.Write('.');
            }
        }
    }

    private static string GetNow() {
        DateTime dt = DateTime.Now;
        return(String.Format("{0,4:00}/{1,2:00}/{2,2:00} {3,2:00}:{4,2:00}", dt.Year, dt.Month, dt.Day,dt.Hour,dt.Minute));
    }
    static void AppendToFile(string file, string text) {
        StreamWriter SW;
        SW=File.AppendText(file);
        SW.WriteLine(text);
        SW.Close();

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
