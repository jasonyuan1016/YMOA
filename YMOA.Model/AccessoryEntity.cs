using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.Comm;

namespace YMOA.Model
{

    /// <summary>
    ///  附件实体
    ///  创建者: zxy
    ///  创建时间: 2019年5月29日
    /// </summary>
    public class AccessoryEntity
    {
        /// <summary>
        ///  附件编号
        /// </summary>
        public string ID { get; set; } = Guid.NewGuid().To16String();
        /// <summary>
        ///  标题
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  任务编号
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string AccessoryUrl { get; set; }
    }
}
