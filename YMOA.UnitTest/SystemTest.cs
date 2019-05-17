using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.UnitTest
{
    [TestClass]
    public class SystemTest
    {
        [TestMethod]
        public void TestMethod1()
        {

        }

        [TestMethod]
        public void TestCheckUseridAndEmail()
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars["Email"] = "aabbc221cdd@gmail.com";
            pars["AccountName"] = "admin";
            pars["ID"] = 0;
            int result = DALCore.GetInstance().UserCore.CheckUseridAndEmail(pars);
            Console.WriteLine(result);
            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void TestRoleDelete()
        {
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars["id"] = 1007;
            int result = DALCore.GetInstance().SystemCore.RoleDelete(pars);
            // 0成功 1角色不存在 2角色被用户引用
            Console.WriteLine(result);
            Assert.AreEqual(result,0);
        }
        
        [TestMethod]
        public void TestMenuGetList()
        {
            IEnumerable<MenuEntity> result = DALCore.GetInstance().SystemCore.MenuGetList<MenuEntity>(null);
            Debug.WriteLine(result);
            Assert.AreNotEqual(result, null);
        }
    }
}
