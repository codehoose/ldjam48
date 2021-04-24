using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IntArrayExtensions
{
    public static bool Contains(this int[] array, int value)
    {
        foreach(var i in array)
        {
            if (i == value)
                return true;
        }

        return false;
    }
}