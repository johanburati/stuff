//make csc2.bat getsrv.cs
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

public class MyClass {
    public static void Main(string[] args) {

        string Engineer = "";
        bool option_nonas = false;
        bool option_nasonly = false;
        bool option_full = false;
        bool option_active = false;

        //string[] fields = { "[Application-Description]","[OS]", "[Location]",[Serial#] };
        string[] fields = { "*" };



        foreach ( string option in args )
            switch(option) {
            case "-nonas":
            case "/nonas":
                option_nonas = true;
                break;
            case "-nasonly":
            case "/nasonly":
            case "-nas":
            case "/nas":
                option_nasonly = true;
                break;
            case "-full":
            case "/full":
            case "-all":
            case "/all":
                option_full = true;
                break;
            case "-active":
            case "/active":
                option_active = true;
                break;
            default:
                Engineer = option;
                break;
            }

        if (option_nonas && option_nasonly) {
            Console.WriteLine("you cannot choose both options (/nonas, /nasonly) at the sametime");
            return;
        }

        // Build sqlstring
        string sqlstring = "SELECT [ServerName]";
        //if (option_full) sqlstring += "," + System.String.Join(",",fields);
        if (option_full) sqlstring = "SELECT " + System.String.Join(",",fields);
        sqlstring += " FROM Wintel_Servers WHERE";
        if (Engineer != "") sqlstring += " [Primary SA]=\'"+Engineer+"\' AND";
        if( option_nonas) sqlstring += " [OS]<>\'ONTAP\' AND";
        if( option_nasonly) sqlstring += " [OS]=\'ONTAP\' AND";
        if (option_active) sqlstring += " [Server Status]=\'ACTIVE'\' AND";
        sqlstring  += " [ServerName]<>\'\' ORDER BY [ServerName];";
        Console.WriteLine(sqlstring);


        // deal with command line
        if(args.Length > 0) {
            Engineer = args[0].Replace("'", "''");
        }

        try {
            string connectionString = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\connectionStrings").GetValue("Infraconf").ToString();
            SqlConnection WSSconn = new SqlConnection(connectionString);
            SqlCommand WSScomm = new SqlCommand(sqlstring, WSSconn);
            WSScomm.Connection.Open();
            SqlDataReader WSSreader = WSScomm.ExecuteReader(CommandBehavior.CloseConnection);
            while(WSSreader.Read()) {
                string Row =  WSSreader.GetString(0);
                if(option_full) {
                    for(int i=0; i<WSSreader.FieldCount; i++) {
                        string Value = "" + (WSSreader.IsDBNull(i) ? "" : (WSSreader.GetValue(i)));
                        Row += ",\"" + Value.Trim() + "\"";
                    }
                }
                Console.WriteLine(Row);
            }
            WSSreader.Close();
            WSSconn.Close();
        } catch (Exception exc) {
            Console.WriteLine("ERROR: " + exc.Message );
        }
        //RL();
    }

    #region Helper methods

    private static void WL(object text, params object[] args) {
        Console.WriteLine(text.ToString(), args);
    }

    private static void RL() {
        Console.WriteLine("Press Enter to Continue...");
        Console.ReadLine();
    }

    private static void Break() {
        System.Diagnostics.Debugger.Break();
    }

    #endregion
}
