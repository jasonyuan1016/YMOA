using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace YMOA.Web
{
    [Description("操作类型")]
    public enum OperationType
    {
        /// <summary>
        /// 查看
        /// </summary>
        View = 0,
        /// <summary>
        /// 新增
        /// </summary>
        Add = 1,
        /// <summary>
        /// 修改
        /// </summary>
        Update = 2,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 3,
        /// <summary>
        /// 删除
        /// </summary>
        Other = 4
    };

    public enum DataMainType
    {
        /// <summary>
        /// 系统
        /// </summary>
        System = 1,
        /// <summary>
        /// 用户
        /// </summary>
        User = 2,
        /// <summary>
        /// 报表
        /// </summary>
        Report = 3,
        /// <summary>
        /// 其他
        /// </summary>
        Others = 0
    }
}
