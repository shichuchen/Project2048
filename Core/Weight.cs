namespace Project2048
{
    /// <summary>
    /// 负责权重的变化, 需要设置相应的最大最小值,
    /// 保证权重的
    /// </summary>
    public class Weight : NamedDouble
    {
        public Weight(double value, double min, double max) : base(value)
        {
            Min = min;
            Max = max;
        }
        public double Min;
        public double Max;
        /// <summary>
        /// 对该值进行线性插值
        /// </summary>
        /// <param name="interp">扰动因子</param>
        /// <returns>新的Weight参数, 保留最大最小值</returns>
        public Weight SetInterpolation(double interp)
        {
            interp = MathUtility.Clamp(interp, 0, 1);
            return SetDeltaChange(Min - Value + interp * (Max - Min));
        }
        /// <summary>
        /// 直接将原值加上一定的偏移量
        /// </summary>
        /// <param name="deltaChange">偏移量</param>
        /// <returns>新的Weight参数, 保留最大最小值</returns>
        public Weight SetDeltaChange(double deltaChange)
        {
            double rawValue = Value + deltaChange;
            if (rawValue <= Min || rawValue >= Max)
            {
                return FromMinMax(Value - deltaChange);
            }
            return FromMinMax(rawValue);
        }
        /// <summary>
        /// 在给定value的情况下生成新的Weight参数, 保留最大最小值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Weight FromMinMax(double value)
        {
            return new Weight(Clamp(value), Min, Max);
        }
        /// <summary>
        /// 将该值限定在Weight的最大最小值内
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double Clamp(double value)
        {
            return MathUtility.Clamp(value, Min, Max);
        }

    }
}
