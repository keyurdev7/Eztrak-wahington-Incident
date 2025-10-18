namespace Web.Extensions
{
    public static class NumberExtension
    {
        public static float RoundFloat(this float value, int roundTo = 2)
        {
            return (float)Math.Round(value, roundTo);
        }
    }
}
