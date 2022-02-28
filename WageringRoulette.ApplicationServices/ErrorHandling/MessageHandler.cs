using System.Globalization;

namespace WageringRoulette.ApplicationServices.ErrorHandling
{
    public static class MessageHandler
    {
        public static class MessageCodes
        {
            public static string NotFound
            {
                get { return "1001"; }
            }
            public static string MoneyOutRange
            {
                get { return "1002"; }
            }
            public static string NotAllowedOpen
            {
                get { return "1003"; }
            }
            public static string NotAllowedClose
            {
                get { return "1004"; }
            }
            public static string RouletteClosed
            {
                get { return "1005"; }
            }
            public static string NotAllowedCloseByOpen
            {
                get { return "1006"; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageCode"></param>
        /// <returns></returns>
        public static string GetErrorDescription(string messageCode)
        {
            return MessageResource.ResourceManager.GetString(messageCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageCode"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetErrorDescription(string messageCode, params object[] parameters)
        {
            return string.Format(CultureInfo.InvariantCulture, MessageResource.ResourceManager.GetString(messageCode), parameters);
        }
    }
}
