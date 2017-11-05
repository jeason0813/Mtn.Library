using System;
using System.Collections.Generic;
using System.Text;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;

#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif

namespace Mtn.Library.Web.Scripts.Template
{
    /// <summary>
    /// 
    /// </summary>
    public class JQuery : BaseTemplate
    {
        private string _jQueryPrefix = "$";


        /// <summary>
        /// if you need change the default prefix for jquery ("$")
        /// </summary>
        public String JQueryPrefix
        {
            get { return _jQueryPrefix; }
            set { _jQueryPrefix = value; }
        }

        #region AjaxAuxFunctionTemplate
        
        /// <summary>
        /// Javascript code necessary for the other templates, will be create once 
        /// </summary>
        public override string AjaxAuxFunctionTemplate
        {
            get
            {
                const string strFunc = @"
Mtn.jquery = {};
Mtn.jquery.ajax = function (mtnOptions, type, hasUpload) {
    var useFormData = (typeof FormData === 'function');
    if(typeof mtnOptions.callbackSuccess === 'undefined') {
        if(typeof mtnOptions.success !== 'undefined') 
            mtnOptions.callbackSuccess = mtnOptions.success;
        else
            mtnOptions.callbackSuccess = function() {};
    }

    if(typeof mtnOptions.callbackError === 'undefined') {
        if(typeof mtnOptions.error !== 'undefined') 
            mtnOptions.callbackError = mtnOptions.error;
        else
            mtnOptions.callbackError = function() {};
    }

    if(typeof mtnOptions.callbackComplete === 'undefined') {
        if(typeof mtnOptions.complete !== 'undefined') 
            mtnOptions.callbackComplete = mtnOptions.complete;
        else
            mtnOptions.callbackComplete = function() {};
    }

    if (hasUpload) {
        var id = Mtn.id();
        var divId = 'div-' + id;
        var iframeId = 'iframe-' + id;
        var formId = 'form-' + id;
        if (typeof FormData === 'function') {
            data = new FormData();
            useFormData = true;
        } else {
            {0}('body').append('<div id=""' + divId + '"" style=""display:none"" width=""0"" height=""0""></div>').end();
            {0}('#' + divId).append('<iframe id=""' + iframeId + '"" name=""' + iframeId + '"" style=""display:none""  width=""0"" height=""0""></iframe>').end();
            {0}('#' + divId).append('<form enctype=""multipart/form-data"" method=""POST"" target=""' + iframeId + '"" action=""' + mtnOptions.url + '"" id=""' + formId + '"" style=""display:none"" width=""0"" height=""0"" ></form>').end();
        }

        for (var propName in mtnOptions.data) {
            var propData = mtnOptions.data[propName];
            if (propData.toString().substring(0, 1) === '#') {
                var isFile = (useFormData && {0}(propData)[0].files.length != 0);
                if (isFile) {
                    var isSingleFile = ((typeof {0}(propData).prop('multiple') === 'undefined') || Mtn.support.multipleFile == false);
                    if (isSingleFile) {
                        data.append(propName, {0}(propData)[0].files[0]);
                    } else {
                        {0}.each({0}(propData)[0].files, function (i, file) {
                            data.append(propName + '[' + i + ']', file);
                        });
                    }
                } else {
                    if (useFormData) {
                        var value = {0}(propData).val() || {0}(propData).html();
                        data.append(propName, value);
                    } else {
                        var propForm     = {0}(propData);
                        var propNewInput = {0}(propForm).clone(true);
                        propForm.hide();
                        propNewInput.insertAfter(propForm);
                        propForm.prop('name', propName);
                        {0}('#' + formId).append(propForm).end();
                    }
                }
            } else {
                var isToJson = (typeof propData == 'object');
                if (useFormData) {
                    data.append(propName, (isToJson ? encodeURI(JSON.stringify(propData)) : propData));
                } else {
                    {0}('#' + formId).append('<input type=""hidden"" value=""' + (isToJson ? encodeURI(JSON.stringify(propData)) : propData) + '"" name=""' + propName + '"" />').end();
                }
            }
        }
        
        

        if (useFormData) {
            mtnOptions.data = data;            
            var settings = {0}.extend(true, {}, {0}.ajaxSettings, {
                type: 'POST',
                url: mtnOptions.url,
                success: mtnOptions.callbackSuccess,
                error: mtnOptions.callbackError,
                complete: mtnOptions.callbackComplete,
                uploadProgress:((typeof mtnOptions.uploadProgress !== 'undefined') ? mtnOptions.uploadProgress : function() {
                }),
                data: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null) ? undefined : mtnOptions.data),
                dataType: mtnOptions.responseType,
                contentType: false,
                processData: false,
                cache: false
            });
            if(Mtn.support.uploadXhr) {
                settings.xhr =  function() {
                    var xhr = {0}.ajaxSettings.xhr();
                    var me = this;
                    xhr.upload.onprogress = function (event) {
                        var totalSize = event.total;                
                        var percentProgress = 0;
                        var filePosition = event.loaded || event.position; 
                
                        if (event.lengthComputable) 
                            percentProgress = Math.ceil(filePosition / totalSize * 100);
                        
                        if(me.uploadProgress)
                          me.uploadProgress(event, filePosition, totalSize, percentProgress);
                    }
                    return xhr;
                }
            }
            {0}.ajax(settings);
        } else {

            Mtn.ns('Mtn.callbacks');
            var mtnFunctionName = 'callbackMtn' + id;
            mtnOptions.callbackSuccess = mtnOptions.callbackSuccess;
            Mtn.callbacks[mtnFunctionName] = function () {
                mtnOptions.callbackSuccess(arguments);
                {0}('#' + divId).remove();
            };
            {0}('#' + iframeId).load(Mtn.callbacks[mtnFunctionName]);
            {0}('#' + formId).submit();

        }
    } else if (type == 'post') {        
        {0}.ajax({
            type: 'POST',
            url: mtnOptions.url,
            success: mtnOptions.callbackSuccess,
            error: mtnOptions.callbackError,
            complete: mtnOptions.callbackComplete,
            data: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null) ? undefined : mtnOptions.data),
            dataType: mtnOptions.responseType,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            cache: ((typeof mtnOptions.cache === 'undefined' || mtnOptions.cache == null) ? false : mtnOptions.cache)
        });
    } else if (type == 'get') {
        {0}.ajax({
            type: 'GET',
            url: mtnOptions.url,
            success: mtnOptions.callbackSuccess,
            error: mtnOptions.callbackError,
            complete: mtnOptions.callbackComplete,
            data: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null) ? undefined : mtnOptions.data),
            dataType: mtnOptions.responseType,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            cache: ((typeof mtnOptions.cache === 'undefined' || mtnOptions.cache == null) ? false : mtnOptions.cache)
        });
    } else if (type == 'jsonp') {

        $.ajax({
            type: 'GET',
            jsonp: '__MtnJsonpCallback',
            url: mtnOptions.url,
            success: mtnOptions.callbackSuccess,
            error: mtnOptions.callbackError,
            complete: mtnOptions.callbackComplete,
            data: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null) ? undefined : mtnOptions.data),
            dataType: 'jsonp',
            crossDomain: true,
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            cache: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null) ? {} : mtnOptions.data)
        });
    }
};
";
                return strFunc.Replace("{0}",JQueryPrefix);
            }
        }
        #endregion

        /// <summary>
        /// Template for Get calls
        /// </summary>
        public override String AjaxGetFunctionTemplate
        {
            get
            {
                return "Mtn.jquery.ajax(mtnOptions,'get',{0});";
            }
        }
        


        /// <summary>
        /// Template for Post calls
        /// </summary>
        public override String AjaxPostFunctionTemplate
        {
            get
            {
                return "Mtn.jquery.ajax(mtnOptions,'post',{0});";
            }
        }

        /// <summary>
        /// Template for Form call
        /// </summary>
        public override String AjaxFormFunctionTemplate
        {
            get { return @""; }
        }

        /// <summary>
        /// Template for JsonP call
        /// </summary>
        public override String AjaxJsonPFunctionTemplate
        {
            get { return @"Mtn.jquery.ajax(mtnOptions,'jsonp',{0});"; }
        }

        /// <summary>
        /// Create the method body
        /// </summary>
        /// <param name="sbWork">StringBuilder to append method</param>
        /// <param name="classNamespace">class name</param>
        /// <param name="methodName">method name</param>
        /// <param name="method">Ajax item method</param>
        /// <param name="parameters">List of parameters</param>
        /// <param name="ajaxClass"> Ajax class item</param>
        /// <param name="useTraditionalParameterForm"> Use traditional parameters, like function(parm1,parm2,parm3) instead function(options), calling myFunc({data:'data'});</param>
        /// <returns></returns>
        public override void CreateMethodBody(ref System.Text.StringBuilder sbWork, string classNamespace, string methodName, AjaxMethodItem method, IList<Entities.Parameter> parameters, AjaxClassItem ajaxClass, bool useTraditionalParameterForm = false)
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
                functionBody.Append((parameters.Count >0 ?",":"") + "callbackSuccess,callbackError,callbackComplete,uploadProgress");
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

                
#if ! UseSystemExtensions
            if ( Mtn.Library.Extensions.SystemExtensions.IsPrimitiveMtn(parameter.Type) || parameter.Type.FullName.ToLower().Contains("httppostedfile"))
                    continue;
#else
                if (parameter.Type.IsPrimitiveMtn()  || parameter.Type.FullName.ToLower().Contains("httppostedfile"))
                    continue;
#endif
                functionBody.Append(GetNewLineTab(1));
                functionBody.AppendFormat("mtnOptions.data.{0} = JSON.stringify(mtnOptions.data.{0});",parameter.Name);
                functionBody.Append(GetNewLineTab(1));
            }

            functionBody.AppendFormat("mtnOptions.data.__MtnClassHash = '{0}';", ajaxClass.Key);
            functionBody.Append(GetNewLineTab(1));
            functionBody.AppendFormat("mtnOptions.data.__MtnMethodHash = '{0}';",method.Key );
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
                                        .Append(classNamespace.Replace(".", Utils.Parameter.SeoCharacter).ToLowerInvariant()).Append("/").Append(methodName.ToLowerInvariant())
                                        .Append("/';");
                    break;
                default:
                    functionBody.Append("mtnOptions.url='")
                                        .Append(HostUrl).Append("/{MtnMvcRoute}")
                                        .Append(classNamespace.Replace(".", Utils.Parameter.SeoCharacter).ToLowerInvariant()).Append("/").Append(methodName.ToLowerInvariant())
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
            switch (method.AjaxMethod.ResponseType)
            {
                case ResponseType.Json:
                    break;
                case ResponseType.Xml:
                    break;
                case ResponseType.Text:
                    break;
                case ResponseType.JsonP:
                    ajaxTmpl = AjaxJsonPFunctionTemplate;
                    break;
                case ResponseType.Html:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

    }
}