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
            
            return View(reimbursement);
        }

        public ActionResult SubmitForm(ReimbursementEntity reimbursement)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            paras["Department"] = reimbursement.Department;
            paras["Money"] = reimbursement.Money;
            paras["Applicant"] = reimbursement.Applicant;
            paras["ApplicantTime"] = reimbursement.ApplicantTime;
            paras["ApprovalTime"] = reimbursement.ApprovalTime;
            paras["Purpose"] = reimbursement.Purpose;
            paras["Manager"] = reimbursement.Manager;
            paras["State"] = reimbursement.State;
            paras["Finace"] = reimbursement.Finace;

            if (!reimbursement.ID.Equals(""))
            {
                paras["ID"] = reimbursement.ID;
            }
            return OperationReturn(DALUtility.ReimbursementCore.Save(paras) > 0);
        }
    }
}