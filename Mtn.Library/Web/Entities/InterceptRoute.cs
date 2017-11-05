using System;
using System.Collections.Generic;
using Mtn.Library.Web.Enums;


namespace Mtn.Library.Web.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class InterceptRoute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        public static IList<InterceptRoute> CreateRange(params InterceptRoute[] routes)
        {
            var list = new List<InterceptRoute>();
            list.AddRange((routes));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="assemblyCollection"></param>
        /// <param name="ignoredAssemblyCollection"></param>
        /// <param name="ignoreAssembly"></param>
        /// <param name="interceptType"></param>
        /// <param name="defaultDelimiter"></param>
        /// <param name="customRedirect"></param>
        /// <param name="urlType"> </param>
        /// <param name="acceptEmptyRoute"> </param>
        /// <returns></returns>
        public static InterceptRoute CreateRoute(String routeName = null, String assemblyCollection = null, String ignoredAssemblyCollection = null, Boolean ignoreAssembly = true, InterceptType interceptType = InterceptType.Ajax, String defaultDelimiter = ",", Func<string, RouteType> customRedirect = null, bool acceptEmptyRoute = false, UrlType urlType = UrlType.Route)
        {
            return new InterceptRoute(routeName, assemblyCollection, ignoredAssemblyCollection, ignoreAssembly,
                                      interceptType, defaultDelimiter, customRedirect, acceptEmptyRoute, urlType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="assemblyCollection"></param>
        /// <param name="ignoredAssemblyCollection"></param>
        /// <param name="ignoreAssembly"></param>
        /// <param name="interceptType"></param>
        /// <param name="defaultDelimiter"></param>
        /// <param name="customRedirect"> </param>
        /// <param name="acceptEmptyRoute"> </param>
        /// <param name="urlType"> </param>
        public InterceptRoute(String routeName = null, String assemblyCollection = null, String ignoredAssemblyCollection = null, Boolean ignoreAssembly = true, InterceptType interceptType = InterceptType.Ajax, String defaultDelimiter = ",", Func<string, RouteType> customRedirect = null, bool acceptEmptyRoute = false, UrlType urlType = UrlType.Route)
        {
            RouteName = routeName;
            AssemblyCollection = assemblyCollection;
            IgnoredAssemblyCollection = ignoredAssemblyCollection;
            IgnoreAssembly = ignoreAssembly;
            InterceptType = interceptType;
            DefaultDelimiter = defaultDelimiter;
            CustomRedirect = customRedirect;
            AcceptEmptyRoute = acceptEmptyRoute;
            UrlType = urlType;
        }

        /// <summary>
        /// 
        /// </summary>
        public InterceptType InterceptType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String RouteName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String IgnoredAssemblyCollection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String AssemblyCollection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String DefaultDelimiter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boolean IgnoreAssembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Boolean AcceptEmptyRoute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UrlType UrlType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Func<string, RouteType> CustomRedirect { get; set; }
    }
}
