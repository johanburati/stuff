// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Collections;


namespace RenExif {
public class RENEXIF {
    private static string PROGRAM = "renexif";
    private static string VERSION  = "1.00";
    private static string AUTHOR   = "Johan Burati";
    private static string EMAIL    = "johan@burati.com";
    private static string COPYRIGHT = PROGRAM+" v"+VERSION+"\t Copyright (c)2006 by "+AUTHOR+"("+EMAIL+")";

    private static bool option_debug = false;
    private static bool option_ren = true;

    private static Hashtable h = new Hashtable();

    static int Main(string[] args) {
        try {
            string path =Directory.GetCurrentDirectory();
            string file = "*.jpg";

            bool option_subdir = false;
            foreach ( string option in args )
                switch(option) {
                case "-help":
                case "/help":
                case "-h":
                case "/h":
                case "-?":
                case "/?":
                    usage();
                    return 1;
                case "-debug":
                case "/debug":
                    option_debug = true;
                    break;
                case "-ren":
                case "/ren":
                    option_ren = true;
                    break;
                case "-s":
                case "/s":
                case "/subdir":
                case "-subdir":
                    option_subdir = true;
                    break;
                default:
                    if(Directory.Exists(option)) path = option;
                    else file = option;
                    break;
                }
            if (option_debug) {
                WL("Debug On");
                if (option_subdir) WL("Subdir On");
                else WL("Subdir Off");
                WL("Debug: path({0}) file({1})", path, file);
            }
            if (option_subdir) subdir(path,file);
            else curdir(path,file);
        } catch (Exception ex) {
            WL("Error: "+ex.Message);
        }

        return(0);
    }

    #region My methods
    public static void process(string FullName) {
        try {
            if(option_debug) WL("Debug(process): file({0})",FullName);
            FileInfo f = new FileInfo(FullName);
            string NewName = GetExifDateTime(FullName);
            string FileName = f.Name;
            if (NewName != "") {
                NewName +=  f.Extension; // add file extention

                string FullNewName = f.DirectoryName+"\\" + NewName;
                WL("renaming {2}\\{0} to {2}\\{1}", f.Name, NewName, f.DirectoryName);
                if (h.ContainsKey(FullNewName) || (FullNewName == FullName) || File.Exists(FullNewName)) {
                    WL("Warning: file already exists.");
                } else {
                    h.Add(FullNewName,FullName);
                    if (option_ren) f.MoveTo(FullNewName);
                }
            } else {

                WL("Error: {1}\\{0} could not read Exif data.", f.Name, f.DirectoryName);
            }
        } catch (Exception ex) {
            WL("Error: {0}", ex.Message);
        }
    }
    public static string GetExifDateTime(string file) {
        string ExifDateTime = "";
        try {
            ExifDateTime = GetExifPropertyTagDateTime(file);
            ExifDateTime = Regex.Replace(ExifDateTime,":","");
            ExifDateTime = Regex.Replace(ExifDateTime," ","_");
            ExifDateTime = Regex.Replace(ExifDateTime,"\x0$","");
        } catch (Exception ex) {
            WL("Error(GetExifDateTime): {0}", ex.Message);
        }
        return ExifDateTime;
    }
    public static string GetExifPropertyTagDateTime(string file) {
        const int PropertyTagDateTime = 0x0132;
        string DateTime = "";
        try {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            using (FileStream stream = File.OpenRead(file)) {
                Image image = Image.FromStream(stream, true, false);
                PropertyItem[] propItems = image.PropertyItems;

                // For each PropertyItem in the array, display the ID, type, and length and value.
                // 0x0132 _=

                foreach (PropertyItem propItem in propItems) {
                    if (propItem.Id == PropertyTagDateTime) {
                        DateTime = encoding.GetString(propItem.Value);
                        break;
                    }
                }
            }
        } catch (Exception) {
            // if there was an error (such as read a defect image file), just ignore
        }
        return(DateTime);
    }

    private static void curdir(string path, string filemask) {
        try {
            foreach (string f in Directory.GetFiles(path, filemask)) {
                process(f);
            }
        } catch (Exception ex) {
            WL("Error: "+ex.Message);
        }
    }
    private static void subdir(string path, string filemask) {
        try {
            curdir(path, filemask);
            foreach (string d in Directory.GetDirectories(path)) {
                subdir(d,filemask);
            }
        } catch (Exception ex) {
            WL("Error: "+ex.Message);
        }
    }
    #endregion
    #region Helper methods
    private static void usage() {
        WL(COPYRIGHT);
        WL("\n\n"+PROGRAM + " (/subdir) [path]");
        WL("Rename the jpg files in the current or specified path according to the date and time\nof the EXIF properties (format: 'yyyymmdd_hhmmss.jpg')");
    }

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
}