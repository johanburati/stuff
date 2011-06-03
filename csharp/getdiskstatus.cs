//make csc.bat  getdiskstatus.cs /r:snmp.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using Snmp;
using X690;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

public class GetVolStatusClass {
    private static string PROGNAME = "getVolStatus";
    private static string VERSION = "1.0";

    public static void Main(string[] args) {

        int total = 0;
        try {
            string server = "";
            string community = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\SNMPcommunity").GetValue("Read").ToString();

            //bool shortOn = false;

            if (args.Length == 0) {
                usage();
                return;
            }
            foreach ( string option in args )
                switch ( option )  {
                    // case "-short":  case "/short":  case "-s":  case "/s":
                    //  shortOn = true;  break;
                case "-help":
                case "/help":
                case "-h":
                case "/h":
                case "-?":
                case "/?":
                    usage();
                    return;
                default:
                    if (server == "") {
                        server = option;
                    } else {
                        Console.WriteLine("Warning: wrong argument '"+option+"'\n");
                    }
                    break;
                }

            if (server != "") {

                if(File.Exists(server)) {
                    StreamReader sr = File.OpenText(server);
                    while((server=sr.ReadLine())!=null) {
                        total += getDisksStatus(server,community );
                    }
                    sr.Close();
                } else {
                    total += getDisksStatus(server,community );
                }
            }
        } catch ( Exception ex) {
            Console.WriteLine("error,{0}",ex.Message);
        }
        Console.WriteLine("Total,{0}", total);
    }

    #region VolStatus methods
    private static int getDisksStatus(string hostname, string community) {
        try {
            uint[] na_diskTotalCount = new uint[] {1,3,6,1,4,1,789,1,6,4,1,0};
            uint[] na_diskActiveCount  = new uint[] {1,3,6,1,4,1,789,1,6,4,2,0};
            uint[] na_diskFailedCount  = new uint[] {1,3,6,1,4,1,789,1,6,4,7,0};
            uint[] na_diskSpareCount  = new uint[] {1,3,6,1,4,1,789,1,6,4,8,0};
            uint[] na_diskFailedMessage  = new uint[] {1,3,6,1,4,1,789,1,6,4,10,0};

            int diskTotalCount = 0;
            {
                ManagerSession sess = new ManagerSession(hostname,community);
                ManagerItem mi = new ManagerItem(sess,na_diskTotalCount);
                string value   = mi.Value.ToString();
                diskTotalCount = Convert.ToInt32(value);
            }
            int diskActiveCount = 0;
            {
                ManagerSession sess = new ManagerSession(hostname,community);
                ManagerItem mi = new ManagerItem(sess,na_diskActiveCount);
                string value   = mi.Value.ToString();
                diskActiveCount = Convert.ToInt32(value);
            }
            int diskFailedCount = 0;
            {
                ManagerSession sess = new ManagerSession(hostname,community);
                ManagerItem mi = new ManagerItem(sess,na_diskFailedCount);
                string value   = mi.Value.ToString();
                diskFailedCount = Convert.ToInt32(value);
            }
            int diskSpareCount = 0;
            {
                ManagerSession sess = new ManagerSession(hostname,community);
                ManagerItem mi = new ManagerItem(sess,na_diskSpareCount);
                string value   = mi.Value.ToString();
                diskSpareCount = Convert.ToInt32(value);
            }
            string diskFailedMessage = "";
            {
                ManagerSession sess = new ManagerSession(hostname,community);
                ManagerItem mi = new ManagerItem(sess,na_diskFailedMessage);
                string value   = mi.Value.ToString();
                diskFailedMessage = Convert.ToString(value);
            }

            Console.WriteLine("{0},{1},{2},{3},{4},{5}", hostname, diskTotalCount,diskActiveCount,diskFailedCount, diskSpareCount,diskFailedMessage);
            return diskTotalCount;

        } catch ( Exception ex) {
            Console.WriteLine("error: {0}",ex.Message);
            return 0;
        }


    }
    private static void usage() {
        Console.WriteLine( PROGNAME +" v"+VERSION+"\t(c) 2006 Johan Burati\n\n" +
                           "Usage: "+PROGNAME+" <server>  [/?]\n\n"+
                           "  server\tMandatory argument, a hostname or a file containing a list of hostnames\n"+
//									"  /short\t\tReturn a shorter form of the uptime string\n"+
//									"  /community\tUse the specified read community string\n"+
                           "  /?\t\tDisplay this help message\n\n"
                         );
    }
    #endregion
}

