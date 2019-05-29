using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Model
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/05/29
    /// 项目基本信息类
    /// </summary>
    public class ProjectEntity
    {
        //项目编号
        public string ID { get; set; }
        //项目名称
        public string Name { get; set; }
        //项目开始日期
        public DateTime StartTime { get; set; }
        //项目结束日期
        public DateTime EndTime { get; set; }
        //项目描述
        public string Describe { get; set; }
        //项目参观者
        public string Victors { get; set; }
        //项目创建人
        public string CreateBy { get; set; }
        //项目创建时间
        public DateTime CreateTime { get; set; }
        //项目负责人
        public string DutyPerson { get; set; }
        //项目备注
        public string Remarks { get; set; }
        //项目状态
        public int State { get; set; }
        //项目团队
        public List<TeamEntity> Teams { get; set; }
    }
}
