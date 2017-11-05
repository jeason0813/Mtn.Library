namespace Mtn.Library.Web.Enums
{
    #region Enums
    /// <summary>
    /// <para>Indicates the type of request</para>
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// Post
        /// </summary>
        Post = 0,
        /// <summary>
        /// Get 
        /// </summary>
        Get = 1
    }

    /// <summary>
    /// <para>Indicates the format of the response and serialize according to the type reported</para>
    /// </summary>
    public enum ResponseType
    {
        /// <summary>
        /// Json
        /// </summary>
        Json = 0,
        /// <summary>
        /// Xml
        /// </summary>
        Xml = 1,
        /// <summary>
        /// Text
        /// </summary>
        Text = 2,
        /// <summary>
        /// JsonP
        /// </summary>
        JsonP = 3,
        /// <summary>
        /// HTML
        /// </summary>
        Html = 4
    }

    /// <summary>
    /// 
    /// </summary>
    public enum WebModeType
    {
        /// <summary>
        /// 
        /// </summary>
        AjaxHandler=1,
        /// <summary>
        /// REST mode like /Route/Namespace-Class/Method
        /// </summary>
        MvcController=2,

        /// <summary>
        /// Use the both
        /// </summary>
        Both = 3
    }
    /// <summary>
    /// 
    /// </summary>
    public enum InterceptType
    {
        /// <summary>
        /// 
        /// </summary>
        CustomRoute = 1,

        /// <summary>
        /// 
        /// </summary>
        Ajax = 2
    }
    /// <summary>
    /// 
    /// </summary>
    public enum RouteCode
    {
        /// <summary>
        /// 
        /// </summary>
        Ok200 = 1,
        /// <summary>
        /// 
        /// </summary>
        Redirect301 = 2,
        /// <summary>
        /// 
        /// </summary>
        Redirect302 = 3,
        /// <summary>
        /// 
        /// </summary>
        NotFound404 = 4,
        /// <summary>
        /// 
        /// </summary>
        Error500 = 5
    }

    /// <summary>
    /// 
    /// </summary>
    public enum UrlType
    {
        /// <summary>
        /// 
        /// </summary>
        Route = 1,
        /// <summary>
        /// 
        /// </summary>
        RawUrl = 2
    }
    #endregion
}
