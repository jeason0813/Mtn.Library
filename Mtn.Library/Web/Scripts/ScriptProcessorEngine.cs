using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mtn.Library.Service;
using Mtn.Library.Web.Ajax;
using Mtn.Library.Web.Entities;
using Mtn.Library.Extensions;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Scripts.Template;

namespace Mtn.Library.Web.Scripts
{
    /// <summary>
    /// 
    /// </summary>
    public class ScriptProcessorEngine
    {
        /// <summary>
        /// 
        /// </summary>
        public ScriptProcessorType ProcessorType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IScriptTemplate ScriptTemplate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AjaxClassItem AjaxClassItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WebModeType ModeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="modeType"></param>
        /// <param name="scriptProcessorType"></param>
        /// <returns></returns>
        public static ScriptProcessorEngine CreateInstance(AjaxClassItem item, WebModeType modeType = WebModeType.MvcController, ScriptProcessorType? scriptProcessorType = null)
        {
            var scriptInstance = new ScriptProcessorEngine(item, modeType, scriptProcessorType);

            return scriptInstance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="modeType"></param>
        /// <param name="scriptProcessorType"></param>
        public ScriptProcessorEngine(AjaxClassItem item, WebModeType modeType = WebModeType.MvcController, ScriptProcessorType? scriptProcessorType = null)
        {
            if(item==null)
                return;
            this.AjaxClassItem = item;
            this.ProcessorType = scriptProcessorType.HasValue ? scriptProcessorType.Value : item.AjaxClass.ScriptProcessor;
            this.ModeType = modeType;
            switch (this.ProcessorType)
            {
                case ScriptProcessorType.JQuery:
                    this.ScriptTemplate = new JQuery();
                    break;
                case ScriptProcessorType.ExtJs:
                    this.ScriptTemplate = new ExtJs();
                    break;
                case ScriptProcessorType.Dojo:
                    this.ScriptTemplate = new Dojo();
                    break;
                case ScriptProcessorType.Prototype:
                    this.ScriptTemplate = new Prototype();
                    break;
                case ScriptProcessorType.Custom:
                    if (item.AjaxClass.ScriptTemplate == null)
                        throw new ArgumentNullException("ScriptTemplate");
                    this.ScriptTemplate = item.AjaxClass.ScriptTemplate;
                    break;
            }

            this.ScriptTemplate.Mode = modeType;
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StringBuilder CreateClassCode()
        {
            ScriptTemplate.Mode = this.ModeType;
            StringBuilder result = ScriptTemplate.Init(AjaxClassItem.AjaxClass.RootNameSpace);
            ScriptTemplate.AppendClass(ref result, AjaxClassItem.ClassName);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String CreateFullCode(Boolean useTraditionalParameterForm)
        {
            var result = "";
            ScriptTemplate.Mode = this.ModeType;
            var script = ScriptTemplate.Init(AjaxClassItem.AjaxClass.RootNameSpace);
            ScriptTemplate.AppendClass(ref script, AjaxClassItem.ClassName);
            foreach (AjaxMethodItem method in AjaxClassItem.AjaxMethodItemCollection.Select(methodHash => methodHash.Value))
            {
                ScriptTemplate.CreateMethodBody(ref script, AjaxClassItem.ClassName, method.AjaxMethod.Name, method, method.Parameters, AjaxClassItem, useTraditionalParameterForm);
            }
            result = script.ToString();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="useTraditionalParameterForm"> </param>
        /// <returns></returns>
        public String CreateMethodCode(AjaxMethodItem method, Boolean useTraditionalParameterForm)
        {
            ScriptTemplate.Mode = this.ModeType;
            var script = new StringBuilder();
            ScriptTemplate.CreateMethodBody(ref script, AjaxClassItem.ClassName, method.AjaxMethod.Name, method, method.Parameters, AjaxClassItem, useTraditionalParameterForm);

            return script.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="script"></param>
        /// <param name="useTraditionalParameterForm"> </param>
        public void CreateMethodCode(AjaxMethodItem method, ref StringBuilder script, Boolean useTraditionalParameterForm)
        {
            ScriptTemplate.Mode = this.ModeType;

            ScriptTemplate.CreateMethodBody(ref script, AjaxClassItem.ClassName, method.AjaxMethod.Name, method, method.Parameters, AjaxClassItem, useTraditionalParameterForm);

        }
        private static readonly string WriteMinifiedMd5 = "minified".Md5Mtn(true);
        private static readonly string WriteUnMinifiedMd5 = "unminified".Md5Mtn(true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="classList"></param>
        /// <param name="methodList"></param>
        /// <param name="writeOnBody"></param>
        /// <param name="writeMinified"></param>
        /// <param name="scriptProcessorType"></param>
        /// <param name="webModeType"></param>
        /// <param name="isDeferScript"></param>
        /// <param name="isAsync"></param>
        /// <param name="hashTag"></param>
        /// <param name="exportFile"></param>
        /// <param name="virtualPath"></param>
        /// <param name="useTraditionalParameterForm"> </param>
        /// <returns></returns>
        public static string GetScriptCode(String routeName = null, String classList = null, String methodList = null, Boolean writeOnBody = false, 
            Boolean writeMinified = true, ScriptProcessorType? scriptProcessorType = null, 
            WebModeType webModeType = WebModeType.MvcController, Boolean isDeferScript = false,
            Boolean isAsync = false, String hashTag = null, Boolean exportFile = false, String virtualPath = "", Boolean useTraditionalParameterForm = false)
        {
            if (writeMinified == false && Utils.Parameter.DisableAjaxDebug)
                return "Ajax debug is disabled on server";

            var script = new StringBuilder();
            if (routeName.IsNullOrWhiteSpaceMtn())
                routeName = Config.RouteCollection.FirstOrDefault();

            var scriptProcessorTag = (scriptProcessorType.HasValue ? scriptProcessorType.Value.ToString().ToLower().Md5Mtn(true) : "");
            string hashtag = hashTag.IsNullOrWhiteSpaceMtn() ? (writeMinified ? WriteMinifiedMd5 : WriteUnMinifiedMd5) + scriptProcessorTag + (useTraditionalParameterForm?"useTraditionalParameterForm".Md5Mtn(true):"") : hashTag;
            var names = new StringBuilder();
            StringBuilder[] scriptCache = {new StringBuilder("")};
            if (hashTag.IsNullOrWhiteSpaceMtn())
            {
                if (classList.IsNullOrWhiteSpaceMtn() == false)
                {
                    foreach (var cname in classList.SplitMtn(",").OrderBy(x => x))
                    {
                        names.Append(cname.ToLowerInvariant());
                    }
                    hashtag += names.ToString().Md5Mtn(true);
                }
                else
                {
                    var list = Config.AjaxScriptCollection;
                    foreach (var ajaxClass in list.Select(x => x.Value))
                    {
                        names.Append(ajaxClass.Key);
                    };
                    hashtag += names.ToString();
                }

                if (methodList.IsNullOrWhiteSpaceMtn() == false)
                {
                    foreach (var cname in methodList.SplitMtn(",").OrderBy(x => x))
                    {
                        names.Append(cname);
                    }
                    hashtag += names.ToString().Md5Mtn(true);
                }
            }

            if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxScriptMethod, hashtag) == false)
            {
                var listAuxFunctions = new List<string>();
                if (classList.IsNullOrWhiteSpaceMtn() == false)
                {
                    var listClass = from ajx in Config.AjaxScriptCollection
                                    join ajxCReq in classList.SplitMtn(",").OrderBy(x => x).ToList()
                                        on ajx.Key equals ajxCReq.ToLowerInvariant().Md5Mtn(true)
                                    select ajx.Value;

                    Parallel.ForEach(listClass, (ajaxClass, state) =>
                    {
                        var localAjaxClass = ajaxClass;
                        var ajaxClassItem = Config.AjaxClassItemCollection.Where(x => x.Key == localAjaxClass.AjaxClassKey).Select(x => x.Value).FirstOrDefault();
                        if(ajaxClassItem == null)
                            state.Break();

                        var scriptProcessorTypeKey = scriptProcessorType.HasValue ? scriptProcessorType.Value.ToString() : "";
                        var inst = CreateInstance(ajaxClassItem, webModeType, scriptProcessorType);
                        if (listAuxFunctions.Contains(scriptProcessorTypeKey) == false)
                        {
                            scriptCache[0].Append(inst.ScriptTemplate.GetAuxiliarFunctions());
                            listAuxFunctions.Add(scriptProcessorTypeKey);
                        }
                        if (methodList.IsNullOrWhiteSpaceMtn() == false)
                        {
                            var listMeth = from ajxMeth in localAjaxClass.AjaxScriptMethodCollection
                                           join ajxMReq in methodList.SplitMtn(",").OrderBy(x => x).ToList()
                                               on ajxMeth.Key equals ajxMReq.ToLowerInvariant().Md5Mtn(true)
                                           select ajxMeth.Value;
                            var strScript = new StringBuilder("");


                            if (localAjaxClass.ScriptProcessor != scriptProcessorType || localAjaxClass.UseTraditionalParameterForm != useTraditionalParameterForm)
                            {
                                strScript.Append(inst.CreateClassCode());
                                Parallel.ForEach(listMeth.ToList(), ajaxClassMeth =>
                                {
                                    var classItem = Config.AjaxClassItemCollection.Where(x => x.Key == localAjaxClass.AjaxClassKey).Select(x => x.Value).FirstOrDefault();
                                    if (classItem != null)
                                    {
                                        var meth = ajaxClassMeth;
                                        var ajaxMethodItem = classItem.AjaxMethodItemCollection
                                            .Where(x => x.Key == meth.Key).Select((x => x.Value)).FirstOrDefault();

                                        inst.CreateMethodCode(ajaxMethodItem, ref strScript, useTraditionalParameterForm);
                                    }
                                });
                                scriptCache[0].AppendLine((writeMinified) ? Utils.Minifier.MinifyCode(strScript.ToString()) : strScript.ToString());
                            }
                            else
                            {
                                scriptCache[0].AppendLine((writeMinified) ? ajaxClass.ScriptMinified : ajaxClass.Script);
                                Parallel.ForEach(listMeth.ToList(), ajaxClassMeth => scriptCache[0].AppendLine((writeMinified) ? ajaxClassMeth.ScriptMinified : ajaxClassMeth.Script));
                            }
                        }
                        else
                        {
                            if (ajaxClass.ScriptProcessor == scriptProcessorType)
                            {
                                scriptCache[0].AppendLine(writeMinified ? ajaxClass.FullScriptMinified : ajaxClass.FullScript);
                            }
                            else
                            {
                                var scriptFull = inst.CreateFullCode(useTraditionalParameterForm);
                                scriptCache[0].AppendLine(writeMinified ? Utils.Minifier.MinifyCode(scriptFull) : scriptFull);
                            }
                        }

                    });

                }
                else
                {
                    foreach (var ajaxClass in Config.AjaxScriptCollection.Select(x => x.Value))
                    {
                        var ajaxClassItem = Config.AjaxClassItemCollection.Where(x => x.Key == ajaxClass.AjaxClassKey).Select(
                                   x => x.Value).FirstOrDefault();
                        if(ajaxClassItem == null)
                            continue;
                        var scriptProcessorTypeKey = scriptProcessorType.HasValue?scriptProcessorType.Value.ToString():"";
                        var inst = CreateInstance(ajaxClassItem, webModeType, scriptProcessorType);
                        if (listAuxFunctions.Contains(scriptProcessorTypeKey) == false)
                        {
                            scriptCache[0].Append(inst.ScriptTemplate.GetAuxiliarFunctions());
                            listAuxFunctions.Add(scriptProcessorTypeKey);
                        }
                        if (ajaxClass.ScriptProcessor == scriptProcessorType)
                        {
                            scriptCache[0].AppendLine(writeMinified ? ajaxClass.FullScriptMinified : ajaxClass.FullScript);
                        }
                        else
                        {

                            var scriptFull = inst.CreateFullCode(useTraditionalParameterForm);
                            scriptCache[0].AppendLine(writeMinified ? Utils.Minifier.MinifyCode(scriptFull) : scriptFull);
                        }
                    }
                }

                if (routeName.IsNullOrWhiteSpaceMtn())
                {
                    scriptCache[0] = scriptCache[0].Replace("{MtnMvcRoute}", webModeType == WebModeType.AjaxHandler ? "mtnajax." + Utils.Parameter.AjaxExtension + "/" : "");
                }
                else
                {
                    scriptCache[0] = scriptCache[0].Replace("{MtnMvcRoute}", routeName.ToLowerInvariant() + "/");
                }

                Cache.Instance.Add(MtnAjaxCacheType.AjaxScriptMethod, scriptCache[0].ToString(), hashtag);
            }
            else if (writeOnBody)
            {
                scriptCache[0].Append(Cache.Instance[MtnAjaxCacheType.AjaxScriptMethod, hashtag]);
            }

            if (exportFile && Directory.Exists(HttpContext.Current.Server.MapPath(virtualPath)))
            {
                string extension = ((Utils.Parameter.UseAjaxExtension && webModeType == WebModeType.MvcController) ? "" : "." + Utils.Parameter.AjaxExtension);
                var filename = HttpContext.Current.Server.MapPath(virtualPath) + "\\mtn." + hashtag + (writeMinified ? ".min" : "") + extension;
                scriptCache[0].ToString().ToFileMtn(filePath: filename);
            }

            var hostUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            hostUrl = hostUrl.Substring(0, hostUrl.LastIndexOf(HttpContext.Current.Request.Url.AbsolutePath)) + HttpContext.Current.Request.ApplicationPath;
            if (writeOnBody)
            {
                scriptCache[0].Insert(0, ScriptProcessorEngine.GetStartScriptCode(Utils.Parameter.DisableMtnScriptForClasses));
                script.AppendFormat("<script type=\"text/javascript\">\n{0}</script>", scriptCache[0]);
            }
            else
            {
                string extension = ((!Utils.Parameter.UseAjaxExtension && webModeType == WebModeType.MvcController) ? "/" : "." + Utils.Parameter.AjaxExtension);
                script.AppendFormat(
                    "<script language=\"Javascript\" type=\"text/javascript\" src=\"{0}/{1}mtnajaxmethods{2}?hashtag={3}\" {4} {5}></script>",
                    (hostUrl.EndsWith("/")?hostUrl.Substring(0,hostUrl.Length -1):hostUrl), routeName.IsNullOrWhiteSpaceMtn() ? "" : (routeName + "/"), extension, hashtag, isDeferScript ? "defer" : "", isAsync ? "async" : "");
            }

            return script.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="writeOnBody"></param>
        /// <param name="writeMinified"></param>
        /// <param name="webModeType"></param>
        /// <param name="disableMtnScriptForClasses"></param>
        /// <returns></returns>
        public static String GetRequiredCode(String routeName = null, Boolean writeOnBody = false, Boolean writeMinified = true, WebModeType webModeType = WebModeType.MvcController, Boolean disableMtnScriptForClasses = false)
        {
            var hostUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            hostUrl = hostUrl.Substring(0, hostUrl.LastIndexOf(HttpContext.Current.Request.Url.AbsolutePath)) + HttpContext.Current.Request.ApplicationPath;
            var script = new StringBuilder("");
            if (disableMtnScriptForClasses)
                Utils.Parameter.DisableMtnScriptForClasses = true;

            if (writeOnBody)
            {
                script.AppendFormat("<script type=\"text/javascript\">\n{0}</script>",
                                    ScriptProcessorEngine.GetStartScriptCode(false, writeMinified));
            }
            else
            {
                string scriptName = ((!Utils.Parameter.UseAjaxExtension && webModeType == WebModeType.MvcController)
                                         ? "mtnlibrary{0}/"
                                         : "mtnlibrary{0}." + Utils.Parameter.AjaxExtension);
                scriptName = string.Format(scriptName, writeMinified ? (!Utils.Parameter.UseAjaxExtension?"-":".") + "min" : "");
                script.AppendFormat(
                    "<script language=\"Javascript\" type=\"text/javascript\" src=\"{0}/{1}{2}\"></script>",
                    (hostUrl.EndsWith("/") ? hostUrl.Substring(0, hostUrl.Length - 1) : hostUrl), routeName.IsNullOrWhiteSpaceMtn() ? "" : routeName + "/", scriptName);
            }
            return script.ToString();
        }

        #region auxiliar functions

        private const string MtnScriptCode = @"
(function () {    
    var global = this;
    if (typeof Mtn === 'undefined') {
        global.Mtn = {};
    }
    if (typeof Mtn.utils === 'undefined') {
        Mtn.utils = {};
    }
    if (typeof Mtn.getUrl === 'undefined') {
        Mtn.getUrl = function() {return Mtn.SERVER_URL;};
    }
    if (typeof Mtn.utils.createNamespace === 'undefined') {
        Mtn.utils.createNamespace = function (namespace) {
            var me = window;
            var arrNs = namespace.split('.');
            var lastObj = me;
            for (var i = 0; i < arrNs.length; i++) {
                var ns = arrNs[i];
                if (typeof lastObj[ns] === 'undefined')
                    lastObj[ns] = {};
                lastObj = lastObj[ns];
            }
        };
    }

    Mtn.ns = Mtn.utils.createNamespace;
    Mtn.utils.isNullOrUndefined = function (o) {
        if (typeof o === 'undefined') return true;
        if (o === null) return true;
        return false;
    };
    Mtn.isNullOrUndefined = Mtn.utils.isNullOrUndefined;
    Mtn.utils.isEmpty = function (o) {
        return (typeof o !== 'string' || o.length === 0);
    };
    Mtn.isEmpty = Mtn.utils.isEmpty;
    Mtn.utils.createId = function (el,matchDate) {
        if(Mtn.isNullOrUndefined(el))
            el = 'id';
        var replaceMatch = 'xxxxxxxx-xxxx-4xxx-yxxx-';
        if(matchDate) {
            var dateVal = (new Date().getTime() + '');
            replaceMatch += dateVal.slice(dateVal.length-12);
        }
        else {
            replaceMatch += 'xxxxxxxxxxxx';
        }
        var id = replaceMatch.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
        var lel = (Mtn.isEmpty(el) ? '' : el + '-');
        lel += id;
        if (Mtn.isNullOrUndefined(document.getElementById(lel)))
            return lel;
        else
            return Mtn.utils.createId(el);
    };
    Mtn.id = Mtn.utils.createId;
    Mtn.support = {};
    var fi = document.createElement('input');
    fi.type = 'file';
    if(fi.multiple === false || fi.multiple === true) {
        fi.multiple = true;
    }
    else if(fi.min === '' && fi.max === '') {
        fi.min = 1;
        fi.max = 9999;
    }
    Mtn.support.multipleFile = (typeof fi.multiple !== 'undefined');
    Mtn.support.uploadXhr = (Mtn.support.multipleFile && typeof (new XMLHttpRequest()).upload !== 'undefined' ); 

    // from jQuery
    Mtn.getType = function( obj ) {
        var class2type = {};
        var toString = class2type.toString;
		if ( obj == null ) {
			return obj + '';
		}
		// Support: Android < 4.0, iOS < 6 (functionish RegExp)
		return typeof obj === 'object' || typeof obj === 'function' ?
			class2type[ toString.call(obj) ] || 'object' :
			typeof obj;
	};
    //from jquery
    Mtn.isFunction = function( obj ) {
		return Mtn.getType(obj) === 'function';
	};

    Mtn.reset = function (obj) {
        for (var name in obj) {
            if (Mtn.isFunction(obj[name]))
                continue;
            this[name] = '';
        };
    };
    Mtn.trim = function(text) {
        //from jquery
        var rtrim = /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g;
        return text == null ? '' : (text + '').replace( rtrim, '' );
    };

    Mtn.removeAt = function (array, pos) {
        if (pos < 0 || pos >= array.length)
            return array;
        var nArr = [];
        for (var i = pos+1; i < array.length; i++) {
            nArr.push(array[i]);
        }
        return nArr;
    };
    Mtn.utils.simpleLambda = function(query) {
        if (Mtn.isEmpty(query))
            return new Function('return true;');
        if (query.indexOf('=>') < 0)
            return new Function(query);

        var arr = query.split('=>');
        var qualifier = Mtn.trim(arr[0]);
        var body = Mtn.removeAt(arr, 0);
        body = body.join('=>');
        var code = 'return ' + body + ';';
        var fn = new Function(qualifier, code);
        return fn;
    };

    Mtn.lambda = Mtn.utils.simpleLambda;
    
    Mtn.whereMtn = function(array,query) {
        var fn = Mtn.lambda(query);
        var nArr = [];
        for (var i = 0; i < array.length; i++) {
            if (fn(array[i])) {
                nArr.push(array[i]);
            }
        }
        return nArr;
    };

    Mtn.firstMtn = function (array,query) {
        var fn = Mtn.lambda(query);
        for (var i = 0; i < array.length; i++) {
            if (fn(array[i])) {
                return array[i];
            }
        }
        return null;
    };

    Mtn.anyMtn = function (array,query) {
        var fn = Mtn.lambda(query);
        for (var i = 0; i < array.length; i++) {
            if (fn(array[i])) {
                return true;
            }
        }
        return false;
    };

    Mtn.version = function(password) {
        Mtn.Library.Internals.Utilities.version({data:{password:password}, success:function(r) {console.log(r);},error:function(){console.log(arguments);}});
    };

})();";

        private static string _mtnScriptCodeMin;
        //@"(function(){var global=this;if(typeof Mtn==='undefined'){global.Mtn={};} if(typeof Mtn.utils==='undefined'){Mtn.utils={};} if(typeof Mtn.utils.createNamespace==='undefined'){Mtn.utils.createNamespace=function(namespace){var me=window;var arrNs=namespace.split('.');var lastObj=me;for(var i=0;i<arrNs.length;i++){var ns=arrNs[i];if(typeof lastObj[ns]==='undefined') lastObj[ns]={};lastObj=lastObj[ns];}};} Mtn.ns=Mtn.utils.createNamespace;Mtn.utils.isNullOrUndefined=function(o){if(typeof o==='undefined') return true;if(o===null) return true;return false;};Mtn.isNullOrUndefined=Mtn.utils.isNullOrUndefined;Mtn.utils.isEmpty=function(o){return(typeof o!=='string'||o.length===0);};Mtn.isEmpty=Mtn.utils.isEmpty;Mtn.utils.createId=function(el){var id='id-xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g,function(c){var r=Math.random()*16|0,v=c=='x'?r:(r&0x3|0x8);return v.toString(16);});var lel=(Mtn.isEmpty(el)?'':el+'-');lel+=id;if(Mtn.isNullOrUndefined(document.getElementById(lel))) return lel;else return Mtn.utils.createId(el);};Mtn.id=Mtn.utils.createId;Mtn.support={};var fi=document.createElement('input');fi.type='file';if(fi.multiple===false||fi.multiple===true){fi.multiple=true;}else if(fi.min===''&&fi.max===''){fi.min=1;fi.max=9999;} Mtn.support.multipleFile=(typeof fi.multiple!=='undefined');Mtn.support.uploadXhr=(Mtn.support.multipleFile && typeof (new XMLHttpRequest()).upload !== 'undefined');Mtn.getType = function (obj) {var class2type = {};var toString = class2type.toString;if (obj == null) {return obj + "";} return typeof obj === 'object' || typeof obj === 'function' ? class2type[toString.call(obj)] || 'object' : typeof obj;} Mtn.isFunction = function (obj) { return jQuery.type(obj) === 'function';} Mtn.reset = function (obj) {for (var name in obj) {if (Mtn.isFunction(obj[name]))continue;this[name] = '';};}})(); ";
        // new GUID comes from Source: http://stackoverflow.com/questions/105034/how-to-create-a-guid-uuid-in-javascript/105074#105074
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnEmptyString"></param>
        /// <param name="minified"></param>
        /// <returns></returns>
        public static String GetStartScriptCode(Boolean returnEmptyString, Boolean minified = true)
        {
            if (returnEmptyString)
                return "";
            if (minified && _mtnScriptCodeMin.IsNullOrWhiteSpaceMtn())
            {
                _mtnScriptCodeMin = Utils.Minifier.MinifyCode(MtnScriptCode);
            }
            Func<string,string> fnGetData = (val) => val + string.Format("Mtn.SERVER_URL = '{0}';", BaseTemplate.GetUrl());

            return (minified ? fnGetData(_mtnScriptCodeMin) : fnGetData(MtnScriptCode));
        }


    }
}
