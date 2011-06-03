// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;
using System.Text.RegularExpressions;


class Pauker {

    static void Main(string[] args) {


        try {

            if (args.Length == 0) throw new ArgumentException("you need to specify an argument.");
            if(File.Exists(args[0])) {
                StreamReader sr = File.OpenText(args[0]);
                string line;
                bool newrow = false;
                string kanji = "";
                string hiragana = "";
                string english = "";
                while((line=sr.ReadLine())!=null) {
                    if (Regex.Match(line, "class=\"row").Success) {
                        newrow = true;
                        kanji = "";
                        hiragana = "";
                        english = "";
                    }
                    if (newrow && Regex.Match(line, "</tr>").Success) {
                        newrow = false;

                        if (english != "") {
                            if (kanji == "") {
                                Console.WriteLine("{0}\r\n{1}", hiragana, english);
                            } else {
                                Console.WriteLine("{0}\r\n[{1}] {2}", kanji, hiragana, english);
                            }
                        }
                    }


                    if (Regex.Match(line, "class=\"kanji").Success)  {
                        string output = Regex.Replace(line,@"<(.|\n)*?>",string.Empty);
                        output = Regex.Replace(output,"\r\n",";");

                        if (hiragana == "") {
                            hiragana = output;
                        } else {
                            kanji = output;
                        }
                    }
                    if (Regex.Match(line, "class=\"en").Success)  {
                        string output = Regex.Replace(line,@"<(.|\n)*?>",string.Empty);
                        english = output;
                    }

                }
                sr.Close();
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