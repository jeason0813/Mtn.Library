using System;
using System.Diagnostics;
using System.Linq;
using Mtn.Library.Extensions;
namespace Mtn.Library.Mvc3.Controller
{
    /// <summary>
    /// Deprecated, use Mtn.Library.Mvc instead
    /// </summary>
    public class MtnBaseController:System.Web.Mvc.Controller
    {
        #region Attributes
        private Type _mType;
        private readonly Stopwatch _mSwTime = Stopwatch.StartNew();
        private const string HeaderDefaultPrefix = "Mtn-";
        private int counter ;

        #endregion

        #region properties
        /// <summary>
        /// <para>Return this Page Type.</para>
        /// </summary>
        public Type ControllerType
        {
            get { return _mType ?? (_mType = this.GetType()); }
        }
        #endregion

        #region override

        /// <summary>
        /// Initializes data that might not be available when the constructor is called.
        /// </summary>
        /// <param name="requestContext">The HTTP context and route data.</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            this._mSwTime.Start();
            base.Initialize(requestContext);
            this.WriteStatisc("Initialize", this.RouteData.Values.FirstOrDefault().Value.ToString());
        }

        /// <summary>
        /// Invokes the action in the current controller context.
        /// </summary>
        protected override void ExecuteCore()
        {
            //this._mSwTime.Start();
            base.ExecuteCore();
            this.WriteStatisc("ExecuteCore", this.RouteData.Values.FirstOrDefault().Value.ToString());
            
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            this.WriteStatisc("OnActionExecuted", this.RouteData.Values.FirstOrDefault().Value.ToString());
        }

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.WriteStatisc("OnActionExecuting", this.RouteData.Values.FirstOrDefault().Value.ToString());
        }

        /// <summary>
        /// Called when authorization occurs.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            this.WriteStatisc("OnAuthorization", this.RouteData.Values.FirstOrDefault().Value.ToString());
        }

        /// <summary>
        /// Called before the action result that is returned by an action method is executed.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action result</param>
        protected override void OnResultExecuting(System.Web.Mvc.ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            this.WriteStatisc("OnResultExecuting", this.RouteData.Values.FirstOrDefault().Value.ToString());
        }

        /// <summary>
        /// Called after the action result that is returned by an action method is executed.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action result</param>
        protected override void OnResultExecuted(System.Web.Mvc.ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            this.WriteStatisc("OnResultExecuted", this.RouteData.Values.FirstOrDefault().Value.ToString());
            this._mSwTime.Stop();
        }

        /// <summary>
        /// Called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            this.WriteStatisc("OnException", this.RouteData.Values.FirstOrDefault().Value.ToString());
        }

        /// <summary>
        /// Called when a request matches this controller, but no method with the specified action name is found in the controller.
        /// </summary>
        /// <param name="actionName">The name of the attempted action.</param>
        protected override void HandleUnknownAction(string actionName)
        {   
            base.HandleUnknownAction(actionName);
            this.WriteStatisc("UnknownAction", actionName);
        }


        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this._mSwTime.Stop();
            base.Dispose(disposing);
        }
        #endregion

        #region WriteStatisc


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        protected void WriteStatisc(string source, string value)
        {
            counter++;
            if (Mtn.Library.Web.Utils.Parameter.EnableStatistics)
            {
                if (value.IsNullOrWhiteSpaceMtn() == false)
                    Service.Statistics.Add(DateTime.Now.ToString("yyyy-mm-dd HH:mm:ssss =>") + value);
            }
            if (Mtn.Library.Web.Utils.Parameter.EnableHeaderStatistics && !HttpContext.Response.IsRequestBeingRedirected)
            {
                try
                {
                    if (value.IsNullOrWhiteSpaceMtn() == false)
                        HttpContext.Response.AddHeader(HeaderDefaultPrefix + counter.ToString("00") + source, value);
                    HttpContext.Response.AddHeader(HeaderDefaultPrefix + counter.ToString("00") + source + "-Time", string.Format("Time:{0:0.00000} milliseconds", this._mSwTime.Elapsed.TotalMilliseconds));
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(DateTime.Now.ToString("yyyy-mm-dd HH:mm:ssss =>") + value + "-" + ex.GetAllMessages());
                }
            }
        }
        #endregion
    }
}

namespace Mtn.Library.Mvc4.Controller
{
    /// <summary>
    /// Deprecated, use Mtn.Library.Mvc instead
    /// </summary>
    public class MtnBaseController : Mtn.Library.Mvc3.Controller.MtnBaseController
    {
        
    }
}
