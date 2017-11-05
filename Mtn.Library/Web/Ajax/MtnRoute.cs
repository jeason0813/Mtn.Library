using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using Mtn.Library.Extensions;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;

namespace Mtn.Library.Web.Ajax
{  

    /// <summary>
    /// 
    /// </summary>
    public class MtnRoute : RouteBase //MvcRouteHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interceptRoutes"></param>
        /// <returns></returns>
        public static MtnRoute CreateRouteHandler(IList<InterceptRoute> interceptRoutes)
        {
            return new MtnRoute(interceptRoutes);
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<InterceptRoute> InterceptRoutes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interceptRoutes"></param>
        public MtnRoute(IList<InterceptRoute> interceptRoutes)
        {
            InterceptRoutes = interceptRoutes;
            Config.RouteCollection = interceptRoutes.Select(x => x.RouteName).ToList();
        }
        

        /// <summary>
        /// When overridden in a derived class, returns route information about the request.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>
        /// An object that contains the values from the route definition if the route matches the current request, or null if the route does not match the request.
        /// </returns>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext.Request != null)
            {
                
                var url = httpContext.Request.RawUrl.ToLowerInvariant();

                if (url.Length < 1)
                    return null;


                if (AjaxEngine.WriteScript(url, httpContext.ApplicationInstance.Context))
                {   
                    return null;
                }

                var urlAjax = (httpContext.Request.ApplicationPath != null && httpContext.Request.ApplicationPath.Length > 1 ? url.Replace(httpContext.Request.ApplicationPath.ToLowerInvariant(), "") : url);
                urlAjax = urlAjax.StartsWith("/") ? urlAjax.Remove(0, 1) : urlAjax;
                var routeAjaxName = (urlAjax.Contains("/") && urlAjax.Length > 1) ? urlAjax.SplitMtn("/")[0] : urlAjax.Replace("/", "");
            
                Parallel.ForEach(InterceptRoutes, interceptRoute =>
                {
                    #region Ajax Route
                    switch (interceptRoute.InterceptType)
                    {
                        case InterceptType.Ajax:
                            if (interceptRoute.RouteName == null)
                                throw new ArgumentNullException("httpContext", "RouteName - To InterceptType == Ajax you need set RouteName.");
                            if (interceptRoute.RouteName.ToLowerInvariant().Trim().Equals(routeAjaxName.ToLowerInvariant()))
                                AjaxEngine.ProcessRequest(httpContext.ApplicationInstance.Context, WebModeType.MvcController);
                            break;
                        case InterceptType.CustomRoute:
                            if (interceptRoute.CustomRedirect == null)
                                throw new ArgumentNullException("httpContext", "CustomRedirect - To InterceptType == CustomRoute you need a CustomRedirect Func.");
                            if (url.Replace("/", "").IsNullOrWhiteSpaceMtn() == false && interceptRoute.AcceptEmptyRoute)
                            {

                                var routeName = (url.Contains("/") && url.Length > 0 && interceptRoute.UrlType == UrlType.Route)
                                    ? url.SplitMtn("/")[0]
                                    : url;
                                var resUrl = interceptRoute.CustomRedirect(routeName);
                                if (resUrl == null)
                                    return;

                                switch (resUrl.Code)
                                {
                                    case RouteCode.Ok200:
                                        httpContext.Response.StatusCode = 200;
                                        if (resUrl.Html.IsNullOrWhiteSpaceMtn() == false)
                                            httpContext.Response.Write(resUrl.Html);
                                        //httpContext.Response.End();
                                        break;
                                    case RouteCode.Redirect301:
                                        httpContext.Response.StatusCode = 301;
                                        httpContext.Response.RedirectPermanent(resUrl.RouteUrl, true);
                                        //httpContext.Response.End();
                                        break;
                                    case RouteCode.Redirect302:
                                        httpContext.Response.StatusCode = 302;
                                        httpContext.Response.Redirect(resUrl.RouteUrl, true);
                                        //httpContext.Response.End();
                                        break;
                                    case RouteCode.NotFound404:
                                        httpContext.Response.StatusCode = 404;
                                        if (resUrl.Html.IsNullOrWhiteSpaceMtn() == false)
                                            httpContext.Response.Write(resUrl.Html);
                                        httpContext.Response.Status = "404 File not found!";
                                        //httpContext.Response.End();
                                        break;
                                    case RouteCode.Error500:
                                        httpContext.Response.StatusCode = 404;
                                        httpContext.Response.Status = "404 File not found!";
                                        if (resUrl.Html.IsNullOrWhiteSpaceMtn() == false)
                                            httpContext.Response.Write(resUrl.Html);
                                        //httpContext.Response.End();
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException("httpContext", "RouteCode");
                                }
                                httpContext.ApplicationInstance.CompleteRequest();
                            }
                            break;
                    }
                    #endregion
                });
            }

            return null;
        }


        /// <summary>
        /// When overridden in a derived class, checks whether the route matches the specified values, and if so, generates a URL and retrieves information about the route.
        /// </summary>
        /// <returns>
        /// An object that contains the generated URL and information about the route, or null if the route does not match <paramref name="values"/>.
        /// </returns>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param><param name="values">An object that contains the parameters for a route.</param>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }
}
