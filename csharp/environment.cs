using System;

class environment {
    public static void Main () {
        Console.WriteLine("CommandLine: " + Environment.CommandLine.ToString());
        Console.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory.ToString());
        Console.WriteLine("ExitCode: " + Environment.ExitCode.ToString());
        Console.WriteLine("HasShutdownStarted: " + Environment.HasShutdownStarted.ToString());
        //Console.WriteLine("Is64BitOperatingSystem: " + Environment.Is64BitOperatingSystem.ToString());
        //Console.WriteLine("Is64BitProcess: " + Environment.Is64BitProcess.ToString());
        Console.WriteLine("MachineName: " + Environment.MachineName.ToString());
        Console.WriteLine("NewLine: " + Environment.NewLine.ToString());
        Console.WriteLine("OSVersion: " + Environment.OSVersion.ToString());
        Console.WriteLine("ProcessorCount: " + Environment.ProcessorCount.ToString());
        Console.WriteLine("StackTrace: " + Environment.StackTrace.ToString());
        Console.WriteLine("SystemDirectory: " + Environment.SystemDirectory.ToString());
        //Console.WriteLine("SystemPageSize: " + Environment.SystemPageSize.ToString());
        Console.WriteLine("TickCount: " + Environment.TickCount.ToString());
        Console.WriteLine("UserDomainName: " + Environment.UserDomainName.ToString());
        Console.WriteLine("UserInteractive: " + Environment.UserInteractive.ToString());
        Console.WriteLine("UserName: " + Environment.UserName.ToString());
        Console.WriteLine("Version: " + Environment.Version.ToString());
        Console.WriteLine("WorkingSet: " + Environment.WorkingSet.ToString());
    }
}

