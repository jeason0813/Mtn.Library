using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;
using Mtn.Library.Extensions;

namespace Mtn.Library.Web.Scripts.Template
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseTemplate : IScriptTemplate
    {
        #region private fields
        private String _newLine = Environment.NewLine;
        private String _tabulation = "\t";
        private static String _rootNamespace = "";
        private WebModeType _mode = WebModeType.MvcController;

        #region Ajax Basic Templates

        private string _ajaxAuxFunctionTemplate =
        @"
        MtnCreateNS('Mtn.AjaxTemplate');
       
        Mtn.AjaxTemplate.createXHR = function() {
            try {
                return new XMLHttpRequest();
            } catch(e) {
            }
            try {
                return new ActiveXObject('Msxml2.XMLHTTP.6.0');
            } catch(e) {
            }
            try {
                return new ActiveXObject('Msxml2.XMLHTTP.3.0');
            } catch(e) {
            }
            try {
                return new ActiveXObject('Msxml2.XMLHTTP');
            } catch(e) {
            }
            try {
                return new ActiveXObject('Microsoft.XMLHTTP');
            } catch(e) {
            }
            alert('XMLHttpRequest not supported');
    
            return null;
        };

        Mtn.AjaxTemplate.parseJSON = function(data) {
            return window.JSON && window.JSON.parse ?
                window.JSON.parse(data) :
                (new Function('return ' + data))();
        };

        Mtn.AjaxTemplate.handleResponse = function(xhr, responseType, callbackSuccess, callbackError) {
            var xhr = Mtn.AjaxTemplate.createXHR(); // cross browser XHR creation
            if (xhr.readyState == 4) {
                var responseData = '';

                try {
                    if (responseType === 'xml')
                        responseData = xhr.responseXML;
                    else
                        responseData = xhr.responseText;

                } catch(e) {
                    responseData = e;
                }
                if (xhr.status == 200) {
                    if (responseType === 'json') {

                        callbackSuccess(Mtn.AjaxTemplate.parseJSON(responseData));
                    } else
                        callbackSuccess(responseData);

                } else {
                    callbackError(responseData);
                }
            }
        };
        Mtn.AjaxTemplate.sendGet = function(options){
                var xhr = Mtn.AjaxTemplate.createXHR(); // cross browser XHR creation
                if (xhr) {
                    if(typeof options.data === 'undefined' || options.data == null )
                        options.data = '';
                    options.data = '___RandomMtn=' + Mtn.AjaxTemplate.NewGuid() + '&' + options.data;
                    if(options.user)
                        xhr.open(""GET"", options.url, true, options.user, options.password);
                    else
                        xhr.open(""GET"", options.url, true);
                    xhr.send(options.data);
                    xhr.onreadystatechange(function() {
                        Mtn.AjaxTemplate.handleResponse(xhr, options.responseType, options.callbackSuccess, options.callbackError);
                    });
                }
            };
        Mtn.AjaxTemplate.sendPost = function(options){
                var xhr = createXHR(); // cross browser XHR creation
                if (xhr) {
                    if (options.user)
                        xhr.open(""POST"", options.url, true, options.user, options.password);
                    else
                        xhr.open(""POST"", options.url, true);
        
                    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                    xhr.send(options.data);
                    xhr.onreadystatechange(function() {
                        Mtn.AjaxTemplate.handleResponse(xhr, options.responseType, options.callbackSuccess, options.callbackError);
                    });
                }
            };";

        private string _ajaxGetFunctionTemplate =
            @"Mtn.AjaxTemplate.sendGet(mtnOptions);";
        
        private string _ajaxPostFunctionTemplate=
            @"Mtn.AjaxTemplate.sendPost(mtnOptions);";
        private string _ajaxFormFunctionTemplate;
        private string _ajaxJsonPFunctionTemplate;

        #endregion
        #endregion

        private string _hostUrl;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static String GetUrl()
        {
            string _hostUrl = "//";
            _hostUrl += System.Web.HttpContext.Current.Request.Url.Host.ToLowerInvariant();
            if (!System.Web.HttpContext.Current.Request.Url.IsDefaultPort)
                _hostUrl += ":" + System.Web.HttpContext.Current.Request.Url.Port;
            return _hostUrl;
        }
        /// <summary>
        /// Url to call, can be changed for external calls, normaly you don't need change this
        /// </summary>
        public virtual String HostUrl
        {
            get
            {
                if (Utils.Parameter.EnableMultiPortal)
                    return "' + Mtn.getUrl() + '";
                
                if (_hostUrl == null)
                {
                    _hostUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLowerInvariant();
                    string appPath = System.Web.HttpContext.Current.Request.ApplicationPath.ToLowerInvariant();
                    appPath = (appPath.Length > 1?appPath:"");
                    int urlPos = _hostUrl.LastIndexOf(System.Web.HttpContext.Current.Request.ApplicationPath.ToLowerInvariant());
                    if(urlPos > 0)
                        _hostUrl = _hostUrl.Substring(0, urlPos) + appPath;
                    else
                        _hostUrl = _hostUrl + appPath;
                }
                
                
                return _hostUrl.Replace("http:","").Replace("https:",""); 
            }
            set { _hostUrl = value; }
        }

        /// <summary>
        /// Web mode - represents how you use this functionality, like MVC or WEBForms with ajaxhandler, etc
        /// </summary>
        public virtual Enums.WebModeType Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// Javascript code necessary for the other templates, will be create once 
        /// </summary>
        public virtual String AjaxAuxFunctionTemplate
        {
            get { return _ajaxAuxFunctionTemplate; }
            set { _ajaxAuxFunctionTemplate = value; }
        }

        /// <summary>
        /// Template for Get calls
        /// </summary>
        public virtual String AjaxGetFunctionTemplate
        {
            get { return _ajaxGetFunctionTemplate; }
            set { _ajaxGetFunctionTemplate = value; }
        }

        
        /// <summary>
        /// Template for Post calls
        /// </summary>
        public virtual String AjaxPostFunctionTemplate
        {
            get { return _ajaxPostFunctionTemplate; }
            set { _ajaxPostFunctionTemplate = value; }
        }

        
        /// <summary>
        /// Template for Form call
        /// </summary>
        public virtual String AjaxFormFunctionTemplate
        {
            get { return _ajaxFormFunctionTemplate; }
            set { _ajaxFormFunctionTemplate = value; }
        }

        

        /// <summary>
        /// Template for JsonP call
        /// </summary>
        public virtual String AjaxJsonPFunctionTemplate
        {
            get { return _ajaxJsonPFunctionTemplate; }
            set { _ajaxJsonPFunctionTemplate = value; }
        }

        /// <summary>
        /// New line, normaly "\n"
        /// </summary>
        public virtual String NewLine
        {
            get { return _newLine; }
            set { _newLine= value; }
        }

        
        /// <summary>
        /// Tabulation, normaly "\t"
        /// </summary>
        public virtual String Tabulation
        {
            get { return _tabulation; }
            set { _tabulation = value; }
        }
        /// <summary>
        /// Returns New line and tabulation
        /// </summary>
        /// <param name="tabCount">How many tabs you want</param>
        /// <returns>New line and tabulations </returns>
        public virtual String GetNewLineTab(Int32 tabCount )
        {
            var retVal = new StringBuilder(NewLine);
            for (int i = 0; i < tabCount; i++)
            {
                retVal.Append(Tabulation);
            }
            return retVal.ToString();
        }

        
        /// <summary>
        /// Your RootNamespace ("prefix"), normaly your company like "Metanoia, to use in fucntions like "Metanoia.MyClass.MyMethod"
        /// </summary>
        public virtual String RootNamespace { get { return _rootNamespace; } }

        /// <summary>
        /// Returns the initial StringBuilder with your defaylt namespace
        /// </summary>
        /// <param name="rootNamespace">Root namespace </param>
        /// <param name="hostUrl">Your host to calls,normaly you need do nothing here </param>
        public virtual StringBuilder Init(String rootNamespace, String hostUrl = null)
        {
            _rootNamespace = rootNamespace;
            if (!hostUrl.IsNullOrEmptyMtn())
                HostUrl = hostUrl;
            var retVal = new StringBuilder("");
            
            return retVal;
        }

        /// <summary>
        /// AppendClass Namespace
        /// </summary>
        /// <param name="sbWork">StringBuilder to append classNamespace</param>
        /// <param name="classNamespace">class name</param>
        public virtual void AppendClass(ref StringBuilder sbWork, String classNamespace)
        {
            string format = "Mtn.ns('{0}');";
            if (RootNamespace.IsNullOrWhiteSpaceMtn() == false)
            {
                format = "Mtn.ns('{0}.{1}');";
                sbWork.AppendFormat(format,  RootNamespace, classNamespace);
            }
            else
            {
                sbWork.AppendFormat(format, classNamespace);
            }
            sbWork.AppendLine();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbWork">StringBuilder to append method</param>
        /// <param name="classNamespace">class name</param>
        /// <param name="methodName">method name</param>
        /// <param name="method">Ajax method</param>
        /// <param name="parameters">List of parameters</param>
        /// <param name="ajaxClass"> </param>
        /// <param name="useTraditionalParameterForm"> Use traditional parameters, like function(parm1,parm2,parm3) instead function(options), calling myFunc({data:'data'});</param>
        /// <returns></returns>
        public virtual void CreateMethodBody(ref System.Text.StringBuilder sbWork, string classNamespace, string methodName, AjaxMethodItem method, IList<Entities.Parameter> parameters, AjaxClassItem ajaxClass, bool useTraditionalParameterForm = false)
        {
            var functionBody = new StringBuilder("function(");

            if (useTraditionalParameterForm)
            {
                for (var index = 0; index < parameters.Count; index++)
                {
                    if (index != 0)
                        functionBody.Append(",");
                    var parameter = parameters[index];
                    functionBody.Append(parameter.Name);
                }
                functionBody.Append((parameters.Count > 0 ? "," : "") + "callbackSuccess,callbackError,callbackComplete,uploadProgress");
            }
            else
            {
                functionBody.Append("options");
            }

            functionBody.Append("){" + GetNewLineTab(1));


            functionBody.Append(GetNewLineTab(1));
            functionBody.Append("var mtnOptions = (typeof options === 'undefined' || options == null)?{}:options;");
            functionBody.Append(GetNewLineTab(1));
            functionBody.Append("mtnOptions.data = mtnOptions.data || {};");
            functionBody.Append(GetNewLineTab(1));
            if (useTraditionalParameterForm)
            {

                functionBody.Append(GetNewLineTab(1));
                functionBody.
                    Append("mtnOptions.callbackSuccess = callbackSuccess;").Append(GetNewLineTab(1)).
                    Append("mtnOptions.callbackError = callbackError;").Append(GetNewLineTab(1)).
                    Append("mtnOptions.callbackComplete = callbackComplete;").Append(GetNewLineTab(1)).
                    Append("mtnOptions.uploadProgress = uploadProgress;");
                functionBody.Append(GetNewLineTab(1));

                if (parameters.Count > 0)
                {
                    functionBody.Append("mtnOptions.data = {");

                    for (var index = 0; index < parameters.Count; index++)
                    {
                        var parameter = parameters[index];
                        functionBody.Append(GetNewLineTab(2));
                        functionBody.Append((index != 0 ? "," : ""))
                            .Append(parameter.Name).Append(":").Append(parameter.Name);
                    }


                    functionBody.Append(GetNewLineTab(1));
                    functionBody.Append("};");
                }
                functionBody.Append(GetNewLineTab(1));

            }

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                if (parameter.Type.IsPrimitiveMtn() || parameter.Type.FullName.ToLower().Contains("httppostedfile"))
                    continue;

                functionBody.Append(GetNewLineTab(1));
                functionBody.AppendFormat("mtnOptions.data.{0} = JSON.stringify(mtnOptions.data.{0});", parameter.Name);
                functionBody.Append(GetNewLineTab(1));
            }

            functionBody.AppendFormat("mtnOptions.data.__MtnClassHash = '{0}';", ajaxClass.Key);
            functionBody.Append(GetNewLineTab(1));
            functionBody.AppendFormat("mtnOptions.data.__MtnMethodHash = '{0}';", method.Key);
            functionBody.Append(GetNewLineTab(1));
            switch (Mode)
            {
                case WebModeType.AjaxHandler:
                    functionBody.Append("mtnOptions.url='")
                                        .Append(HostUrl).Append("/")
                                        .Append(classNamespace.ToLowerInvariant()).Append(".").Append(methodName)
                                        .Append(".").Append(Utils.Parameter.AjaxExtension)
                                        .Append("';");
                    break;
                case WebModeType.MvcController:
                    functionBody.Append("mtnOptions.url='")
                                        .Append(HostUrl).Append("/{MtnMvcRoute}")
                                        .Append(classNamespace.Replace(".", Web.Utils.Parameter.SeoCharacter).ToLowerInvariant()).Append("/").Append(methodName.ToLowerInvariant())
                                        .Append("/';");
                    break;
                default:
                    functionBody.Append("mtnOptions.url='")
                                        .Append(HostUrl).Append("/{MtnMvcRoute}")
                                        .Append(classNamespace.Replace(".", Web.Utils.Parameter.SeoCharacter).ToLowerInvariant()).Append("/").Append(methodName.ToLowerInvariant())
                                        .Append("/';");
                    break;
            }
            functionBody.Append(GetNewLineTab(1));

            functionBody.AppendFormat("mtnOptions.responseType = '{0}';", method.AjaxMethod.ResponseType.ToString().ToLowerInvariant());
            functionBody.Append(GetNewLineTab(1));
            string ajaxTmpl = "";
            switch (method.AjaxMethod.RequestType)
            {
                case RequestType.Post:
                    ajaxTmpl = AjaxPostFunctionTemplate;
                    break;
                case RequestType.Get:
                    ajaxTmpl = AjaxGetFunctionTemplate;
                    break;
            }
            ajaxTmpl = String.Format(ajaxTmpl, HasFileUpload(parameters).ToString().ToLower());
            functionBody.AppendLine(ajaxTmpl);

            functionBody.AppendLine("};");
            if (RootNamespace.IsNullOrWhiteSpaceMtn())
            {
                sbWork.AppendFormat("{0}.{1} = {2}", classNamespace, methodName, functionBody.ToString());
            }
            else
            {
                sbWork.AppendFormat("{0}.{1}.{2}  = {3}", RootNamespace, classNamespace, methodName, functionBody.ToString());
            }
            sbWork.AppendLine();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual string GetContentType(ResponseType responseType)
        {
            var result = "";
            switch (responseType)
            {
                case ResponseType.Json:
                    result = "application/json";
                    break;
                case ResponseType.Xml:
                    result = "application/xml";
                    break;
                case ResponseType.Text:
                    result = "text/html";
                    break;
                case ResponseType.JsonP:
                    result = "application/jsonp";
                    break;
                case ResponseType.Html:
                    result = "text/html";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("responseType");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual Boolean HasFileUpload(IEnumerable<Parameter> parameters)
        {
            if (parameters == null) 
                return false;
            return parameters.Any(x => x.Type.FullName.Contains("HttpPostedFile"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual String GetAuxiliarFunctions()
        {
            var strScript = !Web.Utils.Parameter.DisableAjaxDebug ?
                Utils.Minifier.MinifyCode(this.AjaxAuxFunctionTemplate) 
                : this.AjaxAuxFunctionTemplate;
            return strScript;

        }
    }
}
