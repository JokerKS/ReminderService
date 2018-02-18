using System;

namespace VKozenko.ReminderClient.Tools
{
    internal static class Extensions
    {
        #region NotNull()
        internal static bool NotNull(this object obj)
        {
            return obj == null ? false : true;
        } 
        #endregion

        #region FirstLetterToUpper()
        /// <summary>
        /// Returns the input string with the first character converted to uppercase
        /// </summary>
        public static string FirstLetterToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        } 
        #endregion
    }
}
