using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Comm;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    /// <summary>
    /// 创建人：朱茂琛
    /// 创建时间：2019/06/25
    /// 报销单控制器
    /// </summary>
    public class ReimbursementController : BaseController
    {
        #region  添加/修改报销单
        // GET: Reimbursement
        /// <summary>
        /// modal页面
        /// </summary>
        /// <param name="ID">报销单编号</param>
        /// <returns></returns>
        public ActionResult Index(string ID ="")
        {
            if (ID.IsEmpty())
            {
                ReimbursementEntity reimbursement = new ReimbursementEntity();
                reimbursement.Department = Convert.ToInt32(Session["DepartmentId"].ToString());
                reimbursement.Level = Convert.ToInt32(Session["DutyId"].ToString());
                return View(reimbursement);
            }
            else
            {
                var reimbursement = DALUtility.ReimbursementCore.QryReimbursement<ReimbursementEntity>(ID);
                reimbursement.Level = Convert.ToInt32(Session["DutyId"].ToString());
                return View(reimbursement);
            }
            
        }
        /// <summary>
        /// 添加报销单
        /// </summary>
        /// <param name="reimbursement">报销单</param>
        /// <returns></returns>
        public ActionResult Add(ReimbursementEntity reimbursement)
        {
            return SubmitForm(reimbursement);
        }
        /// <summary>
        /// 修改报销单（主要用于审批功能）
        /// </summary>
        /// <param name="ID">报销单编号</param>
        /// <param name="state">报销单状态</param>
        /// <returns></returns>
        public ActionResult Edit(string ID,int state)
        {

            var reimbursement = DALUtility.ReimbursementCore.QryReimbursement<ReimbursementEntity>(ID);
            if (reimbursement.Manager == null)
            {
                reimbursement.Manager = Session["UserId"].ToString();
            }
            else
            {
                reimbursement.Finace = Session["UserId"].ToString();
            }
            reimbursement.State = state;
            return SubmitForm(reimbursement);
        }
        /// <summary>
        /// 保存/修改方法
        /// </summary>
        /// <param name="reimbursement"></param>
        /// <returns></returns>
        public ActionResult SubmitForm(ReimbursementEntity reimbursement)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            paras["Department"] = reimbursement.Department;
            paras["Money"] = reimbursement.Money;
            paras["Applicant"] = reimbursement.Applicant;
            paras["ApplicantTime"] = reimbursement.ApplicantTime;
            paras["Purpose"] = reimbursement.Purpose;
            paras["Manager"] = reimbursement.Manager;
            paras["Finace"] = reimbursement.Finace;
            paras["State"] = reimbursement.State;
            if (reimbursement.ID != null)
            {
                paras["ID"] = reimbursement.ID;
                paras["ApprovalTime"] = DateTime.Now.Date;
                return OperationReturn(DALUtility.ReimbursementCore.Save(paras) > 0);
            }
            else
            {
                return OperationReturn(DALUtility.ReimbursementCore.Save(paras) > 0);
            }
        }
        #endregion
        #region 查看当前用户能看的所有报销单
        /// <summary>
        /// 查看页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll()
        {
            return View();
        }
        /// <summary>
        /// 查看方法
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public ActionResult GetAllRB(Pagination pagination)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            int departmentId = Convert.ToInt32(Session["DepartmentId"]);
            int dutyId = Convert.ToInt32(Session["DutyId"]);
            if (dutyId > 1 && departmentId != 4)
            {
                paras["Department"] = departmentId;
            }
            else if (dutyId == 1 && departmentId != 4)
            {
                paras["Applicant"] = Session["UserId"].ToString();
            }
            var objRet = DALUtility.ReimbursementCore.QryRei<ReimbursementEntity>(paras, pagination);
            return Content(JsonConvert.SerializeObject(objRet));
        }
        #endregion
        #region 未处理报销单
        public ActionResult GetUntreated()
        {
            return View();
        }
        public ActionResult GetUntreatedRB()
        {
            string userId = Session["UserId"].ToString();
            UserEntity user = DALUtility.UserCore.GetUserByUserId(userId);
            DynamicParameters dp = new DynamicParameters();
            dp.Add("Daprtment", user.DepartmentId);
            dp.Add("Applicant", "");
            if (user.DepartmentId == 4)
            {
                dp.Add("State", 2);
            }
            else if (user.DutyId > 1)
            {
                dp.Add("State", 1);
            }
            else
            {
                dp.Add("Applicant", userId);
                dp.Add("State", 0);
            }
            return Content(DALUtility.ReimbursementCore.QryUntreated(dp));
        }
        #endregion
        #region 删除报销单（主要用于撤回功能）
        public ActionResult Delete(string ID)
        {
            return OperationReturn(DALUtility.ReimbursementCore.Delete(ID) > 0);
        }
        #endregion
    }
}