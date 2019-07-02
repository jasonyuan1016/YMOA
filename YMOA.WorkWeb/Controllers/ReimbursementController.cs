using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YMOA.Model;

namespace YMOA.WorkWeb.Controllers
{
    public class ReimbursementController : BaseController
    {
        // GET: Reimbursement
        public ActionResult Index()
        {
            string userId = Session["UserId"].ToString();
            UserEntity user = DALUtility.UserCore.GetUserByUserId(userId);
            ReimbursementEntity reimbursement = new ReimbursementEntity();
            reimbursement.Department = user.DepartmentId;
            reimbursement.Level = user.DutyId;
            return View(reimbursement);
        }
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
        public ActionResult Add(ReimbursementEntity reimbursement)
        {
            return SubmitForm(reimbursement);
        }
        public ActionResult Edit(string ID,int state)
        {

            var reimbursement = DALUtility.ReimbursementCore.QryReimbursement<ReimbursementEntity>(ID);
            if (reimbursement.Manager == null)
            {
                reimbursement.State = state;
                reimbursement.Manager = Session["UserId"].ToString();
            }
            else
            {
                reimbursement.State = state;
                reimbursement.Finace = Session["UserId"].ToString();
            }
            return SubmitForm(reimbursement);
        }
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
    }
}