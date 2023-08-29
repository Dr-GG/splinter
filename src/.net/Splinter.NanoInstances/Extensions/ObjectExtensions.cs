using System;
using Splinter.NanoTypes.Domain.Exceptions;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of static methods for extensions on objects commonly used in Splinter.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines if an object is null.
    /// If it is null, it then throws the specified exception.
    /// If the value is not null, the value is returned.
    /// </summary>
    public static TValue AssertReturnGetterValue<TValue, TException>(this TValue? value)
        where TValue : class
        where TException : SplinterException, new()
    {
        return value.AssertReturnGetterValue (() => new TException());
    }

    /// <summary>
    /// Determines if an object is null.
    /// If it is null, it then throws the specified exception.
    /// If the value is not null, the value is returned.
    /// </summary>
    public static TValue AssertReturnGetterValue<TValue, TException>(this TValue? value, Func<TException> exceptionDelegate)
        where TValue : class
        where TException : SplinterException
    {
        if (value == null)
        {
            throw exceptionDelegate();
        }

        return value;
    }

    /// <summary>
    /// Determines if an object should be returned if a predicate is true.
    /// If the predicate is false, it then throws the specified exception.
    /// If the predicate is true, the value is returned.
    /// </summary>
    public static TValue AssertReturnGetterValue<TValue, TException>(
        this object _,
        Func<bool> assertPredicate,
        Func<TValue> valuePredicate) where TException : SplinterException, new()
    {
        if (assertPredicate())
        {
            return valuePredicate();
        }

        throw new TException();
    }
}
