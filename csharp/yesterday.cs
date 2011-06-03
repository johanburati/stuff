// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
class Yesterday {
    static void Main(string[] args) {
        Console.WriteLine(ymddate(DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0))));
    }
    private static string ymddate(DateTime dt) {
        return(String.Format("{0,4:00}{1,2:00}{2,2:00}", dt.Year, dt.Month, dt.Day));
    }
}
