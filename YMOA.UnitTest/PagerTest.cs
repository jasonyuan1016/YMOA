using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.Comm;
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
            Dictionary<string, object> pars = new Dictionary<string, object>();
            pars["keyword"] = "Test_";
            Pagination pagination = new Pagination();
            pagination.sidx = "ID";
            pagination.sord = "DESC";
            pagination.rows = 10;
            pagination.page = 1;
            
            var data = new
            {
                rows = DALCore.GetInstance().UserCore.QryUsers<UserEntity>(pagination, pars),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            Assert.AreNotEqual(pagination.records, 0);
        }
    }
}
