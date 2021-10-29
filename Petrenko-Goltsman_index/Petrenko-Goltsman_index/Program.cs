using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Petrenko_Goltsman_index
{
    class Program
    {
        static void Main(string[] args)
        {
            //get list of english strings
            Dictionary<string, float> english = new Dictionary<string, float>();
            var logFile = File.ReadAllLines(@".\Files\English.txt");

            //calculate index and store in dictionary
            foreach (var item in logFile)
            {
                english.Add(item, IndexPetrenko.CalculateIndexWithComment(item));
            }
            //get list of russian strings (text here is in german, i parsed batch07-de for data, but no essential difference)
            Dictionary<string, float> russian = new Dictionary<string, float>();
            logFile = File.ReadAllLines(@".\Files\Russian.txt");

            //calculate index and store in dictionary
            foreach (var item in logFile)
            {
                russian.Add(item, IndexPetrenko.CalculateIndex(item));
            }

            //query to join dictionaries by index value and select matching results. It is also possible to group/sort result depending on further demands
            //On small quantities of text PLINQ is relatively slower because of threads allocation (still a matter of miliseconds), but in case of large quantities it should be much better
            var query = from dictRuss in russian.AsParallel()
                         join dictEng in english.AsParallel() on dictRuss.Value equals dictEng.Value
                         select new
                         {
                             Index = dictRuss.Value,
                             Russian = dictRuss.Key,
                             English = dictEng.Key
                         };

            //show result in console
            foreach (var item in query)
            {
                Console.WriteLine($"{item.Index}-{item.Russian}-{item.English}");
            }
        }
    }
}
