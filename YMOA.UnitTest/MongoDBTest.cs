using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.Model;
using YMOA.MongoDB;
using YMOA.Comm;
using System.Collections.Generic;

namespace YMOA.UnitTest
{
    [TestClass]
    public class MongoDBTest
    {
        MongoDbService dbService = new MongoDbService();

        [TestMethod]
        public void AddTest()
        {
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
        }

        /// <summary>
        /// 请求
        /// </summary>
        public void QryDBLogs()
        {
            var retData = dbService.List<DBLogEntity>("YMOA", "DBLog", x => x.tId == "1" && x.tabName == "tbUser", null, 1, false, x => x.ctime);
        }

        /// <summary>
        /// 公告测试
        /// </summary>
        [TestMethod]
        public void MsgTest()
        {
            //添加测试数据
            MsgAdd();
            //读取未读
            //会员ag1user1 代理ag1
            var c1 = MsgUnReadCount("ag1user1", "ag1");
            //会员ag4user1 代理ag4
            var c2 = MsgUnReadCount("ag4user1", "ag4");
            Assert.AreEqual(c1, 2);
            Assert.AreEqual(c2, 1);

            //读取当前会员公告
            var c1Msgs = ReadMsgs("ag1");
            var c1MsgStatus = dbService.List<MsgState>("YMOA", "msg_state", x => x.uid.Equals("ag1user1") && x.state < 2, null, null);
            var c2Msgs = ReadMsgs("ag4");
            var c2MsgStatus = dbService.List<MsgState>("YMOA", "msg_state", x => x.uid.Equals("ag4user1") && x.state < 2, null, null);
            //将未读公告标识为已读
            foreach (var _m in c1Msgs)
            {
                if (c1MsgStatus.Find(x => x.msgid.Equals(_m._id) && x.uid.Equals("ag1user1")) == null)
                {
                    dbService.Add<MsgState>("YMOA", "msg_state", new MsgState() { uid = "ag1user1", msgid = _m._id, state = 1 });
                }
            }
            foreach (var _m in c2Msgs)
            {
                if (c1MsgStatus.Find(x => x.msgid.Equals(_m._id) && x.uid.Equals("ag4user1")) == null)
                {
                    dbService.Add<MsgState>("YMOA", "msg_state", new MsgState() { uid = "ag4user1", msgid = _m._id, state = 1 });
                }
            }
            //读取未读
            //会员ag1user1 代理ag1
            c1 = MsgUnReadCount("ag1user1", "ag1");
            //会员ag4user1 代理ag4
            c2 = MsgUnReadCount("ag4user1", "ag4");
            Assert.AreEqual(c1, 0);
            Assert.AreEqual(c2, 0);
            //自行清空YMOA =》msg，msg_state内容，方便下次测试
        }

        void MsgAdd()
        {
            MsgEntity msgEntity = new MsgEntity();
            msgEntity.content = "公告测试1,to ag1 ag2 ag3";
            msgEntity.starttime = DateTime.Now;
            msgEntity.endtime = msgEntity.starttime.AddDays(10);
            msgEntity.receiver = new List<string>() { "ag1", "ag2", "ag3" };
            dbService.Add<MsgEntity>("YMOA", "msg", msgEntity);

            msgEntity = new MsgEntity();
            msgEntity.content = "公告测试2,to all agent";
            msgEntity.starttime = DateTime.Now;
            msgEntity.endtime = msgEntity.starttime.AddDays(10);
            dbService.Add<MsgEntity>("YMOA", "msg", msgEntity);

            msgEntity = new MsgEntity();
            msgEntity.content = "公告测试3,to ag2";
            msgEntity.starttime = DateTime.Now;
            msgEntity.endtime = msgEntity.starttime.AddDays(10);
            msgEntity.receiver = new List<string>() { "ag2"};
            dbService.Add<MsgEntity>("YMOA", "msg", msgEntity);
        }

        /// <summary>
        /// 返回未读笔数
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="agId"></param>
        /// <returns></returns>
        long MsgUnReadCount(string uid, string agId)
        {
            DateTime dtNow = DateTime.Now;
            var msgStatus = dbService.List<MsgState>("YMOA", "msg_state", x => x.uid.Equals(uid) && x.state < 2, x => new MsgState() { msgid = x.msgid }, null);
            List<string> listMsgId = new List<string>();
            foreach (var ms in msgStatus)
            {
                listMsgId.Add(ms.msgid);
            }
            return dbService.GetCount<MsgEntity>("YMOA", "msg", x => x.starttime < dtNow && x.endtime > dtNow && (x.receiver == null || x.receiver.Contains(agId)) && !listMsgId.Contains(x._id));
        }

        List<MsgEntity> ReadMsgs(string agId)
        {
            DateTime dtNow = DateTime.Now;
            var msgs = dbService.List<MsgEntity>("YMOA", "msg", x => x.starttime < dtNow && x.endtime > dtNow && (x.receiver == null || x.receiver.Contains(agId)), null, null);
            return msgs;
        }
    }

    public class MsgEntity
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        public string _id { get; set; } = Guid.NewGuid().To16String();
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime starttime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endtime { get; set; }
        /// <summary>
        /// 接收方
        /// </summary>
        public List<string> receiver { get; set; }
    }

    public class MsgState
    {
        public string _id { get; set; } = Guid.NewGuid().To16String();
        /// <summary>
        /// 公告ID
        /// </summary>
        public string msgid { get; set; }
        /// <summary>
        /// 会员账号
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// 状态[1已读 2删除]
        /// </summary>
        public int state { get; set; }
    }
}
