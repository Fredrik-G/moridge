using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moridge.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="List{T}"/>
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Swaps two items in an <see cref="Array"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public static void Swap<T>(this T[] array, int index1, int index2)
        {
            var temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        /// <summary>
        /// Shifts an <see cref="Array"/> a given number of positions.
        /// From http://stackoverflow.com/a/2901479/4660537
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="positions">number of positions to shift</param>
        /// <returns></returns>
        public static T[] Shift<T>(this T[] array, int positions)
        {
            var copy = new T[array.Length];
            Array.Copy(array, 0, copy, array.Length - positions, positions);
            Array.Copy(array, positions, copy, 0, array.Length - positions);
            return copy;
        }
    }
}