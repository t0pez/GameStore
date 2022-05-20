#nullable enable
using System;
using System.Collections;

namespace GameStore.SharedKernel;

public class Range<T> where T : struct, IComparable
{
    private readonly IComparer? _comparer;

    public Range(T? min, T? max, IComparer? comparer = null)
    {
        if (min is not null && max is not null && min.Value.IsGreaterThan(max))
        {
            throw new ArgumentException("Min can not be greater than max");
        }

        Min = min;
        Max = max;
        _comparer = comparer;
    }

    public T? Min { get; }
    public T? Max { get; }

    private bool IsMinEnabled => Min is not null;
    private bool IsMaxEnabled => Max is not null;
    private bool HasCustomComparer => _comparer is not null;

    public bool Contains(T element)
    {
        return ElementGreaterOrEqualToMin(element) &&
               ElementLessOrEqualToMax(element);
    }

    private bool ElementLessOrEqualToMax(T element)
    {
        if (HasCustomComparer)
        {
            return element.IsLessThan(Max, _comparer) || element.IsEqualTo(Max, _comparer);
        }

        return IsMaxEnabled == false || element.IsLessThan(Max) || element.IsEqualTo(Max);
    }

    private bool ElementGreaterOrEqualToMin(T element)
    {
        if (HasCustomComparer)
        {
            return element.IsGreaterThan(Min, _comparer) || element.IsEqualTo(Min, _comparer);
        }

        return IsMinEnabled == false || element.IsGreaterThan(Min) || element.IsEqualTo(Min);
    }
}

public static class ComparableExtensions
{
    public static bool IsInRange<T>(this T element, Range<T> range) where T : struct, IComparable
    {
        return range.Contains(element);
    }

    public static bool IsGreaterThan<T>(this T element, T? other) where T : struct, IComparable
    {
        return element.CompareTo(other) == 1;
    }

    public static bool IsGreaterThan<T>(this T element, T? other, IComparer comparer) where T : struct, IComparable
    {
        return comparer.Compare(element, other) == 1;
    }

    public static bool IsLessThan<T>(this T element, T? other) where T : struct, IComparable
    {
        return element.CompareTo(other) == -1;
    }

    public static bool IsLessThan<T>(this T element, T? other, IComparer comparer) where T : struct, IComparable
    {
        return comparer.Compare(element, other) == -1;
    }

    public static bool IsEqualTo<T>(this T element, T? other) where T : struct, IComparable
    {
        return element.CompareTo(other) == 0;
    }

    public static bool IsEqualTo<T>(this T element, T? other, IComparer comparer) where T : struct, IComparable
    {
        return comparer.Compare(element, other) == 0;
    }
}