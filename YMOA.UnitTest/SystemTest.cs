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
    }
}
