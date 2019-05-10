/*-------------------------------------*
 * 創建人:         J.Y
 * 創建時間:       2019/04/28
 * 最后修改時間:    
 * 最后修改原因:
 * 修改歷史:
 * 2019/04/28       J.Y       創建
 *-------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace YMOA.Web
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class OperationLogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 具体操作
        /// </summary>
        private string operate { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        private DataMainType dataMainType { get; set; }

        private ActionExecutingContext actionExecutingContext { get; set; }

        private ResultExecutedContext resultExecutedContext { get; set; }

        private string testKey { get; set; }
        /// <summary>
        /// 操作日志
        /// </summary>
        /// <param name="_operate">具体操作</param>
        /// <param name="_dataMainType">操作类型</param>
        public OperationLogAttribute(string _operate, DataMainType _dataMainType = DataMainType.Others)
        {
            operate = _operate;
            dataMainType = _dataMainType;
        }

        /// <summary>
        /// 执行操作方法之前
        /// </summary>
        /// <param name="_filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext _filterContext)
        {
            actionExecutingContext = _filterContext;
            base.OnActionExecuting(_filterContext);
        }

        /// <summary>
        /// 在执行操作结果后
        /// </summary>
        /// <param name="_filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext _filterContext)
        {
            resultExecutedContext = _filterContext;
            base.OnResultExecuted(_filterContext);
        }
    }
}