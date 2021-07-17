using System.Collections.Generic;
using System.Linq;

namespace Splinter.Common.Extensions
{
    public static class MapperExtensions
    {
        public static TDestination? MapNullable<TSource, TDestination>(
            this IUnaryMapper<TSource, TDestination> mapper,
            TSource? source) 
                where TSource : class 
                where TDestination : class
        {
            return source == null 
                ? null 
                : mapper.Map(source);
        }

        public static TLeft? MapNullable<TLeft, TRight>(
            this IBinaryMapper<TLeft, TRight> mapper,
            TRight? source)
            where TRight : class
            where TLeft : class
        {
            return source == null
                ? null
                : mapper.Map(source);
        }

        public static TRight? MapNullable<TLeft, TRight>(
            this IBinaryMapper<TLeft, TRight> mapper,
            TLeft? source)
            where TRight : class
            where TLeft : class
        {
            return source == null
                ? null
                : mapper.Map(source);
        }

        public static IEnumerable<TDestination> MapCollection<TSource, TDestination>(
            this IUnaryMapper<TSource, TDestination> mapper,
            IEnumerable<TSource>? source)
        {
            return source?
                       .Select(mapper.Map)
                       .ToList()
                   ?? Enumerable.Empty<TDestination>();
        }

        public static IEnumerable<TLeft> MapCollection<TLeft, TRight>(
            this IBinaryMapper<TLeft, TRight> mapper,
            IEnumerable<TRight>? source)
        {
            return source?
                       .Select(mapper.Map)
                       .ToList()
                   ?? Enumerable.Empty<TLeft>();
        }

        public static IEnumerable<TRight> MapCollection<TLeft, TRight>(
            this IBinaryMapper<TLeft, TRight> mapper,
            IEnumerable<TLeft>? source)
        {
            return source?
                       .Select(mapper.Map)
                       .ToList()
                   ?? Enumerable.Empty<TRight>();
        }
    }
}
