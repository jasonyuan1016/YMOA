using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Model
{
    /// <summary>
    ///  任务实体
    ///  创建者: zxy
    ///  创建时间: 2019年5月29日
    /// </summary>
    public class TaskEntity
    {
        /// <summary>
        ///  任务编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        ///  任务名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        ///  项目编号
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 父级编号
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        ///  截至时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        ///  描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        ///  备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        ///  预计工时
        /// </summary>
        public Decimal Estimate { get; set; }
        /// <summary>
        ///  消耗工时
        /// </summary>
        public Decimal Consume { get; set; }
        /// <summary>
        ///  优先级
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        ///  状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        ///  发送至
        /// </summary>
        public string Send { get; set; }
        /// <summary>
        ///  创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        ///  创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        ///  启动人
        /// </summary>
        public string StartBy { get; set; }
        /// <summary>
        ///  启动时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        ///  完成人
        /// </summary>
        public string FinishBy { get; set; }
        /// <summary>
        ///  完成时间
        /// </summary>
        public DateTime? FinishTime { get; set; }
        /// <summary>
        ///  取消人
        /// </summary>
        public string CancelBy { get; set; }
        /// <summary>
        ///  取消时间
        /// </summary>
        public DateTime? CancelTime { get; set; }
        /// <summary>
        ///  关闭人
        /// </summary>
        public string CloseBy { get; set; }
        /// <summary>
        ///  关闭时间
        /// </summary>
        public DateTime? CloseTime { get; set; }
        /// <summary>
        ///  关闭原因
        /// </summary>
        public string CloseReason { get; set; }
        /// <summary>
        ///  最后修改人
        /// </summary>
        public string UpdateBy { get; set; }
        /// <summary>
        ///  最后修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }

    //public class Task: TaskEntity
    //{

    //}
}
