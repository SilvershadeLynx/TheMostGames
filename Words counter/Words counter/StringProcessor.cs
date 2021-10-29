using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Words_counter
{
    internal static class StringProcessor
    {
        /// <summary>
        /// Splits input string into a string list with cusom delimitter or set of delimitters, excluding empty entries
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="delimiterChars"></param>
        /// <returns></returns>
        private static List<string> SplitString(string inputString, char[] delimiterChars)
        {
            char[] delimiter = delimiterChars;

            string text = inputString;

            List<string> outputList = text.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList();

            return outputList;
        }

        /// <summary>
        /// Convert input string delimited by ',' or ';' into valid identifiers list, removes duplicates and incorrect values
        /// </summary>
        /// <param name="identifiersString"></param>
        /// <returns>List of unique int identifiers</returns>
        public static List<string> GetUniqueIdentifiersList(string identifiersString)
        {
            char[] delimiter = { ',', ';' };
            List<string> identifiersList = SplitString(identifiersString, delimiter)
                //try to convert delimited strings to int to ensure it's valid
                .Select(str =>
                {
                    bool success = int.TryParse(str, out int value);
                    return new { value, success };
                })
                //select all valid distinct values between 1 and 20
                .Where(pair => pair.success && pair.value >= 1 && pair.value <= 20)
                .Select(pair => pair.value.ToString())
                .Distinct()
                .ToList();

            return identifiersList;
        }

        /// <summary>
        /// Counts number of words in string delimited by space
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>Count of words in string</returns>
        public static int CountWords(string inputString)
        {
            //i'm aware that this is VERY primitive counter. But for sake of simplicity and lack of specifical instructions i'm leaving it as it is. 
            //to improve the quality of counter i suppose you have to ignore some conjunctions, punctuation symbols, etc. 
            //I bet there's a special black list exist for things you have to exclude in each language
            char[] delimiter = { ' ' };
            int count = SplitString(inputString, delimiter).Count;

            return count;
        }

        /// <summary>
        /// Counts number of vowels in string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static int CountVowels(string inputString)
        {
            //also highly simplified method. While searching for information about vowels i found tonns of conditions for several symbols to be considered as vowel or as consonant (letter 'y' for example). I'm not even talking about special symbols e.g. 'æ', 'ç', 'ɛ', etc.
            //I did some research and found the way to eliminate diacritics for most of the symbols to make it possible to count them as standart ones, but i need professional help with the special symbols and their usage.
            int counter = 0;
            string stringWithoutDiacritics = RemoveDiacritics(inputString.ToLower());
            char[] vowels = {
                //cyrillic vowels
                //'а',      'е'     'и'      'о'       'у'      'ы'      'э'      'ю'      'я'      'є'      'і'       'ї'
                '\u0430','\u0435','\u0438','\u043e','\u0443','\u044b','\u044d','\u044e','\u044f','\u0454','\u0456', '\u0457',
                //european (latin) vowels
                //'a'        'e'       'i'       'o'       'u'
                '\u0061', '\u0065', '\u0069', '\u006f', '\u0075'
            };

            foreach (char letter in stringWithoutDiacritics)
            {
                if (vowels.Contains(letter))
                {
                    counter++;
                }
            }

            return counter;
        }

        /// <summary>
        /// Removes diacritics and deletes unicode characters intended to be combined with another character from string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string RemoveDiacritics(string text)
        {
            Regex nonSpacingMarkRegex = new Regex(@"\p{Mn}", RegexOptions.Compiled);
            if (text == null)
                return string.Empty;

            //normalize unicode with canonical decompression
            string normalizedText = text.Normalize(NormalizationForm.FormD);
            //remove unicode character intended to be combined with another character without taking up extra space
            return nonSpacingMarkRegex.Replace(normalizedText, string.Empty);
        }

        /// <summary>
        /// Get plain text from RichTextBox
        /// </summary>
        /// <param name="rtb"></param>
        /// <returns></returns>
        public static string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            //trim excess \r\n
            textRange.Text = textRange.Text.Trim();
            return textRange.Text;
        }

        public static bool TryHighlightIncorrectIdentifiers(RichTextBox rtb)
        {
            char[] delimiter = { ',', ';' };            
            string inputString = StringFromRichTextBox(rtb);
            List<string> validIdentifiers = GetUniqueIdentifiersList(inputString);
            List<string> allIdentifiers = SplitString(inputString, delimiter);
            List<TextRange> textRanges = new List<TextRange>();

            //flag for warning message
            bool isSuccess = false;

            foreach (string identifier in allIdentifiers)
            {
                if (!validIdentifiers.Contains(identifier.Trim()) && identifier.Trim() != "")
                {
                    //regex to find current identifier
                    Regex regex = new Regex(Regex.Escape(identifier.Trim()));
                    //place cursor at the beginning of document
                    rtb.CaretPosition = rtb.CaretPosition.DocumentStart;
                    //find starting position of incorrect identifier
                    TextPointer identifierStart = rtb.CaretPosition.GetPositionAtOffset(inputString.IndexOf(identifier), LogicalDirection.Forward);
                    //find end position
                    TextPointer identifierEnd = identifierStart.GetPositionAtOffset(identifier.Length, LogicalDirection.Forward);
                    //save range for further highlight
                    TextRange wordRange = new TextRange(identifierStart, identifierEnd);
                    textRanges.Add(wordRange);
                    //replace current identifier with blank spaces in case if there several duplicates (IndexOf always search for first)
                    inputString = regex.Replace(inputString, new string(' ', identifier.Trim().Length), 1);

                    isSuccess = true;
                }
            }

            //if any incorrect ranges found - highlight them
            if (isSuccess)
            {
                foreach (TextRange wordRange in textRanges)
                {
                    wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    wordRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
            }

            
            return isSuccess;
        }
    }
}
