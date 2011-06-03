// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;

public class MyClass {
    public static void Main(string[] args) {
        try {
            if((args.Length > 0) && (File.Exists(args[0]))) {
                StreamReader sr = File.OpenText(args[0]);
                string line;
                // if (line=sr.ReadLine() != null) {
                //  Console.WriteLine("rrdtool create test.rrd DS:counter1:GAUGE:900:0:100 RRA:AVERAGE:0.5:1:9000");
                // }
                while((line=sr.ReadLine())!=null) {
                    string[] fields = line.Replace("\"","").Split(new char[] {','});
                    Console.WriteLine("rrdtool update test.rrd {0}:{1}",timetoepoch(fields[0]), fields[1]);
                }
                //  Console.WriteLine("rrdtool graph counter1.png --start 1184600945 --end 1184760246 -u 100 -l 0 DEF:counter1=test.rrd:counter1:AVERAGE LINE2:counter1#FF0000");
                sr.Close();
            } else {
                Console.WriteLine("Usage: epoch filename.csv");
            }
        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
    }

    private static string timetoepoch(string strDateTime) {
        System.DateTime localDateTime;
        int epoch;

        try {
            localDateTime = System.DateTime.Parse(strDateTime);
            epoch = (int)(localDateTime - new DateTime(1970, 1, 1)).TotalSeconds;
            epoch -= 32400; // Tokyo time GTM+9)
        } catch {
            epoch = 0;
        }

        return Convert.ToString(epoch);
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