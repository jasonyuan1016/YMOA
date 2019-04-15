using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.UnitTest
{
    [TestClass]
    public class PagerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var paras = new Dictionary<string, object>();
            paras["pi"] = 1;
            paras["pageSize"] = 20;
            paras["userid"] = "admin";
            int iCount = 0;
            var userList = DALCore.GetInstance().User.QryUsers<UserEntity>(paras, out iCount);
            Assert.AreNotEqual(iCount, 0);
        }
    }
}
