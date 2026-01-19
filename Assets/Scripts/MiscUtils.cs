using System;
using System.Text;
using System.Collections.Generic;

public static class MiscUtils
{
    private static UTF8Encoding utf8Encoder = new();    

    private static int[] StringToIntArr(string target)
    {
        Byte[] convertedString;

        char[] targetAsChars = target.ToCharArray();

        convertedString = utf8Encoder.GetBytes(target);

        return ByteToIntArr(convertedString);
    }
    public static string[] DecodeDelimitedCharArrAsString(int[] encodedChars)
    {
        string[] decodedString;

        // calculate needed length
        // loop through it & count occurences of -1

        int sections = Array.FindAll<int>(encodedChars, (int i) => { return i == -1; }).Length;

        decodedString = new string[sections];

        // get the locations of all the -1 delims
        int[] breaks = GetAllIndicesOfValue(encodedChars, -1);

        // starting index for current section
        int sectionStartIndex = 0;

        // loop for each encoded string
        for (int stringIndex = 0; stringIndex < breaks.Length; stringIndex++)
        {
            // get the subarray containing the characters
            int[] chars = GetSubArrayFromIndices(encodedChars, sectionStartIndex, breaks[stringIndex] - 1);

            // convert char array to string
            string data = ByteCharsToString(chars);
            // add it to the array
            TryInsertDataToArray(decodedString, data);
        }

        return decodedString;
    }

    public static int[] ConvertStringArray(string[] messages)
    {
        // converts an array of string data to a double array
        // kept as doubles to prevent data loss
        // note: may be unoptimised as we should find a way to compress this data

        // -1 is used as an array end delimeter

        int totalLength = 0;
        int currentStartIndex = 0;
        int delimIndex = 0;

        int[] finalArr;

        // calculate total length needed, add the delimeter character
        for (int stringIndex = 0; stringIndex < messages.Length; stringIndex++)
        {
            totalLength += messages[stringIndex].Length;
        }

        totalLength += messages.Length;

        finalArr = new int[totalLength];

        for (int i = 0; i < messages.Length; i++)
        {
            // grab the array & try to insert it into the finalArr
            delimIndex = TryWriteItemsToArray(finalArr, StringToIntArr(messages[i]), currentStartIndex);
            currentStartIndex = delimIndex + 1;

            if (delimIndex == -1)
                break;

            // insert delimiter
            finalArr[delimIndex] = -1;
        }

        return finalArr;
    }



    public static string ByteCharsToString(int[] charSequence)
    {
        // converts the given char sequence (stored as ints representing the UTF value) to a string
        // convert each char to a byte
        byte[] charBytes = IntToByteArr(charSequence);

        return utf8Encoder.GetString(charBytes);
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

    public static T[] GetSubArray<T>(T[] arr, int startIndex, int length)
    {
        T[] subArray = new T[length];

        for (int i = startIndex; i < length; i++)
        {
            subArray[i] = arr[startIndex + i];
        }

        return subArray;
    }

    public static T[] GetSubArrayFromIndices<T>(T[] arr, int startIndex, int endIndex)
    {
        T[] subArray = new T[endIndex - startIndex];

        for (int i = startIndex; i < endIndex; i++)
        {
            subArray[i] = arr[startIndex + i];
        }

        return subArray;
    }

    public static int[] GetAllIndicesOfValue<T>(T[] arr, T value)
    {
        List<int> indicies = new();

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] is not null)
            {
                if (arr[i].Equals(value))
                {
                    indicies.Add(i);
                }
            }
        }

        return indicies.ToArray();
    }

    private static int[] ByteToIntArr(byte[] bytes)
    {
        int[] converted = new int[bytes.Length];

        for (int i = 0; i < bytes.Length; i++)
        {
            converted[i] = (int)bytes[i];
        }

        return converted;
    }

    private static byte[] IntToByteArr(int[] values)
    {
        byte[] byteArr = new byte[values.Length];

        for (int i = 0; i < byteArr.Length; i++)
        {
            byteArr[i] = (byte)values[i];
        }

        return byteArr;
    }
}
