using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.DALFactory;
using YMOA.Model;
using YMOA.WorkWeb.Models;

namespace YMOA.WorkWeb.Controllers
{
    public class ClientsDataController : Controller
    {
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetClientsDataJson()
        {
            var data = new ClientData();
            var groups = new List<group>();
            var departments = new List<department>();
            var menuPermissions = new List<MenuPermission>();
            DALCore.GetInstance().User.GetClientData<group, department, MenuPermission>(1, ref groups, ref departments, ref menuPermissions);
            Dictionary<string, object> dictionaryItem = new Dictionary<string, object>();
            foreach (var itm in groups)
            {
                dictionaryItem.Add(itm.id.ToString(), itm.name);
            }
            data.groups = dictionaryItem;
            dictionaryItem = new Dictionary<string, object>();
            foreach (var itm in departments)
            {
                dictionaryItem.Add(itm.id.ToString(), itm.DepartmentName);
            }
            data.departments = dictionaryItem;
            data.menuPermissions = menuPermissions;
            return Content(data.ToJson());
        }
    }
}
