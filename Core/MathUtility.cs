namespace Project2048.Core
{
    public static class MathUtility
    {
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            return value > max ? max : value;
        }
    }
}
