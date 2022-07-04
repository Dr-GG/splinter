using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Splinter.NanoTypes.Domain.Constants;
using Splinter.NanoTypes.Domain.Core;
using Splinter.NanoTypes.Interfaces.Agents.NanoAgents;
using Tenjin.Extensions;

namespace Splinter.NanoInstances.Utilities;

public static class NanoTypeUtilities
{
    public static bool IsNanoType(Type nanoType)
    {
        return nanoType.IsAssignableTo(typeof(INanoAgent));
    }

    public static bool IsNanoInstance(Type nanoInstance)
    {
        return IsNanoType(nanoInstance)
               && nanoInstance.IsClass
               && !nanoInstance.IsAbstract
               && !nanoInstance.IsInterface;
    }

    public static void GetSplinterIds<TNanoType>(
        out SplinterId? nanoTypeId,
        out SplinterId? nanoInstanceId)
    {
        GetSplinterIds(typeof(TNanoType), out nanoTypeId, out nanoInstanceId);
    }

    public static void GetSplinterIds(
        Type nanoType, 
        out SplinterId? nanoTypeId, 
        out SplinterId? nanoInstanceId)
    {
        var fields = nanoType
            .GetFields(BindingFlags.Public 
                       | BindingFlags.Static
                       | BindingFlags.FlattenHierarchy);

        nanoTypeId = GetSplinterId(nanoType, SplinterIdConstants.NanoTypeId, fields);
        nanoInstanceId = GetSplinterId(nanoType, SplinterIdConstants.NanoInstanceId, fields);
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