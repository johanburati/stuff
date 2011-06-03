//make csc.bat  getuptime.cs /r:snmp.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using Snmp;
using X690;
using System.IO;
using System.Net;

public class GetUpTimeClass {
    private static string PROGNAME = "getuptime";
    private static string VERSION = "1.0";

    public static void Main(string[] args) {

        string server = "";
        string community = "public";
        bool shortOn = false;

        if (args.Length == 0) {
            usage();
            return;
        }
        foreach ( string option in args )
            switch ( option )  {
            case "-short":
            case "/short":
            case "-s":
            case "/s":
                shortOn = true;
                break;
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
                    if(shortOn) {
                        Console.WriteLine("{0}:\t{1}",server, getuptime(server,community ) );
                    } else {
                        Console.WriteLine("{0}:\t{1}",server, getuptime(server,community ) );
                    }
                }
                sr.Close();
            } else {
                if(shortOn) {
                    Console.WriteLine("{0}:\t{1}",server, getuptime(server,community ) );
                } else {
                    Console.WriteLine("{0}:\t{1}",server, getuptime(server,community ) );
                }
            }
        }
    }

    private static string getuptime(string hostname, string community) {
        try {
            ManagerSession sess = new ManagerSession(hostname,community);

            uint[] oid = new uint[] {1,3,6,1,2,1,1,3,0};
            ManagerItem mi = new ManagerItem(sess,oid);
            string delim = "[]";
            int ticks = Convert.ToInt32(mi.Value.ToString().Trim(delim.ToCharArray()));
            ticks /= 100;
            int days = ticks / (60*60*24);
            ticks -= days * (60*60*24);
            int hours = ticks / (60*60);
            ticks -= hours*(60*60);
            int mins = ticks/(60);
            ticks -= mins*60;
            int secs = ticks;
            if (days > 1) {
                //return (String.Format("{0} days,  {1,2:00}:{2,2:00}:{3,2:00}", days,hours,mins,secs));
                return (String.Format("{0} days, {1} hours, {2} mins, {3} secs", days,hours,mins,secs));
            } else {
                //return (String.Format("{1} day,  {1,2:00}:{2,2:00}:{3,2:00}", days,hours,mins,secs))
                return (String.Format("{0} days, {1} hours, {2} mins, {3} secs", days,hours,mins,secs));;
            }
        } catch ( Exception ex) {
            return (String.Format("error: {0}",ex.Message));
        }
    }
    private static void usage() {
        Console.WriteLine( PROGNAME +" v"+VERSION+"\t(c) 2005 Johan Burati\n" +
                           "Return the uptime of a given host or list of hosts.\n\n"+
                           "Usage: "+PROGNAME+" <server>  [/short] [/?]\n"+
                           "server\tMandatory argument, a hostname or a file containing a list of hostnames\n"+
//										"/short\t\tReturn a shorter form of the uptime string\n"+
//										"/community\tUse the specified read community string\n"+
                           "/?\t\tDisplay this help message\n\n"+
                           "Note: "+PROGNAME+"get the uptime information via SNMP"
                         );
    }

}

