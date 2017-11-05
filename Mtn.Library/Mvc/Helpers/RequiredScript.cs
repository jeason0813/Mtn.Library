using System;
using System.Web;
using Mtn.Library.Web.Scripts;

namespace Mtn.Library.Mvc.Helpers
{
    /// <summary>
    /// 
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
        public static HtmlString RequiredScript(String routeName = null, Boolean writeOnBody = false, Boolean writeMinified = true)
        {

            var script = "";

            script = ScriptProcessorEngine.GetRequiredCode(routeName, writeOnBody, writeMinified);

            return new HtmlString(script);
        }
        
    }
}
