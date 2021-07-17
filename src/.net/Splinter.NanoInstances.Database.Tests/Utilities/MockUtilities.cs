using System;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Splinter.NanoInstances.Database.Tests.Utilities
{
    public static class MockUtilities
    {
        private static T ConstructFromInternalConstructor<T>(params object?[] parameters)
        {
            var constructors  = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            return (T)constructors
                .First(ctor => ctor.GetParameters().Length == parameters.Length)
                .Invoke(parameters);
        }

        public static SqlException MockSqlException(int number)
        {
            var error = ConstructFromInternalConstructor<SqlError>(
                number, (byte)0, (byte)0, string.Empty, string.Empty, string.Empty, 0, null);
            var errorCollection = ConstructFromInternalConstructor<SqlErrorCollection>();
            var addErrorMethod = typeof(SqlErrorCollection)
                .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);

            if (addErrorMethod == null)
            {
                throw new InvalidOperationException("Could not find all methods to create SqlException");
            }

            addErrorMethod.Invoke(errorCollection, new object?[] {error});

            return ConstructFromInternalConstructor<SqlException>(
                string.Empty, errorCollection, null, Guid.NewGuid());
        }
    }
}
