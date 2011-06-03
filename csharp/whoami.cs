// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;


public class WhoAmI {
    public static void Main(string[] args) {
        try {
            string username = Environment.ExpandEnvironmentVariables("%USERNAME%");
            string domain = Environment.ExpandEnvironmentVariables("%USERDNSDOMAIN%");
            Console.WriteLine("{0} {1}", username, domain);
        } catch (Exception ex) {
            Console.WriteLine("Error: {0}",ex.Message);
        }
    }


}