using Splinter.NanoTypes.Interfaces.Mappers;

namespace Splinter.NanoInstances.Mappers
{
    public class BinaryMapper<TLeft, TRight>: IBinaryMapper<TLeft, TRight>
    {
        private readonly IUnaryMapper<TLeft, TRight> _leftToRightMapper;
        private readonly IUnaryMapper<TRight, TLeft> _rightToLeftMapper;

        public BinaryMapper(
            IUnaryMapper<TLeft, TRight> leftToRightMapper, 
            IUnaryMapper<TRight, TLeft> rightToLeftMapper)
        {
            _leftToRightMapper = leftToRightMapper;
            _rightToLeftMapper = rightToLeftMapper;
        }

        public TLeft Map(TRight source)
        {
            return _rightToLeftMapper.Map(source);
        }

        public TRight Map(TLeft source)
        {
            return _leftToRightMapper.Map(source);
        }
    }
}
