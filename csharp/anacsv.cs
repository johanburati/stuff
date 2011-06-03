// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using  System.Collections;
using System.Configuration;

public class MyClass {
    public static void Main(string[] args) {

        Boolean isdebug = false;

        ArrayList DS = new ArrayList();
        ArrayList Max = new ArrayList();

        if (isdebug) Console.WriteLine("-- begin --");
        try {
            if((args.Length > 0) && (File.Exists(args[0]))) {
                if (isdebug) Console.WriteLine("-- reading file --");
                StreamReader sr = File.OpenText(args[0]);
                string fn = System.IO.Path.GetFileNameWithoutExtension(args[0]);
                string line;

                // --- read first line of CSV file ---
                if ((line=sr.ReadLine())!=null) {

                    string[] columns = line.Replace("\"","").Split(new char[] {','});
                    for(int i=0; i<columns.Length; i++) {
                        string Counter = Regex.Match(columns[i],@"\\\\(?<server>\w+)\\").Groups["server"].ToString() + "_" + i.ToString("00");

                        if (Counter.StartsWith("_")) {
                            Counter = Counter.Replace("_","");
                        }
                        //if (!Counter.StartsWith("_")) {
                        DS.Add(Counter);
                        Max.Add(0);
                        //}
                    }
                }

                if (isdebug) Console.WriteLine("-- reading data --");
                // --- read data line ---
                while ((line=sr.ReadLine())!=null) {

                    ArrayList Row = new ArrayList();

                    string[] columns = line.Replace("\"","").Split(new char[] {','});

                    for(int i=1; i<columns.Length-1; i++) {

                        //if (isdebug) Console.Write("({0})", columns[i]);

                        uint value1 = 0;


                        if (Convert.ToString(columns[i]).Replace(" ", "") != "") value1 = (uint) Convert.ToSingle(columns[i]);
                        if (isdebug) Console.Write("{0}/", value1);


                        uint value2 = (uint) Convert.ToSingle(Max[i]);

                        if (value1 > value2) Max[i] = columns[i];

                    }
                    if (isdebug) Console.WriteLine("");
                }



                StringBuilder uplimit = new StringBuilder();
                uplimit.Append(String.Format("<add key=\"{0}\" value=\"", fn));
                for(int i=0; i<Max.Count-1; i++) {
                    uint value = (uint) Convert.ToSingle(Max[i]);
                    uplimit.Append(value);
                    if (i != Max.Count-2) uplimit.Append(",");
                }
                uplimit.Append("\" />\r\n");
                Console.Write(uplimit);


            } else {
                Console.WriteLine("Usage: readcsv filename.csv");
            }
        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
    }

    public static void PrintValues( IEnumerable myList )  {
        foreach ( Object obj in myList ) Console.WriteLine( "   {0}", obj );
        Console.WriteLine();
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