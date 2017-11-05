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
    public class ExtJs : BaseTemplate
    {

        /// <summary>
        /// Javascript code necessary for the other templates, will be create once 
        /// </summary>
        public override string AjaxAuxFunctionTemplate
        {
            get
            {
                const string strFunc =
                    @"Mtn.extjs = {};
Mtn.extjs.ajax = function (mtnOptions, type, hasUpload) {
    var useFormData = (typeof FormData === 'function');
    if (hasUpload) {
        var id = Mtn.id();
        var divId = 'div-' + id;
        var iframeId = 'iframe-' + id;
        var formId = 'form-' + id;
        
        Ext.DomHelper.append(Ext.getBody(),'<div id=""' + divId + '"" style=""display:none"" width=""0"" height=""0""></div>');
        Ext.DomHelper.append(divId,'<iframe id=""' + iframeId + '"" name=""' + iframeId + '"" style=""display:none""  width=""0"" height=""0""></iframe>');
        Ext.DomHelper.append(divId,'<form enctype=""multipart/form-data"" method=""POST"" target=""' + iframeId + '"" action=""' + mtnOptions.url + '"" id=""' + formId + '"" style=""display:none"" width=""0"" height=""0"" ></form>');
        
        for (var propName in mtnOptions.data) {
            var propData = mtnOptions.data[propName];
            if (propData.toString().substring(0, 1) === '#') {
                propData = propData.substring(1, propData.length);
                var fileInput = Ext.get(propData).dom;
                var propNewInput = Ext.clone(fileInput);
                        Ext.get(propData).hide();
                        Ext.get(propNewInput).insertAfter(fileInput);
               
                Ext.get(propData).set({'name': propName});
                Ext.get(formId).appendChild(fileInput);
                    
               
            } else {
                var isToJson = (typeof propData == 'object');
                Ext.DomHelper.append(formId,'<input type=""hidden"" value=""' + (isToJson ? encodeURI(JSON.stringify(propData)) : propData) + '"" name=""' + propName + '"" />');
                
            }
        }

        Mtn.ns('Mtn.callbacks');
        var mtnFunctionName = 'callbackMtn' + id;
        mtnOptions.callbackSuccess = ((typeof mtnOptions.callbackSuccess !== 'undefined') ? mtnOptions.callbackSuccess : function () { });
        Mtn.callbacks[mtnFunctionName] = function () {
            mtnOptions.callbackSuccess(arguments);
            Ext.get(divId).remove();
        };
        Ext.get(iframeId).onload = Mtn.callbacks[mtnFunctionName];
        Ext.get(formId).dom.submit();

        
    } else if (type == 'post') {
        Ext.Ajax.request({
            url: mtnOptions.url,
            params: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
            success:((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
            callback:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
            failure:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
            method: 'POST',
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8',
                Accept:mtnOptions.responseType
            }
        });
    } else if (type == 'get') {
        Ext.Ajax.request({
            url: mtnOptions.url,
            params: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
            success:((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
            callback:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
            failure:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
            method: 'GET',
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8',
                Accept:mtnOptions.responseType
            },
            disableCaching:((typeof mtnOptions.cache === 'undefined' || mtnOptions.cache == null)?true:mtnOptions.cache)
        });
    } else if (type == 'jsonp') {
        Ext.data.JsonP.request({
            url: mtnOptions.url,
            callbackKey:'__MtnJsonpCallback',
            params: ((typeof mtnOptions.data === 'undefined' || mtnOptions.data == null)?undefined:mtnOptions.data),
            success:((typeof mtnOptions.callbackSuccess !== 'undefined')?mtnOptions.callbackSuccess:function(){}),
            callback:((typeof mtnOptions.callbackComplete !== 'undefined')?mtnOptions.callbackComplete:function(){}),
            failure:((typeof mtnOptions.callbackError !== 'undefined')?mtnOptions.callbackError:function(){}),
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8',
                Accept:mtnOptions.responseType
            },
            disableCaching:((typeof mtnOptions.cache === 'undefined' || mtnOptions.cache == null)?true:mtnOptions.cache)
        });
    }
};
";
                return strFunc;
            }
        }

        /// <summary>
        /// Template for Get calls
        /// </summary>
        public override String AjaxGetFunctionTemplate
        {
            get
            {
                return "Mtn.extjs.ajax(mtnOptions,'get',{0});";
            }
        }

        /// <summary>
        /// Template for Post calls
        /// </summary>
        public override String AjaxPostFunctionTemplate
        {
            get
            {
                return "Mtn.extjs.ajax(mtnOptions,'post',{0});";
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
            get { return "Mtn.extjs.ajax(mtnOptions,'jsonp',{0});"; }
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