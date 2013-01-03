using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace iBoox.Helpers
{
    public static class AntiRude
    {
        // C проверкой подмены ЛАТ <- РУС пока повременим (усложняет процедуру)
        // И разбиение слов точками и прочими знаками, типа Б.Л.Я, тоже пока не делаем
        //private static Dictionary<string, string> LetMatches = new Dictionary<string,string> {
        //    {"a", "а"},
        //    {"c", "с"},
        //    {"e", "е"},
        //    {"k", "к"},
        //    {"m", "м"},
        //    {"o", "о"},
        //    {"x", "х"},
        //    {"y", "у"},
        //    {"ё", "е"}
        //};

        private static string[] BadWords = {
            "ху[йияе]",
            "^хул[ие]$",
            "п[иея][зс]д",
            "^бля$",
            "бля[дтц]",
            "^(с|сц)ук[аои]",
            "^c[сц][аиоуы][^д]",    // ссуда
            "^еб",
            "[аеиоуыя]еб",
            "[^л]еб[аи][нсщц]",     // колебания
            "ебу[чщ]",
            "пид[ое]р",
            "г[ао]ндон",
            "залуп"
        };

        public static string AntiRudeFilter(this string source)
        {
            string[] words = source.Split(' ');     // разбили текст на слова

            string splittedWord = string.Empty;     // сюда будем записывать потенциально "разбитые на буквы" ругательства, типа Б Л Я
            string censoredWord;                    // а сюда - поочередно все исправленные слова
            int splittedStart = 0, splittedEnd = 0; // это начальный и конечный индекс слов в потенциальном "разбитом на буквы" ругательстве
            bool isSplitted = false;

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length == 1)
                {
                    if (!isSplitted) { splittedStart = i; }

                    splittedWord += words[i];
                    isSplitted = true;

                    if (i < words.Length - 1) { continue; } // Если не конец текста, то берем следующее слово, а если конец - надо проверить
                }

                if (isSplitted)
                {
                    if (splittedWord.Length > 1)    // Если нам попался НЕ предлог или союз, а подозрительное слово, то проверяем
                    {
                        splittedEnd = i - 1;
                        censoredWord = string.Empty;
                        foreach (string pattern in BadWords)
                        {
                            censoredWord = Regex.Replace(splittedWord, pattern, "***", RegexOptions.IgnoreCase);
                            if (censoredWord.Contains("***"))
                                break;
                        }

                        if (censoredWord.Contains("***"))    // Если что-то нехорошее все-таки обнаружили
                        {
                            for (int j = splittedStart; j <= splittedEnd; j++)
                            {
                                words[j] = "*";
                            }
                        }
                    }

                    // В любом случае обнуляем всё для поиска нового "разбитого на буквы" ругательства
                    splittedWord = "";
                    isSplitted = false;
                    splittedStart = 0;
                    splittedEnd = 0;
                }

                foreach (string pattern in BadWords)
                {
                    words[i] = Regex.Replace(words[i], pattern, "***", RegexOptions.IgnoreCase);
                }
            }

            string result = string.Join(" ", words);

            //foreach (KeyValuePair<string, string> item in let_matches)
            //{
                
            //}
            return result;
        }
    }
}