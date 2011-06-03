// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Security.Permissions;
using Microsoft.Win32;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,
                                       Read = @"HKEY_LOCAL_MACHINE\Software")]
[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum,
                                       UnmanagedCode = true)]

class RemoteKey {
    static void Main(string[] args) {
        RegistryKey environmentKey;
        string server;

        // Check that an argument was specified when the
        // program was invoked.
        if(args.Length == 0) {
            Console.WriteLine("Error: The name of the remote " +
                              "computer must be specified when the program is " +
                              "invoked.");
            return;
        } else {
            server = args[0];
        }

        try {
            // Open HKEY_CURRENT_USER\Environment
            // on a remote computer.
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion

            environmentKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine,server).OpenSubKey("software").OpenSubKey("Microsoft").OpenSubKey("Windows NT").OpenSubKey("CurrentVersion");

        } catch(IOException e) {
            Console.WriteLine("{0}: {1}",
                              e.GetType().Name, e.Message);
            return;
        }

        // Print the values.
        Console.WriteLine("\nThere are {0} values for {1}.",
                          environmentKey.ValueCount.ToString(),
                          environmentKey.Name);
        foreach(string valueName in environmentKey.GetValueNames()) {
            Console.WriteLine("{0,-20}: {1}", valueName,
                              environmentKey.GetValue(valueName).ToString());
        }

        // Close the registry key.
        environmentKey.Close();
    }
}