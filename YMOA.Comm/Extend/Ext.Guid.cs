using System;

namespace YMOA.Comm
{
    public static partial class Ext
    {
        /// <summary>
        /// 生成16位GUID
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static string To16String(this Guid g)
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
