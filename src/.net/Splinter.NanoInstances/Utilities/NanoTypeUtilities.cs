using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Utilities;

/// <summary>
/// A collection of utility methods used for Nano Type and Nano Instance ID manipulation.
/// </summary>
public static class NanoTypeUtilities
{
    /// <summary>
    /// Determines if a Type is of an INanoAgent type.
    /// </summary>
    public static bool IsNanoType(Type nanoType)
    {
        return nanoType.IsAssignableTo(typeof(INanoAgent));
    }
    
    /// <summary>
    /// Determines if a Type is a potential Nano Instance.
    /// </summary>
    public static bool IsNanoInstance(Type nanoInstance)
    {
        return IsNanoType(nanoInstance) && 
               nanoInstance is {IsClass: true, IsAbstract: false, IsInterface: false};
    }

    /// <summary>
    /// Gets the Nano Type and Nano Instance ID from a Type.
    /// </summary>
    public static void GetSplinterIds<TNanoType>(
        out SplinterId? nanoTypeId,
        out SplinterId? nanoInstanceId)
    {
        GetSplinterIds(typeof(TNanoType), out nanoTypeId, out nanoInstanceId);
    }

    /// <summary>
    /// Gets the Nano Type and Nano Instance ID from a Type.
    /// </summary>
    public static void GetSplinterIds(
        Type nanoType, 
        out SplinterId? nanoTypeId, 
        out SplinterId? nanoInstanceId)
    {
        var fields = nanoType
            .GetFields(BindingFlags.Public 
                       | BindingFlags.Static
                       | BindingFlags.FlattenHierarchy);

        nanoTypeId = GetSplinterId(nanoType, SplinterIdConstants.NanoTypeIdPropertyName, fields);
        nanoInstanceId = GetSplinterId(nanoType, SplinterIdConstants.NanoInstanceIdPropertyName, fields);
    }
    
    private static SplinterId? GetSplinterId(
        Type nanoType,
        string propertyName,
        IEnumerable<FieldInfo> fields)
    {
        var field = fields.FirstOrDefault(f =>
            f.Name.EqualsOrdinalIgnoreCase(propertyName));

        if (field == null)
        {
            return null;
        }

        return (SplinterId?)field.GetValue(nanoType);
    }
}