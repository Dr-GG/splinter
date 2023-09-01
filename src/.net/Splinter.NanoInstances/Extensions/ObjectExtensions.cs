using System;
using Splinter.NanoTypes.Domain.Exceptions;
using Splinter.NanoTypes.Domain.Exceptions.Services;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;

namespace Splinter.NanoInstances.Extensions;

/// <summary>
/// A collection of static methods for extensions on objects commonly used in Splinter.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Determines if an INanoType instance is null.
    /// If it is null, it then throws the NanoServiceNotInitialisedException.
    /// If the value is not null, the value is returned.
    /// </summary>
    public static TValue AssertNanoTypeReturnGetterValue<TValue>(this TValue? value)
        where TValue : class, INanoAgent
    {
        return value.AssertReturnGetterValue(() => new NanoTypeNotInitialiseException<TValue>());
    }

    /// <summary>
    /// Determines if a nano-based service is null.
    /// If it is null, it then throws the NanoServiceNotInitialisedException.
    /// If the value is not null, the value is returned.
    /// </summary>
    public static TValue AssertNanoServiceReturnGetterValue<TValue>(this TValue? value)
        where TValue : class
    {
        return value.AssertReturnGetterValue(() => new NanoServiceNotInitialiseException<TValue>());
    }

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
        return _.AssertReturnGetterValue(assertPredicate, valuePredicate, () => new TException());
    }

    /// <summary>
    /// Determines if an object should be returned if a predicate is true.
    /// If the predicate is false, it then throws the specified exception.
    /// If the predicate is true, the value is returned.
    /// </summary>
    public static TValue AssertReturnGetterValue<TValue, TException>(
        this object _,
        Func<bool> assertPredicate,
        Func<TValue> valuePredicate,
        Func<TException> exceptionDelegate) where TException : SplinterException, new()
    {
        if (assertPredicate())
        {
            return valuePredicate();
        }

        throw exceptionDelegate();
    }
}
