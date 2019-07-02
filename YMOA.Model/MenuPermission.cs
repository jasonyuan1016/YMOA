using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Model
{
    /// <summary>
    /// 对应角色的选单权限
    /// </summary>
    public class MenuPermission
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 控制器
        /// </summary>
        public string controller { get; set; }

        /// <summary>
        /// 动作
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// 添加权限
        /// </summary>
        public bool add { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public int parentid { get; set; }

        /// <summary>
        /// 修改权限
        /// </summary>
        public bool update { get; set; }

        /// <summary>
        /// 删除权限
        /// </summary>
        public bool delete { get; set; }

        /// <summary>
        /// 其他权限
        /// </summary>
        public bool other { get; set; }
    }
}
