using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace YMOA.Comm
{
    public class CultureHelper
    {
        public static readonly List<string> validCultures = new List<string>() { "zh-cn", "en-us" };
        public static readonly List<string> cultures = new List<string>() { "zh-cn", "en-us" };

        public static bool IsRightToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
        }

        public static string GetImplementedCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GetDefaultCulture();
            }
            if (!validCultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                return GetDefaultCulture();
            }
            if (cultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                return name;
            }
            var n = GetNeutralCulture(name);
            foreach (var c in cultures)
            {
                if (c.StartsWith(n))
                {
                    return c;
                }
            }
            return GetDefaultCulture();
        }

        public static string GetDefaultCulture()
        {
            return cultures[0];
        }

        public static string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        public static string GetCurrentNeutralCulture()
        {
            return GetNeutralCulture(Thread.CurrentThread.CurrentCulture.Name);
        }

        public static string GetNeutralCulture(string name)
        {
            if (!name.Contains("-"))
            {
                return name;
            }
            else
            {
                return name.Split('-')[0];
            }
        }
    }
}
