//make csc.bat /out:ac.exe ac.cs
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Diagnostics;

class ac {
    public static void Main(string[] args)  {

        if (args.Length < 1) {
            Console.WriteLine("Usage: ac sourcefile.cs");
            return;
        }

        try {
            string filename = args[0];
            string line;

            StreamReader sr=File.OpenText(filename);
            while ((line = sr.ReadLine()) != null) {
                if(line.ToLower().StartsWith("//make ")) {
                    //Console.WriteLine(line);
                    string[] tokens = line.Split(' ');
                    if (tokens.Length > 0) {
                        Process meProc = new Process();
                        meProc.StartInfo.FileName = tokens[1];
                        if (tokens.Length > 1) {
                            string arguments = "";
                            for (int i = 2; i < tokens.Length; i++) {
                                arguments += " "+tokens[i];
                            }
                            meProc.StartInfo.Arguments = arguments;
                        } else {
                            meProc.StartInfo.Arguments =  "";
                        }
                        meProc.StartInfo.UseShellExecute = false;
                        meProc.StartInfo.CreateNoWindow = true;
                        meProc.StartInfo.RedirectStandardError = true;
                        meProc.StartInfo.RedirectStandardOutput = true;
                        Console.WriteLine( "* make: " + meProc.StartInfo.FileName + meProc.StartInfo.Arguments ,":");
                        meProc.Start();
                        string stderr = meProc.StandardError.ReadToEnd();
                        string stdout = meProc.StandardOutput.ReadToEnd();
                        meProc.WaitForExit();
                        Console.WriteLine(stdout+stderr);
                    } // if
                }
            } // while
            sr.Close();

        }  catch(Exception ex)	 {
            Console.WriteLine("ERROR: " +ex.Message);
        }
    }
}
