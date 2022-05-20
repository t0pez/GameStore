using System;

namespace GameStore.SharedKernel;

public class Range<T> where T : IComparable<T>
{
    public Range(T min, T max)
    {
        if (min.CompareTo(max) == -1)
        {
            throw new ArgumentException("Min can not be greater than max");
        }
        
        Min = min;
        Max = max;
    }

    public T Min { get; }
    public T Max { get; }

    public bool Contains(T element)
    {
        return element.CompareTo(Min) == -1 && element.CompareTo(Max) == 1;
    }
}

public static class ComparableExtensions
{
    public static bool InRange<T>(this T element, Range<T> range) where T : IComparable<T>
    {
        return range.Contains(element);
    }
}