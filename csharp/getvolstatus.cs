//make csc2.bat  getvolstatus.cs /r:snmp.dll
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
    private static string VERSION = "1.1";
    // 1.1 changed all UInt32 to UInt64 because UInt32 was too small in some case

    public static void Main(string[] args) {

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
                Console.WriteLine("SystemName!Description!Total_Size (MB)!Used (MB)!Percentage_Used");
                if(File.Exists(server)) {
                    StreamReader sr = File.OpenText(server);
                    while((server=sr.ReadLine())!=null) {
                        //Console.WriteLine("Trying {0}({1})", server,community );
                        getVolStatus(server,community );
                    }
                    sr.Close();
                } else {
                    //Console.WriteLine("Trying {0}({1})", server,community );
                    getVolStatus(server,community );
                }
            }

        } catch ( Exception ex) {
            Console.WriteLine("error1: {0}",ex.Message);
        }
    }

    #region VolStatus methods
    private static void getVolStatus(string hostname, string community) {
        try {
            uint[] na_dfNumber = new uint[] {1,3,6,1,4,1,789,1,5,6,0};
            //uint[] na_dfFileSys= new uint[] {1,3,6,1,4,1,789,1,5,4,1,2,1};
            uint[] oid = new uint[] {1,3,6,1,2,1,1,3,0};

            UInt64 dfNumber = 0;
            {
                ManagerSession sess = new ManagerSession(hostname,community);
                ManagerItem mi = new ManagerItem(sess,na_dfNumber);
                string value   = mi.Value.ToString();
                dfNumber = Convert.ToUInt64(value);
            }

            for(uint i=1; i<dfNumber; i+=2) {
                uint[] na_dfFileSys= new uint[] {1,3,6,1,4,1,789,1,5,4,1,2,i};
                uint[] na_dfKBytesTotal= new uint[] {1,3,6,1,4,1,789,1,5,4,1,3,i};
                uint[] na_dfKBytesUsed= new uint[] {1,3,6,1,4,1,789,1,5,4,1,4,i};
                uint[] na_dfPercentKBytesCapacity= new uint[] {1,3,6,1,4,1,789,1,5,4,1,6,i};

                ManagerSession sess = new ManagerSession(hostname,community);
                string dfFileSys, dfKBytesTotal, dfKBytesUsed, dfPercentKBytesCapacity = "";
                {
                    ManagerItem mi = new ManagerItem(sess,na_dfFileSys);
                    dfFileSys   = mi.Value.ToString();
                    Match m = Regex.Match(dfFileSys, @"/vol/(?<volume>\S+)/");
                    if(m.Success) dfFileSys = m.Groups[1].ToString();
                }
                {
                    ManagerItem mi = new ManagerItem(sess,na_dfKBytesTotal);
                    dfKBytesTotal   = mi.Value.ToString();
                    dfKBytesTotal   = Convert.ToString(Convert.ToUInt64(dfKBytesTotal) / 1024);
                }
                {
                    ManagerItem mi = new ManagerItem(sess,na_dfKBytesUsed);
                    dfKBytesUsed   = mi.Value.ToString();
                    dfKBytesUsed   = Convert.ToString(Convert.ToUInt64(dfKBytesUsed) / 1024);
                }
                {
                    ManagerItem mi = new ManagerItem(sess,na_dfPercentKBytesCapacity);
                    dfPercentKBytesCapacity  = mi.Value.ToString();
                }
                Console.WriteLine("{0}!{1}!{2}!{3}!{4}", hostname, dfFileSys, dfKBytesTotal,dfKBytesUsed,dfPercentKBytesCapacity);
            }
        } catch ( Exception ex) {
            Console.WriteLine("error: {0}",ex.Message);
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

