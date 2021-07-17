namespace Splinter.NanoTypes.Interfaces.Mappers
{
    public interface IBinaryMapper<TLeft, TRight>
    {
        TLeft Map(TRight source);
        TRight Map(TLeft source);
    }
}
