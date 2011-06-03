//make csc2.bat genperfmon.cs
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Text;

public class MyClass {



    public static void Main(string[] args) {
        try {
            if (args.Length == 0) throw new ArgumentException("Usage: perfmon serverslist.txt");

            if(File.Exists(args[0])) {
                GenPerfmon(args[0]);
            } else throw new ArgumentException("Usage: perfmon serverslist.txt");
        } catch(Exception exc) {
            Console.WriteLine("{0}", exc.Message);
        }
        //RL();
    }
    private static void GenPerfmon(string fileName) {
        string[] header = {
            "<HTML>\r\n",
            "<HEAD>\r\n",
            "<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html;\" />\r\n",
            "<META NAME=\"GENERATOR\" Content=\"Microsoft System Monitor\" />\r\n",
            "</HEAD>\r\n",
            "<BODY>\r\n",
            "<OBJECT ID=\"DISystemMonitor1\" WIDTH=\"100%\" HEIGHT=\"100%\"\r\n",
            "CLASSID=\"CLSID:C4D2D8E0-D1DD-11CE-940F-008029004347\">\r\n",
            "       <PARAM NAME=\"_Version\" VALUE=\"196611\"/>\r\n",
            "       <PARAM NAME=\"LogName\" VALUE=\"[TEMPLATE]\"/>\r\n",
            "       <PARAM NAME=\"Comment\" VALUE=\"\"/>\r\n",
            "       <PARAM NAME=\"LogType\" VALUE=\"0\"/>\r\n",
            "       <PARAM NAME=\"CurrentState\" VALUE=\"1\"/>\r\n",
            "       <PARAM NAME=\"RealTimeDataSource\" VALUE=\"1\"/>\r\n",
            "       <PARAM NAME=\"LogFileMaxSize\" VALUE=\"5120\"/>\r\n",
            "       <PARAM NAME=\"DataStoreAttributes\" VALUE=\"33\"/>\r\n",
            "       <PARAM NAME=\"LogFileBaseName\" VALUE=\"[TEMPLATE]\"/>\r\n",
            "       <PARAM NAME=\"LogFileSerialNumber\" VALUE=\"1\"/>\r\n",
            "       <PARAM NAME=\"LogFileFolder\" VALUE=\"D:\\PerfLogs\"/>\r\n",
            "       <PARAM NAME=\"Sql Log Base Name\" VALUE=\"SQL:![TEMPLATE]\"/>\r\n",
            "       <PARAM NAME=\"LogFileAutoFormat\" VALUE=\"-1\"/>\r\n",
            "       <PARAM NAME=\"LogFileType\" VALUE=\"0\"/>\r\n",
            "       <PARAM NAME=\"StartMode\" VALUE=\"0\"/>\r\n",
            "       <PARAM NAME=\"StopMode\" VALUE=\"0\"/>\r\n",
            "       <PARAM NAME=\"RestartMode\" VALUE=\"0\"/>\r\n",
            "       <PARAM NAME=\"LogFileName\" VALUE=\"D:\\PerfLogs\\[TEMPLATE].csv\"/>\r\n",
            "       <PARAM NAME=\"EOFCommandFile\" VALUE=\"\"/>\r\n"
        };

        string[] footer = {
            "\t<PARAM NAME=\"UpdateInterval\" VALUE=\"900\">\r\n",
            "\t<PARAM NAME=\"SampleIntervalUnitType\" VALUE=\"2\">\r\n",
            "\t<PARAM NAME=\"SampleIntervalValue\" VALUE=\"15\">\r\n",
            "</OBJECT>\r\n"
        };

        string [] monitorlistdc = {
            @"LogicalDisk(0/C:)\% Free Space",
            @"LogicalDisk(0/C:)\Free Megabytes",
            @"LogicalDisk(1/L:)\% Free Space",
            @"LogicalDisk(1/L:)\Free Megabytes",
            @"LogicalDisk(2/N:)\% Free Space",
            @"LogicalDisk(2/N:)\Free Megabyte",
            @"Memory\Available MByte",
            @"Memory\Pages/sec",
            @"NTDS\DRA Inbound Bytes Not Compressed (Within Site)/sec",
            @"NTDS\DRA Inbound Bytes Total/sec",
            @"NTDS\DRA Outbound Bytes Not Compressed (Within Site)/sec",
            @"NTDS\DRA Outbound Bytes Total/sec",
            @"Processor(_Total)\% Processor Time"
        };

        string[] monitorlist = {
            @"LogicalDisk(C:)\% Free Space",
            @"LogicalDisk(C:)\Free Megabytes",
            @"LogicalDisk(D:)\% Free Space",
            @"LogicalDisk(D:)\Free Megabytes",
            @"Memory\% Committed Bytes In Use",
            @"Memory\Available MBytes",
            @"Network Interface(public)\Current Bandwidth",
            @"Paging File(_Total)\% Usage",
            @"Processor(_Total)\% Processor Time"
        };
        int count = 1;
        try {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Join("",header).Replace("[TEMPLATE]",fileName.Replace(".txt","")));
            StreamReader sr=File.OpenText(fileName);
            string server;
            while ((server= sr.ReadLine()) != null)
                foreach (string monitor in monitorlistdc)
                    sb.Append(String.Format("\t<PARAM NAME=\"Counter{0,5:00000}.Path\" VALUE=\"\\\\{1}\\{2}\"/>\r\n", count++, server, monitor));
            sb.Append(String.Format("\t<PARAM NAME=\"CounterCount\" VALUE=\"{0}\" >\r\n", count));
            sb.Append(String.Join("",footer));
            Console.Write(sb);
        } catch (Exception exc) {
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
