using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryCurrency.Crawler.Common
{
    public static class CommonExtension
    {
        public static string OnlyNumbers(this string value)
        {
            IReadOnlyCollection<char> array = value.ToCharArray();

            char[] resultArray = array.Where(ar => (char.IsLetterOrDigit(ar)
                                                || char.IsWhiteSpace(ar)
                                                || ar == '-'))
                                      .ToArray();

            return new string(resultArray);
        }
        public static short ToInt16(this string value)
        {
            if (!string.IsNullOrEmpty(value))
                if (short.TryParse(value.Trim(), out short result))
                    return result;

            return 0;
        }
        public static int ToInt32(this string value)
        {
            if (!string.IsNullOrEmpty(value))
                if (int.TryParse(value.Trim(), out int result))
                    return result;

            return 0;
        }
        public static Task<string> ToStringUTF8(this Task<byte[]> task) 
        {
            if (task == null)
                return null;

            return task.ContinueWith(tsk =>
            {
                if (tsk.IsCompletedSuccessfully)
                    return Encoding.UTF8.GetString(tsk.Result);

                return string.Empty;
            });
        }
    }
}
