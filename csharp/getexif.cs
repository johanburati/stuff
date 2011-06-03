// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class MyClass {
    public static void Main(string[] args) {
        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");

            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] files = dir.GetFiles(args[0]);
            foreach(FileInfo file in files) {
                try {
                    WL("{0}\t{1}",file.Name, GetExifPropertyTagDateTime(file.Name));
                } catch(Exception) {
                    //WL("{0}\t",file.Name);
                }

            }
        } catch(Exception exc) {
            WL("ERROR: {0}", exc.Message);
        }
        //RL();
    }

    public static string GetExifPropertyTagDateTime(string fileName) {
        const int PropertyTagDateTime = 0x0132;
        string DateTime = "";

        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        Image image = Image.FromStream(stream, true, false);
        PropertyItem[] propItems = image.PropertyItems;

        // For each PropertyItem in the array, display the ID, type, and length and value.
        // 0x0132 _=

        foreach (PropertyItem propItem in propItems) {
            if (propItem.Id == PropertyTagDateTime) {
                DateTime = encoding.GetString(propItem.Value);
            }

        }
        return(DateTime);

    }

    public static string GetExifProperties(string fileName) {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        Image image = Image.FromStream(stream, true, false);
        PropertyItem[] propItems = image.PropertyItems;

        // For each PropertyItem in the array, display the ID, type, and length and value.
        // 0x0132 _=
        int count = 0;
        foreach (PropertyItem propItem in propItems) {
            Console.WriteLine("   Item: " + count);
            Console.WriteLine("   iD: 0x" + propItem.Id.ToString("x"));
            Console.WriteLine("   type: " + propItem.Type.ToString());
            Console.WriteLine("   length: " + propItem.Len.ToString() + " bytes");
            if (propItem.Len < 10) Console.WriteLine("   value: " + encoding.GetString(propItem.Value));
            count++;
        }
        // Convert the value of the second property to a string, and display / it.

        return(count.ToString());

    }


    #region Helper methods
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