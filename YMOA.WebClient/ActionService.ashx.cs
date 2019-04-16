using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YMOA.WebClient
{
    /// <summary>
    /// Login 的摘要说明
    /// </summary>
    public class ActionService : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            result_base retData = new result_base();
            string action = context.Request.QueryString["act"];  //请求方法
            switch (action)
            {
                case "login":
                    //context.Request.Form.AllKeys      //请求参数
                    retData.result = 0;
                    break;

            }

            context.Response.Write(JsonConvert.SerializeObject(retData));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public class result_base
    {
        public string errorCode { get; set; } = "";
        public string errorMsg { get; set; } = "";
        public object result { get; set; }
    }
}