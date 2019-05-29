using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMOA.IDAL;
using YMOA.Model;

namespace YMOA.DAL
{
    /// <summary>
    ///  任务数据访层
    ///  创建者: zxy
    ///  创建时间: 2019年5月29日
    /// </summary>
    public class TaskDAL : BaseDal, ITaskDAL
    {

        // 任务添加/修改
        public int TaskSave(Dictionary<string, object> tasks, DataTable dtTeam, DataTable dtAccessory)
        {
            int result = StandardInsertOrUpdate("tbTask", tasks);
            if (result > 0)
            {
                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras["ProjectId"] = tasks["ProjectId"];
                paras["TaskId"] = tasks["ID"];
                paras["team"] = dtTeam.AsTableValuedParameter();
                paras["accessory"] = dtAccessory.AsTableValuedParameter();
                QuerySingle<int>("P_TeamAndAccessory_SaveBatch", paras, CommandType.StoredProcedure);
            }
            return result;
        }

        // 批量添加

        public int BatchInsert(List<TaskEntity> listTask) {




            return 1;
        }

    }
}
