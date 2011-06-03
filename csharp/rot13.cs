// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;

class ROT13 {

    static void Main(string[] args) {

        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");
            if(File.Exists(args[0])) {
                StreamReader sr = File.OpenText(args[0]);
                string line;
                while((line=sr.ReadLine())!=null) {
                    Console.WriteLine(rot13(line));
                }
                sr.Close();
            } else {
                for(int i=0; i<args.Length; i++)
                    Console.Write(rot13(args[i]));
            }



        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
        //RL();

    }

    /// <summary>
    /// Encodes text using the ROT13 algorithm
    /// </summary>
    /// <param name="InputText"></param>
    /// <returns></returns>
    static string rot13(string InputText) {
        int i;
        char CurrentCharacter;
        int CurrentCharacterCode;
        string EncodedText = "";

        //Iterate through the length of the input parameter
        for (i = 0; i < InputText.Length; i++) {
            //Convert the current character to a char
            CurrentCharacter = System.Convert.ToChar(InputText.Substring(i, 1));

            //Get the character code of the current character
            CurrentCharacterCode = (int) CurrentCharacter;

            //Modify the character code of the character, - this
            //so that "a" becomes "n", "z" becomes "m", "N" becomes "Y" and so on
            if (CurrentCharacterCode >= 97 && CurrentCharacterCode <= 109) {
                CurrentCharacterCode = CurrentCharacterCode + 13;
            } else

                if (CurrentCharacterCode >= 110 && CurrentCharacterCode <= 122) {
                    CurrentCharacterCode = CurrentCharacterCode - 13;
                } else

                    if (CurrentCharacterCode >= 65 && CurrentCharacterCode <= 77) {
                        CurrentCharacterCode = CurrentCharacterCode + 13;
                    } else

                        if (CurrentCharacterCode >= 78 && CurrentCharacterCode <= 90) {
                            CurrentCharacterCode = CurrentCharacterCode - 13;
                        }

            //Add the current character to the string to be returned
            EncodedText = EncodedText + (char) CurrentCharacterCode;
        }

        return EncodedText;

    }

}