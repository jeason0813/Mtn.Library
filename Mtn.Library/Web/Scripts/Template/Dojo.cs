using System;
using System.Collections.Generic;
using System.Text;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;
using Mtn.Library.Extensions;

namespace Mtn.Library.Web.Scripts.Template
{
    /// <summary>
    /// 
    /// </summary>
    public class Dojo : BaseTemplate
    {

        /// <summary>
        /// Javascript code necessary for the other templates, will be create once 
        /// </summary>
        public override string AjaxAuxFunctionTemplate
        {
            get
            {
                return
                    @"Mtn.dojo = {};
Mtn.dojo.isLessVersion = !(dojo.version.major == 1 && dojo.version.minor >= 7);
Mtn.dojo.ajax = function (mtnOptions, type, hasUpload) {
    var useFormData = (typeof FormData === 'function');
    if (hasUpload) {
        var id = Mtn.id();
        var divId = 'div-' + id;
        var iframeId = 'iframe-' + id;
        var formId = 'form-' + id;
        
        var strIframe = '<iframe id=""' + iframeId + '"" name=""' + iframeId + '"" style=""display:none""  width=""0"" height=""0""></iframe>';
        var strForm = '<form enctype=""multipart/form-data"" method=""POST"" target=""' + iframeId + '"" action=""' + mtnOptions.url + '"" id=""' + formId + '"" style=""display:none"" width=""0"" height=""0"" ></form>';
        var divContent = strIframe + '<br/>' + strForm;
        dojo.create('div', {id:divId,style:'display:none',width:0,height:0, innerHTML:divContent }, dojo.body());
         
        var divCreated = dojo.query('#' + divId)[0];
        var iframeCreated = dojo.query('#' + iframeId)[0];
        var formCreated = dojo.query('#' + formId)[0];

        for (var propName in mtnOptions.data) {
            var propData = mtnOptions.data[propName];
            if (propData.toString().substring(0, 1) === '#') {                
                var fileInput = dojo.query(propData)[0];
                var fpClone = dojo.clone(fileInput);                
                dojo.attr(fileInput,{'style': 'display:none'});
                
                dojo.place(fpClone,fileInput,'after')                
                dojo.attr(fileInput,{'name': propName});
                
                dojo.place(fileInput,formCreated);
                
            } else {
                var isToJson = (typeof propData == 'object');
                dojo.create('input',{type:'hidden', value: (isToJson ? encodeURI(JSON.stringify(propData)) : propData) ,  name: propName},formCreated);
            }
        }
        Mtn.ns('Mtn.callbacks');
        var mtnFunctionName = 'callbackMtn' + id;
        mtnOptions.callbackSuccess = ((typeof mtnOptions.callbackSuccess !== 'undefined') ? mtnOptions.callbackSuccess : function () { });
        Mtn.callbacks[mtnFunctionName] = function () {
            mtnOptions.callbackSuccess(arguments);
            dojo.destroy(divCreated);
        };
        iframeCreated.onload = Mtn.callbacks[mtnFunctionName];
        formCreated.submit();
        
        
    } else if (type == 'post') {
        var post = {};        
        if(Mtn.dojo.isLessVersion){
            dojo.xhrPost({
                url:mtnOptions.url,
                content:((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
                handle:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
                load: ((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
                error:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
                headers :{Accept:mtnOptions.responseType,'Content-type': 'application/x-www-form-urlencoded; charset=utf-8'}
            });
        }
        else {
            require([""dojo/_base/xhr""], function(xhr){		
                 xhr.post({
                    url:mtnOptions.url,
                    content:((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
                    handle:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
                    load: ((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
                    error:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
                    headers :{Accept:mtnOptions.responseType,'Content-type': 'application/x-www-form-urlencoded; charset=utf-8'}
                });
            });
        }
    } else if (type == 'get') {
        if(Mtn.dojo.isLessVersion){
            dojo.xhrGet({
                url:mtnOptions.url,
                content:((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
                handle:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
                load: ((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
                error:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
                headers :{Accept:mtnOptions.responseType,'Content-type': 'application/x-www-form-urlencoded; charset=utf-8'},
                preventCache:((typeof mtnOptions.cache === 'undefined' || mtnOptions.cache == null)?true:mtnOptions.cache)                
            });
        }
        else {
            require([""dojo/_base/xhr""], function(xhr){		
            xhr.get({
                    url:mtnOptions.url,
                    content:((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
                    handle:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
                    load: ((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
                    error:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
                    headers :{Accept:mtnOptions.responseType,'Content-type': 'application/x-www-form-urlencoded; charset=utf-8'},
                    preventCache:((typeof mtnOptions.cache === 'undefined' || mtnOptions.cache == null)?true:mtnOptions.cache)                
                });
            });
        }
    }else if (type == 'jsonp') {
        var idJsonp =  Mtn.id();
        var jsonCalbackName = 'jsonCallBack' + idJsonp.replace(/-/g,'');
        Mtn.dojo[jsonCalbackName] = function (data,a,b,c,d){
            mtnOptions.callbackSuccess(data,a,b,c,d);
        };
        var fullJsonCalbackName = 'Mtn.dojo.' + jsonCalbackName;
        var paramsUrl = mtnOptions.url.split('?');
        mtnOptions.url = paramsUrl[0];
        paramsUrl = paramsUrl.length >1 ? '?' + paramsUrl[1]:'?';
        for(var name in mtnOptions.data)
        {
            paramsUrl+= paramsUrl.length > 1?'&':'';
            paramsUrl+= name + '=' + encodeURI(mtnOptions.data[name]);
        };
        mtnOptions.url += paramsUrl + '&__MtnJsonpCallback=' + fullJsonCalbackName;
        dojo.create('script',  {id:idJsonp, src:mtnOptions.url,type:'text/javascript'}, dojo.body());
        
    }
};";
            }
        }

        /// <summary>
        /// Template for Get calls
        /// </summary>
        public override String AjaxGetFunctionTemplate
        {
            get
            {
                return "Mtn.dojo.ajax(mtnOptions,'post',{0});";

            }
        }

        /// <summary>
        /// Template for Post calls
        /// </summary>
        public override String AjaxPostFunctionTemplate
        {
            get
            {
                return "Mtn.dojo.ajax(mtnOptions,'post',{0});";
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
            get { return "Mtn.dojo.ajax(mtnOptions,'jsonp',{0});"; }
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