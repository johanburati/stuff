//make csc.bat /target:library JohanBurati.cs /r:snmp.dll
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.Text;
using Snmp;

namespace JohanBurati {
public class Snmp {
    public string community = "public";

    public string GetUpTime(string server) {
        StringBuilder uptime = new StringBuilder();
        try {
            ManagerSession sess = new ManagerSession(server,community);

            uint[] oid = new uint[] {1,3,6,1,2,1,1,3,0};
            ManagerItem mi = new ManagerItem(sess,oid);
            string delim = "[]";
            int ticks = Convert.ToInt32(mi.Value.ToString().Trim(delim.ToCharArray()).Replace("NULL",""));
            ticks /= 100;
            int days = ticks / (60*60*24);
            ticks -= days * (60*60*24);
            int hours = ticks / (60*60);
            ticks -= hours*(60*60);
            int mins = ticks/(60);
            ticks -= mins*60;
            int secs = ticks;

            //uptime.AppendFormat("{0,2:00}d{1,2:00}h{2,2:00}m{3,2:00}s", days,hours,mins,secs);
            uptime.AppendFormat("{0,3} days, {1,2} hours, {2,2} mins, {3,2} secs", days,hours,mins,secs);
        } catch ( Exception ex) {
            uptime.AppendFormat("{0}",ex.Message);
        }
        return(uptime.ToString());
    }
    public string GetUpTimeShort(string server) {
        StringBuilder uptime = new StringBuilder();
        try {
            ManagerSession sess = new ManagerSession(server,community);

            uint[] oid = new uint[] {1,3,6,1,2,1,1,3,0};
            ManagerItem mi = new ManagerItem(sess,oid);
            string delim = "[]";
            int ticks = Convert.ToInt32(mi.Value.ToString().Trim(delim.ToCharArray()).Replace("NULL",""));
            ticks /= 100;
            int days = ticks / (60*60*24);
            ticks -= days * (60*60*24);
            int hours = ticks / (60*60);
            ticks -= hours*(60*60);
            int mins = ticks/(60);
            ticks -= mins*60;
            int secs = ticks;

            if(days > 0) {
                uptime.AppendFormat("{0,3:00} days, {1,2:00} hours", days, hours);
            } else if ( hours > 0) {
                uptime.AppendFormat("{0,2:00} hours, {1,2:00} mins", hours, mins);
            } else {
                uptime.AppendFormat("{0,2:00} mins, {1,2:00} secs", mins, secs);
            }
        } catch ( Exception ex) {
            uptime.AppendFormat("{0}",ex.Message);
        }
        return(uptime.ToString());
    }

    public string na_GetStatus(string server) {
        StringBuilder status = new StringBuilder();
        try {
            ManagerSession sess = new ManagerSession(server,community);
            uint[] oid = new uint[] {1,3,6,1,4,1,789,1,2,2,25,0};
            ManagerItem mi = new ManagerItem(sess,oid);
            string delim = "\"";
            status.Append(mi.Value.ToString().Trim(delim.ToCharArray()).Trim().Replace("NULL",""));
        } catch ( Exception ex) {
            status.Append(ex.Message);
        }
        return(status.ToString());
    }
}

/// <summary>
/// Summary description for Class1.
/// </summary>
public class NetApp {
    public NetApp() {
        //
        // TODO: Add constructor logic here
        //
    }
    public string GetStatus() {
        return("everything's okay");
    }
}
public class Report {
    public Report() {
        //
        // TODO: Add constructor logic here
        //
    }
    public string GetStatus() {
        return("JReport");
    }
    public string HtmlHeader(string title) {
        return(@"<html><head><title>"+title+"</title><meta name='Author' content='Johan Burati'><style type='text/css'>h1, h2, h3, body, table, tr, td { font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px; border:  0px solid white; padding: 2px } h1 { font-size: 16px; font-style: bold } h2 { font-size: 14px } h3 { font-size: 9px; color: #999999 } tr { background:  #ccc;} tr.red { background: #ffcccc; } tr.green {background: #99FF99;} td { text-align: left; } td.top { text-align: center; background-color: #262626; color: white; font-weight: bold} a, a:active, a:visited {	color: black;	text-decoration: none; } </style></head><body><center><h1>"+title+"</h1><table>");
    }
    public string HtmlRow(string style, params string[] items) {
        StringBuilder row = new StringBuilder();

        //if (style == "") { style = "grey"; }
        row.AppendFormat("<tr class={0}>", style);

        foreach(string item in items) {
            row.AppendFormat("<td class={0}>{1}</td>", style, item);
        }
        row.Append("</tr>");
        return(row.ToString());
    }
    public string HtmlFooter() {
        return(@"</table><br><br><br><h3>by Johan Burati</h3></center></body></html>");
    }


}
}
