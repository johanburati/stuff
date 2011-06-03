//make csc2.bat getversion.cs /r:com.burati.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using Com.Burati;

public class MyClass {
    public static void Main(string[] args) {

        foreach(string srv in Burati.SrvList(args)) {
            Console.WriteLine("{0},{1} [{2}]",srv,Burati.GetSysDesc(srv),Burati.GetOsVersion(srv));
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
