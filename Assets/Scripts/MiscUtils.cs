using System;
using System.Globalization;
using UnityEngine;

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

    private static void ConvertStringArray(string[] messages)
    {
        // converts an array of string data to a double array
        // kept as doubles to prevent data loss
        // note: may be unoptimised as we should find a way to compress this data

        // -1 is used as an array end delimeter

        int totalLength = 0;
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
            StringToDoubleArr(messages[i]);
        }
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

    public static bool TryWriteItemsToArray<T>(T[] target, T[] newItems, int startIndex = 0)
    {
        /// tries to insert new items into the target array sequentially beginning at the start index
        /// will only insert data if the whole payload can be written
        /// returns next available index on success or -1 on failure
        
        // check given range is empty
        if (!CheckIndiceRangeIsEmpty(target, startIndex, startIndex + (newItems.Length - 1)))
            return false;

        // we can write the data
        for (int i = 0; i < target.Length; i++) 
        {
            target[startIndex + i] = newItems[i];
        }

        return true;
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
        bool success = true;

        if (endIndex >= target.Length)
            return false;

        // check inclusively that indices in the specified range are all null
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (target[i] is null)
            {
                continue;
            }
            else
            {
                success = false;
                break;
            }
        }

        return success;
    }
}
