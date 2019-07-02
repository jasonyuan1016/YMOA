using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Model
{
    /// <summary>
    ///  成员实体类
    /// </summary>
    public class TeamEntity
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 成员账号
        /// </summary>
        public string Person { get; set; }
    }
}
