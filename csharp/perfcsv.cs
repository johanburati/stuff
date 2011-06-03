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

        // Boolean isdebug = false;

        ArrayList DS = new ArrayList();
        ArrayList Desc = new ArrayList();

        try {
            if((args.Length > 0) && (File.Exists(args[0]))) {
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
                        Desc.Add(columns[i]);
                        //}
                    }

                    StringBuilder cmd = new StringBuilder();
                    cmd.Append(String.Format("rrdtool create -b1167663600 {0}.rrd ", fn));
                    for(int i=1; i<DS.Count-1; i++) {
                        cmd.Append(String.Format("DS:{0}:GAUGE:1000:U:U ", DS[i]));
                        //Console.WriteLine( "*   {0}: {1}", DS[i], Desc[i] );
                    }

                    cmd.Append("RRA:AVERAGE:0.5:1:96 RRA:AVERAGE:0.5:4:168 RRA:AVERAGE:0.5:96:1176 RRA:AVERAGE:0.5:2976:14112 ");

                    Console.WriteLine(cmd);
                }

                // --- read data line ---
                while ((line=sr.ReadLine())!=null) {

                    ArrayList Row = new ArrayList();
                    string[] columns = line.Replace("\"","").Split(new char[] {','});
                    for(int i=0; i<columns.Length; i++) {
                        Row.Add(columns[i]);
                    }

                    StringBuilder cmd = new StringBuilder();
                    //if (isdebug) Console.WriteLine("Time: {0}", time);
                    cmd.Append(String.Format("rrdtool update {0}.rrd {1}", fn,timetoepoch(Convert.ToString(Row[0]))));


                    for(int i=1; i<DS.Count-1; i++) {
                        string value = Regex.Match(Convert.ToString(Row[i]),@"^(?<value>\d+)").Groups["value"].ToString();
                        cmd.Append(String.Format(":{0}", value));
                    }
                    Console.WriteLine(cmd);
                }


                {
                    // Update the graphs
                    string config = ConfigurationManager.AppSettings[fn];
                    string[] max = config.Split(',');
                    string[] period = { "daily", "weekly", "monthly" , "yearly" };
                    string[] periodv = { "172800", "691200", "2764800" , "33177600" };
                    for(int x=0; x < period.Length; x++) {


                        string opt = String.Format("-s-{0} -e-86400 -l0 -M -E",periodv[x]);
                        // one month : -s-2764800
                        // one year  : -s-33177600

                        for(int i=1; i<DS.Count-1; i++) {
                            StringBuilder cmd = new StringBuilder();
                            //cmd.Append("rrdtool graph -s1207121856 -e1207726656 -t\"CPU Utilization\" -h60 -w250 -l0 -u100  -M -E");

                            cmd.Append(String.Format("rrdtool graph {1}_{5}.png -t\"{3}\" -u{4} {0} DEF:{1}={2}.rrd:{1}:AVERAGE  LINE1:{1}#FF0000", opt, DS[i], fn, Desc[i], max[i],period[x]));
                            Console.WriteLine(cmd);
                        }
                    }

                    for(int x=0; x < period.Length; x++) {
                        StringBuilder html = new StringBuilder();
                        html.Append("<html><head></head><body>\r\n");
                        for(int i=1; i<DS.Count-1; i++) {
                            //html.Append(String.Format("<h5>{0}</h5>\r\n", Desc[i]));
                            html.Append(String.Format("<img src=\"{0}_{1}.png\">\r\n",DS[i],period[x]));
                        }
                        html.Append("</body></html>\r\n");

                        using (StreamWriter sw = new StreamWriter(String.Format("{0}_{1}.htm",fn, period[x]))) {
                            sw.Write(html);
                        }
                    }

                }

                sr.Close();
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