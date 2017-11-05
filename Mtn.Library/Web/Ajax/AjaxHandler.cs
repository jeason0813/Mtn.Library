using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Web.Routing;
using Mtn.Library.Web.Enums;

namespace Mtn.Library.Web.Ajax
{
    /// <summary>
    /// <para>eng -Handler to ajax code.</para>
    /// </summary>
    public class AjaxHandler : IHttpHandler, IRequiresSessionState, IRouteHandler
    {

        #region atributtes
     
        private static List<string> Errors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected static WebModeType webmodeType = WebModeType.AjaxHandler;
        #endregion

       
        #region ProcessRequest
        /// <summary>
        /// <para>Process the requests.</para>
        /// </summary>
        /// <param name="context">
        /// <para>HttpContext.</para>
        /// </param>
        public virtual void ProcessRequest(HttpContext context)
        {
            string url = context.Request.RawUrl.ToLowerInvariant();
            if (AjaxEngine.WriteScript(url, context, webmodeType))
            {
                return;
            }

            AjaxEngine.ProcessRequest(context, webmodeType);
        }

        #endregion


        #region IHttpHandler Members
        /// <summary>
        /// <para>Indicates if is reusable.</para>
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        #endregion


        #region IRouteHandler Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            //RouteTable.Routes.Add(
            var newAjaxHandler = new AjaxHandler();
            return newAjaxHandler;
        }

        #endregion
    }
}