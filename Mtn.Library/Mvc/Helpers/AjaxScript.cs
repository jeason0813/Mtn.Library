using System;
using System.Web;
using Mtn.Library.Web.Enums;
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
       /// <param name="classList"></param>
       /// <param name="methodList"></param>
       /// <param name="writeOnBody"></param>
       /// <param name="writeMinified"></param>
       /// <param name="scriptProcessorType"></param>
       /// <param name="isDeferScript"></param>
       /// <param name="isAsync"></param>
       /// <param name="hashTag"></param>
       /// <param name="exportFile"></param>
       /// <param name="virtualPath"></param>
        /// <param name="useTraditionalParameterForm"> Use traditional parameters, like function(parm1,parm2,parm3) instead function(options), calling myFunc({data:'data'});</param>
       /// <returns></returns>
        public static HtmlString Script(String routeName = null, String classList = null, String methodList = null, Boolean writeOnBody = false, Boolean writeMinified = true, ScriptProcessorType? scriptProcessorType = null, Boolean isDeferScript = false, Boolean isAsync = false, String hashTag = null, Boolean exportFile = false, String virtualPath = "", Boolean useTraditionalParameterForm = false)
        {

            var script = "";

            script = ScriptProcessorEngine.GetScriptCode(
                routeName: routeName, 
                classList: classList, 
                methodList: methodList, 
                writeOnBody: writeOnBody, 
                writeMinified: writeMinified,
                scriptProcessorType: scriptProcessorType, 
                webModeType: WebModeType.MvcController, 
                isDeferScript: isDeferScript, 
                isAsync: isAsync, 
                hashTag: hashTag,
                exportFile: exportFile, virtualPath: virtualPath, useTraditionalParameterForm: useTraditionalParameterForm);

            return new HtmlString(script);
        }
        
    }
}
