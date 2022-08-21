using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Extensions
{

    public static Vector3 Average<T>(this IEnumerable<T> vectors, System.Func<T, Vector3> func)
    {
        return vectors.Sum(func) / vectors.Count();
    }

    public static Vector3 Sum<T>(this IEnumerable<T> vectors, System.Func<T, Vector3> func)
    {
        Vector3 result = Vector3.zero;

        foreach (var vector in vectors)
        {
            result += func(vector);
        }

        return result;
    }

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable.Count() == 0)
        {
            return default;
        }

        return enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count()));
    }

    public static void AddUnique<T>(this List<T> list, T element)
    {
        if (!list.Contains(element))
        {
            list.Add(element);
        }
    }

}
