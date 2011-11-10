namespace Icodeon.Hotwire.Framework.Utils
{
    public static class DateTimeExtensions
    {
        public static string ToUTC(this IDateTime dateTime)
        {
            return string.Format("{0} {1} (UTC)",
                                 dateTime.Now.ToUniversalTime().ToShortDateString(),
                                 dateTime.Now.ToUniversalTime().ToLongTimeString());
        }

        public static string ToUtcTime(this IDateTime dateTime)
        {
            return dateTime.Now.ToUniversalTime().ToLongTimeString();
        }

        public static string ToUtcDateTime(this IDateTime dateTime)
        {
            return string.Format("{0} {1}",
                                 dateTime.Now.ToUniversalTime().ToShortDateString(),
                                 dateTime.Now.ToUniversalTime().ToLongTimeString());
        }
    }
}
