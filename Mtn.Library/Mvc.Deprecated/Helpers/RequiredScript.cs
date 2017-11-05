using System;
using System.Web.Mvc;
using Mtn.Library.Web.Scripts;

namespace Mtn.Library.Mvc3.Helpers
{
    /// <summary>
    /// Deprecated use Mtn.Library.Mvc.Helpers instead
    /// </summary>
    public static partial class AjaxScriptHelper
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="routeName"></param>
       /// <param name="writeOnBody"></param>
       /// <param name="writeMinified"></param>
       /// <returns></returns>
        public static MvcHtmlString RequiredScript(String routeName = null, Boolean writeOnBody = false, Boolean writeMinified = true)
        {

            var script = "";

            script = ScriptProcessorEngine.GetRequiredCode(routeName, writeOnBody, writeMinified);

            return new MvcHtmlString(script);
        }
        
    }
}

namespace Mtn.Library.Mvc4.Helpers
{
    /// <summary>
    /// Deprecated use Mtn.Library.Mvc.Helpers instead
    /// </summary>
    public static partial class AjaxScriptHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="writeOnBody"></param>
        /// <param name="writeMinified"></param>
        /// <returns></returns>
        public static MvcHtmlString RequiredScript(String routeName = null, Boolean writeOnBody = false, Boolean writeMinified = true)
        {

            var script = "";

            script = ScriptProcessorEngine.GetRequiredCode(routeName, writeOnBody, writeMinified);

            return new MvcHtmlString(script);
        }

    }
}