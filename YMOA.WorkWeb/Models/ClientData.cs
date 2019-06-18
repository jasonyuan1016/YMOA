using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YMOA.Model;

namespace YMOA.WorkWeb.Models
{
    public class ClientData
    {
        public Dictionary<string, object> groups { get; set; }
        public List<LibraryEntity> departments { get; set; }
        public List<MenuPermission> menuPermissions { get; set; }
        public List<MenuEntity> menus { get; set; }
        /// <summary>
        ///  项目状态
        /// </summary>
        public List<LibraryEntity> projectStatus { get; set; }
        /// <summary>
        ///  任务状态
        /// </summary>
        public List<LibraryEntity> taskStatus { get; set; }
        /// <summary>
        ///  优先级
        /// </summary>
        public List<LibraryEntity> prioritys { get; set; }

        /// <summary>
        ///  用户任务
        /// </summary>
        public List<TaskEntity> tasks { get; set; }
        /// <summary>
        /// 所有用户
        /// </summary>
        public List<UserEntity> users { get; set; }
    }

    public class group
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public int state { get; set; }
    }

    public class department
    {
        public int id { get; set; }
        public string DepartmentName { get; set; }
    }

}