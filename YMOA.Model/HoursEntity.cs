using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Model
{
    /// <summary>
    ///  工时表实体类
    ///  作者:zxy
    ///  创建时间:2019年6月19日
    /// </summary>
    public class HoursEntity
    {
        /// <summary>
        ///  编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        ///  项目编号
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        ///  任务编号
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        ///  工时
        /// </summary>
        public Decimal Hour { get; set; }

        /// <summary>
        ///  成员
        /// </summary>
        public string Person { get; set; }

    }
}
