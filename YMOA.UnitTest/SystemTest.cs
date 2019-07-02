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

        /// <summary>
        /// 测试账号邮箱是否重复
        /// </summary>
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

        /// <summary>
        /// 测试根据id删除角色
        /// </summary>
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
        
        /// <summary>
        /// 获取选单
        /// </summary>
        [TestMethod]
        public void TestMenuGetList()
        {
            IEnumerable<MenuEntity> result = DALCore.GetInstance().SystemCore.MenuGetList<MenuEntity>(null);
            Debug.WriteLine(result);
            Assert.AreNotEqual(result, null);
        }

        /// <summary>
        ///  查询部门主管
        /// </summary>
        [TestMethod]
        public void TestGetCharge()
        {
            string result = DALCore.GetInstance().UserCore.GetCharge(2);
            Assert.AreNotEqual(result, null);
        }

        /// <summary>
        ///  测试设置部门主管(每个部门只存在一个主管)
        /// </summary>
        [TestMethod]
        public void TestSetCharge()
        {
            bool result = DALCore.GetInstance().UserCore.SetCharge(2, "Jason");
            Assert.AreEqual(result, true);
        }

    }
}
