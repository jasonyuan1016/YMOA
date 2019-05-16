using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.Model;
using YMOA.MongoDB;
using YMOA.Comm;

namespace YMOA.UnitTest
{
    [TestClass]
    public class MongoDBTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var dbService = new MongoDbService();
            for (int i = 0; i < 10; i++)
            {
                DBLogEntity entity = new DBLogEntity();
                entity.tabName = "tbUser";
                entity.tId = "1";
                entity.lType = 2;
                entity.sql = "";
                entity.paras = "";
                entity.ms = 10;
                entity.uId = "user" + (i + 1);
                entity.ctime = DateTime.Now;
                dbService.Add<DBLogEntity>("YMOA", "DBLog", entity);
            }
            

            var retData = dbService.List<DBLogEntity>("YMOA", "DBLog", x => x.tId == "1" && x.tabName == "tbUser", null, 1, false, x => x.ctime);
        }
    }

    
}
