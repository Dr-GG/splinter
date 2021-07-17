using System;
using Microsoft.Data.SqlClient;

namespace Splinter.NanoInstances.Database.Extensions
{
    public static class ExceptionExtensions
    {
        private const int SqlServerPrimaryKeyViolationErrorCode = 2627;
        private const int SqlServerUniqueKeyViolationErrorCode = 2601;

        public static bool IsDuplicateDataException(this Exception error)
        {
            if (error is SqlException sqlError)
            {
                return sqlError.Number == SqlServerPrimaryKeyViolationErrorCode
                       || sqlError.Number == SqlServerUniqueKeyViolationErrorCode;
            }

            return error.InnerException != null
                   && error.InnerException.IsDuplicateDataException();
        }
    }
}
