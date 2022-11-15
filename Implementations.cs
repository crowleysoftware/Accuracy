using BenchmarkDotNet.Attributes;
using System.Collections.Specialized;

namespace Accuracy
{
    [MemoryDiagnoser]
    public class Implementations
    {
        [Benchmark]
        public char reversing() => FirstCharReversingLoop("accuracy");

        [Benchmark]
        public char twoLoops() => FirstCharTwoLoops("accuracy");

        [Benchmark]
        public string sortedDict() => SortedDictionary("accuracy");

        public char FirstCharReversingLoop(string word)
        {
            #region step-by-step
            /*
             * input: "accuracy"
             * 
             * start at "a"
             * 
             * -
             * accuracy :no (doesn't match itself)
             * ^
             * 
             * -
             * accuracy :no
             *  ^
             *  
             * -
             * accuracy :no
             *   ^
             *   
             * -
             * accuracy :no
             *    ^
             * 
             * -
             * accuracy : no
             *     ^
             *  
             * -
             * accuracy : yes
             *      ^
             *      
             * increment search index to be 1 ("c")
             * start loop back at 0
             * 
             *  -
             * accuracy :no
             * ^
             * 
             *  -
             * accuracy :no (doesn't match itself)
             *  ^
             *  
             *  -
             * accuracy :yes
             *   ^
             *  
             * increment search index to be 2 ("c")
             * start loop back at 0
             * 
             *   -
             * accuracy :no
             * ^
             * 
             *   -
             * accuracy :yes
             *  ^
             * 
             * increment search index to be 3 ("u")
             * start loop back at 0
             * 
             *    -
             * accuracy :no
             * ^
             *
             *    -
             * accuracy :no
             *  ^
             *  
             *    -
             * accuracy :no
             *   ^
             *   
             *    -
             * accuracy :no (doesn't match itself)
             *    ^
             *    
             *    -
             * accuracy :no
             *     ^
             *     
             *    -
             * accuracy :no
             *      ^
             *      
             *    -
             * accuracy :no
             *       ^
             *     
             *    -
             * accuracy :no
             *        ^
             *        
             * end of string without finding more than one "u". This is the first non-repeating character.
            */
            #endregion

            if (word.Length == 1) return word[0];

            char currentSrch = word[0]; //seed with first character
            int curLtrIdx = 0;

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == currentSrch && i != curLtrIdx)
                {
                    //found a repeat, now start testing the next letter starting at beginning of string again
                    ++curLtrIdx;

                    if (curLtrIdx > word.Length - 1) throw new InvalidOperationException("No non-repeating characters in the string");

                    i = -1; //want to start back at zero so this will become zero after loop executes i++

                    currentSrch = word[curLtrIdx];
                }
            }

            return currentSrch;
        }

        public string SortedDictionary(string word)
        {
            var dict = new OrderedDictionary();

            foreach (var aChar in word)
            {
                var newCount = 0;
                var key = aChar.ToString();
                if (dict.Contains(key))
                {
                    var count = (int)(dict[key]);
                    dict[key] = ++count;
                }
                else dict.Add(key, 1);
            }

            foreach (var aPair in dict.Keys)
            {
                var count = (int)dict[aPair];
                if (count == 1)
                {
                    return aPair.ToString();
                }
            }

            throw new InvalidOperationException("No non-repeating chars");
        }

        public char FirstCharTwoLoops(string word)
        {
            //key = the character, value = (position, isFound)
            Dictionary<char, Tracker> keyValuePairs = new();

            //Load dictionary with each character and position and found status
            for (int i = 0; i < word.Length; i++)
            {
                if (!keyValuePairs.ContainsKey(word[i]))
                {
                    keyValuePairs.Add(word[i], new Tracker { position = i, isFound = false });
                }
                else
                {
                    keyValuePairs[word[i]].isFound = true;
                }
            }

            //seed at max position in string
            int idx = word.Length - 1;

            //loop over dictionary, set idx to lowest position of not found character
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                var item = keyValuePairs.ElementAt(i).Value;

                if (!item.isFound)
                {
                    if (idx > item.position)
                    {
                        idx = item.position;

                    }
                }
            }

            return word[idx];
        }
    }

    sealed class Tracker
    {
        public int position { get; set; }
        public bool isFound { get; set; }

        public override string ToString()
        {
            return $"position: {position}, found: {isFound}";
        }
    }
}
