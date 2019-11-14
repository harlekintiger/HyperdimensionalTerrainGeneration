using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethods
{

    public static T[] Populate<T>(this T[] arrayToPopulate, T valueToPopulateWith)
    {
        for (int i = 0; i < arrayToPopulate.Length; ++i)
        {
            arrayToPopulate[ i ] = valueToPopulateWith;
        }

        return arrayToPopulate;
    }
}
