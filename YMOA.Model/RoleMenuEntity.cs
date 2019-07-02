using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YMOA.Model
{
    /// <summary>
    /// 角色能操作的菜单
    /// </summary>
    public class RoleMenuEntity
    {
        public RoleEntity roleEntity { get; set; }
        public List<AllowOperation> allowOperations { get; set; }
    }

    public class AllowOperation
    {
        /// <summary>
        /// 选单ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 添加操作
        /// </summary>
        public bool add { get; set; }
        /// <summary>
        /// 修改操作
        /// </summary>
        public bool update { get; set; }
        /// <summary>
        /// 删除操作
        /// </summary>
        public bool delete { get; set; }
        /// <summary>
        /// 其他操作
        /// </summary>
        public bool other { get; set; }
    }
}
