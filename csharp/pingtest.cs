//make csc2.bat pingtest.cs /r:com.burati.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.Net.NetworkInformation;
using Com.Burati;

public class PingTest {
    public static void Main(string[] args) {
        foreach(string srv in Burati.SrvList(args)) {
            try {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(srv);
                if (pingReply.Status == IPStatus.Success) {
                    Console.WriteLine("{0}:\tUp ({1}ms)",srv,pingReply.RoundtripTime);
                } else {
                    Console.WriteLine("{0}:\tDown ({1}ms)",srv,pingReply.RoundtripTime);
                }
            } catch (Exception e) {
                Console.WriteLine("{0}:\tCould not find host {0}.",srv, e.Message);
            }
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
