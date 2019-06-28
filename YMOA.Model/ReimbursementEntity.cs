using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMOA.Model
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/06/25
    /// 报销单类
    /// </summary>
    public class ReimbursementEntity
    {
        /// <summary>
        /// 报销单编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户所属部门
        /// </summary>
        public int Department { get; set; }
        /// <summary>
        /// 报销用途
        /// </summary>
        public string Purpose { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public double Money { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }
        /// <summary>
        /// 部门管理人员
        /// </summary>
        public string Manager { get; set; }
        /// <summary>
        /// 财务
        /// </summary>
        public string Finace { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime ? ApplicantTime { get; set; }
        /// <summary>
        /// 批准时间
        /// </summary>
        public DateTime ? ApprovalTime { get; set; }

        public int Level;
    }
}
