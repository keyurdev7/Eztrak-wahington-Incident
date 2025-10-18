namespace Web.Extensions
{
    public static class NumberExtension
    {
        public static float RoundFloat(this float value, int roundTo = 2)
        {
            return (float)Math.Round(value, roundTo);
        }
        public static double RoundDouble(this double value, int roundTo = 2)
        {
            return Math.Round(value, roundTo);
        }
        public static string GetFormatedQty(this int qty) => string.Format("{0:N0}", (object)qty);
        public static string GetFormatedQty(this long qty) => string.Format("{0:N0}", (object)qty);
    }
}
