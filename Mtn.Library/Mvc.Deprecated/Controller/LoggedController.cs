﻿using System;
using System.Linq;
using System.Web.Mvc;
using Mtn.Library.Entities;
using Mtn.Library.Extensions;

namespace Mtn.Library.Mvc3.Controller
{
    /// <summary>
    /// Deprecated, use Mtn.Library.Mvc.Controller Instead
    /// </summary>
    public class MtnLoggedController : MtnBaseController
    {
        /// <summary>
        /// If wanna change for a especific page the default login error message
        /// </summary>
        protected String LoginErrorMessage { get; set; }
        /// <summary>
        /// If wanna change for a especific page the default permission denied message
        /// </summary>
        protected String PermissionErrorMessage { get; set; }

        /// <summary>
        /// Ticket entity
        /// </summary>
        protected Ticket Ticket { get; set; }

        private bool _supressContent;
        private dynamic _ticket = null;
        /// <summary>
        /// Returns ticket
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetTicket<T>()
        {
            return (T)_ticket;
        }
        /// <summary>
        /// Set ticket information
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ticket"></param>
        protected void SetTicket<T>(T ticket)
        {
            _ticket = ticket;
        }
        #region override and supress code

        /// <summary>
        /// Creates a content result object by using a string, the content type, and content encoding.
        /// </summary>
        /// <returns>
        /// The content result instance.
        /// </returns>
        /// <param name="content">The content to write to the response.</param><param name="contentType">The content type (MIME type).</param><param name="contentEncoding">The content encoding.</param>
        protected override System.Web.Mvc.ContentResult Content(string content, string contentType, System.Text.Encoding contentEncoding)
        {
            if (_supressContent)
                return new ContentResult();
            return base.Content(content, contentType, contentEncoding);
        }

        /// <summary>
        /// Invokes the action in the current controller context.
        /// </summary>
        protected override void ExecuteCore()
        {
            if (_supressContent == false)
            base.ExecuteCore();
        }

        /// <summary>
        /// Executes the specified request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param><exception cref="T:System.ArgumentNullException">The <paramref name="requestContext"/> parameter is null.</exception>
        protected override void Execute(System.Web.Routing.RequestContext requestContext)
        {
            if (_supressContent == false)
                base.Execute(requestContext);
        }

        /// <summary>
        /// Called after the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (_supressContent == false)
                base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Called when authorization occurs.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (_supressContent == false)
                base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// Called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (_supressContent == false)
                base.OnException(filterContext);
        }

        /// <summary>
        /// Called after the action result that is returned by an action method is executed.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action result</param>
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (_supressContent == false)
                base.OnResultExecuted(filterContext);
        }

        /// <summary>
        /// Called before the action result that is returned by an action method is executed.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action result</param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (_supressContent == false)
                base.OnResultExecuting(filterContext);
        }
        #endregion

        /// <summary>
        /// Initializes data that might not be available when the constructor is called.
        /// </summary>
        /// <param name="requestContext">The HTTP context and route data.</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Library.Attributes.Authorization.ValidateAction != null && Library.Attributes.Authorization.ValidateAction() == false)
            {
                if (Mtn.Library.Web.Utils.Parameter.UnauthorizedPage.IsNullOrWhiteSpaceMtn() == false)
                {
                    _supressContent = true;
                    Response.Redirect(Mtn.Library.Web.Utils.Parameter.UnauthorizedPage, true);
                }
                else
                {
                    string msg = LoginErrorMessage;
                    if (msg.IsNullOrWhiteSpaceMtn())
                        msg = (string)Library.Attributes.Authorization.UnauthorizedValue;

                    if (msg.IsNullOrEmptyMtn())
                        msg = new DataPage<string>().GetUnauthorizedPage().Message;

                    if (msg != null)
                    {
                        WriteStatisc("LoginCheck", msg);
                        Response.Write(msg);
                    }
                    else
                    {
                        WriteStatisc("LoginCheck", "You need login first!");
                    }
                    _supressContent = true;

                    return;
                }
            }
            
            // Check permissions
            var attrs = GetType().GetCustomAttributes(typeof(Library.Attributes.Permission), true);
            if (attrs.Any())
            {
                foreach (var attrP in 
                    attrs.Select(attr => (attr as Library.Attributes.Permission)).
                    Where(attrP => attrP != null && attrP.HasPermission(attrP.Value1, attrP.Value2, attrP.Value3, attrP.Value4) == false))
                {
                    if (Mtn.Library.Web.Utils.Parameter.UnallowedPage.IsNullOrWhiteSpaceMtn())
                    {
                        string msg = PermissionErrorMessage;
                        if (msg.IsNullOrWhiteSpaceMtn())
                            msg = (string)Library.Attributes.Permission.UnallowedResult;

                        if (msg.IsNullOrWhiteSpaceMtn())
                            msg = new DataPage<string>().GetUnallowedPage().Message;

                        if (msg.IsNullOrWhiteSpaceMtn())
                        {
                            WriteStatisc("PermissionCheck", "You don't have permission to see this page/action!");
                        }
                        else
                        {
                            WriteStatisc("PermissionCheck", msg);
                            Response.Write(msg);
                        }
                        _supressContent = true;
                        
                    }
                    else
                    {
                        WriteStatisc("PermissionCheck", string.Format("Redirecting to -> {0}", Mtn.Library.Web.Utils.Parameter.UnallowedPage));
                        _supressContent = true;
                        Response.Redirect(Mtn.Library.Web.Utils.Parameter.UnallowedPage, true);
                    }
                }
            }
        }

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            
            // Check permissions
            var attrs = filterContext.GetType().GetCustomAttributes(typeof(Library.Attributes.Permission), true);
            if (attrs.Any())
            {
                foreach (var attrP in
                    attrs.Select(attr => (attr as Library.Attributes.Permission)).
                    Where(attrP => attrP != null && attrP.HasPermission(attrP.Value1, attrP.Value2, attrP.Value3, attrP.Value4) == false))
                {
                    if (Mtn.Library.Web.Utils.Parameter.UnallowedPage.IsNullOrWhiteSpaceMtn())
                    {
                        string msg = PermissionErrorMessage;
                        if (msg.IsNullOrWhiteSpaceMtn())
                            msg = (string)Library.Attributes.Permission.UnallowedResult;

                        if (msg.IsNullOrWhiteSpaceMtn())
                            msg = new DataPage<string>().GetUnallowedPage().Message;

                        if (msg.IsNullOrWhiteSpaceMtn())
                        {
                            WriteStatisc("PermissionCheck", "You don't have permission to see this page/action!");
                        }
                        else
                        {
                            WriteStatisc("PermissionCheck", msg);
                            Response.Write(msg);
                        }
                        _supressContent = true;
                    }
                    else
                    {
                        WriteStatisc("PermissionCheck", string.Format("Redirecting to -> {0}", Mtn.Library.Web.Utils.Parameter.UnallowedPage));
                        _supressContent = true;
                        Response.Redirect(Mtn.Library.Web.Utils.Parameter.UnallowedPage, true);
                    }
                }
            }
            if(_supressContent ==false)
                base.OnActionExecuting(filterContext);
        }
    }
}

namespace Mtn.Library.Mvc4.Controller
{
    /// <summary>
    /// Deprecated, use Mtn.Library.Mvc.Controller Instead
    /// </summary>
    public class MtnLoggedController : Mtn.Library.Mvc3.Controller.MtnLoggedController
    {
        
    }
}
