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
        public Dictionary<string, object> departments { get; set; }

        public List<MenuPermission> menuPermissions { get; set; }
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