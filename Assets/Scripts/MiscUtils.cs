using System;
using System.Globalization;

public static class MiscUtils
{
    private static double[] StringToDoubleArr(string target)
    {
        double[] convertedString = new double[target.Length];

        char[] targetAsChars = target.ToCharArray();

        for (int i = 0; i < targetAsChars.Length; i++)
        {
            convertedString[i] = CharUnicodeInfo.GetNumericValue(targetAsChars[i]);
        }

        return convertedString;
    }

    public static double[] ConvertStringArray(string[] messages)
    {
        // converts an array of string data to a double array
        // kept as doubles to prevent data loss
        // note: may be unoptimised as we should find a way to compress this data

        // -1 is used as an array end delimeter

        int totalLength = 0;
        int currentStartIndex = 0;
        int delimIndex = 0;

        double[] finalArr;

        // calculate total length needed, add the delimeter character
        for (int stringIndex = 0; stringIndex < messages.Length; stringIndex++)
        {
            totalLength += messages[stringIndex].Length;
        }

        totalLength += messages.Length;

        finalArr = new double[totalLength];

        for (int i = 0; i < messages.Length; i++)
        {
            // grab the array & try to insert it into the finalArr
            delimIndex = TryWriteItemsToArray(finalArr, StringToDoubleArr(messages[i]), currentStartIndex);
            currentStartIndex = delimIndex + 1;

            if (delimIndex == -1)
                break;

            // insert delimiter
            finalArr[delimIndex] = -1;
        }

        return finalArr;
    }

    public static void DecodeDelimitedCharArrAsString(double[] encodedChars)
    {
        string[] decodedString;

        // calculate needed length
        // loop through it & count occurences of -1

        int sections = Array.FindAll<double>(encodedChars, (double i) => { return i == -1; }).Length;

        decodedString = new string[sections];


    }

    public static string CharAsNumericsToString(double[] charSequence)
    {
        // converts the given char sequence (stored as doubles representing the UTF value) to a string
        string convertedString = "";

        // use a direct cast to convert the value (u/theFlyingCode, 24/12/2020 https://www.reddit.com/r/csharp/comments/kjadoo/comment/ggvmdjb/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button)
        for (int charIndex = 0; charIndex < charSequence.Length; charIndex++) 
        {
            convertedString = convertedString + ((char)charSequence[charIndex]).ToString();
        }

        return convertedString;
    }

    public static bool TryInsertDataToArray<T>(T[] target, T newItem)
    {
        // tries to add the specified item to the target array at first empty position
        // returns a boolean value indicating success status

        // insert data at found position
        int pos = FindFirstEmptyPositionInArray(target);

        if (pos == -1)
            return false;

        target[pos] = newItem;
        return true;
    }

    public static int TryWriteItemsToArray<T>(T[] target, T[] newItems, int startIndex = 0)
    {
        /// tries to insert new items into the target array sequentially beginning at the start index
        /// will only insert data if the whole payload can be written
        /// returns next available index on success or -1 on failure
        
        // check given range is empty
        if (!CheckIndiceRangeIsEmpty(target, startIndex, startIndex + (newItems.Length - 1)))
            return -1;

        // we can write the data
        for (int i = 0; i < newItems.Length; i++) 
        {
            target[startIndex + i] = newItems[i];
        }

        return startIndex + newItems.Length; // use this to get next available index
    }

    private static int FindFirstEmptyPositionInArray<T>(T[] target, int startIndex = 0)
    {
        // finds the first empty position in specified array, returns -1 if no empty position exists

        // go through array to check for first empty
        int emptyPos = -1;

        for (int potIndex = startIndex; potIndex < target.Length; potIndex++)
        {
            // check if current index is empty
            if (target[potIndex] is null)
            {
                emptyPos = potIndex;
                break;
            }
        }

        return emptyPos;
    }

    private static bool CheckIndiceRangeIsEmpty<T>(T[] target, int startIndex, int endIndex)
    {
        // make subarray
        T[] subArr = new T[(endIndex - startIndex) + 1];

        // populate it with the items
        for (int i = 0; i < subArr.Length; i++) 
        {
            subArr[i] = target[startIndex + i];
        }

        // run a trueforall check to see if the target is null
        return Array.TrueForAll(target, (T item) => { return item is null; });
    }
}
