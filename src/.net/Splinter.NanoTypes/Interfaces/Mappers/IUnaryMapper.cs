namespace Splinter.NanoTypes.Interfaces.Mappers
{
    public interface IUnaryMapper<in TSource, out TDestination>
    {
        TDestination Map(TSource source);
    }
}
