using System;
using System.Collections.Generic;
using System.Text;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;

namespace Mtn.Library.Web.Scripts.Template
{
    /// <summary>
    /// Template for Script
    /// </summary>
    public interface IScriptTemplate
    {
        /// <summary>
        /// Url to call, can be changed for external calls, normaly you don't need change this
        /// </summary>
        String HostUrl { get; set; }
        
        /// <summary>
        /// Web mode - represents how you use this functionality, like MVC or WEBForms with ajaxhandler, etc
        /// </summary>
        WebModeType Mode { get; set; }

        
        /// <summary>
        /// Javascript code necessary for the other templates, will be create once 
        /// </summary>
        String AjaxAuxFunctionTemplate { get; set; }

        /// <summary>
        /// Template for Get calls
        /// </summary>
        String AjaxGetFunctionTemplate { get; set; }
        
        /// <summary>
        /// Template for Post calls
        /// </summary>
        String AjaxPostFunctionTemplate { get; set; }

        /// <summary>
        /// Template for Form call
        /// </summary>
        String AjaxFormFunctionTemplate { get; set; }

        /// <summary>
        /// Template for JsonP call
        /// </summary>
        String AjaxJsonPFunctionTemplate { get; set; }

        /// <summary>
        /// New line, normaly "\n"
        /// </summary>
        String NewLine { get; set; }

        /// <summary>
        /// Tabulation, normaly "\t"
        /// </summary>
        String Tabulation { get; set; }

        /// <summary>
        /// Your RootNamespace ("prefix"), normaly your company like "Metanoia, to use in fucntions like "Metanoia.MyClass.MyMethod"
        /// </summary>
        String RootNamespace { get; }

        /// <summary>
        /// Returns New line and tabulation
        /// </summary>
        /// <param name="tabCount">How many tabs you want</param>
        /// <returns>New line and tabulations </returns>
        String GetNewLineTab(Int32 tabCount );

        /// <summary>
        /// Returns the initial StringBuilder with your default namespace
        /// </summary>
        /// <param name="rootNamespace">Root Namespace </param>
        /// <param name="hostUrl">Your host to calls,normaly you need do nothing here </param>
        StringBuilder Init(String rootNamespace, String hostUrl = null);

        /// <summary>
        /// AppendClass Namespace
        /// </summary>
        /// <param name="sbWork">StringBuilder to append classNamespace</param>
        /// <param name="classNamespace">class name</param>
        void AppendClass(ref StringBuilder sbWork, String classNamespace);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbWork">StringBuilder to append method</param>
        /// <param name="classNamespace">class name</param>
        /// <param name="methodName">method name</param>
        /// <param name="method">Ajax method item</param>
        /// <param name="parameters">List of parameters</param>
        /// <param name="ajaxClass"> Ajax class item</param>
        /// <param name="useTraditionalParameterForm"> Use traditional parameters, like function(parm1,parm2,parm3) instead function(options), calling myFunc({data:'data'});</param>
        /// <returns></returns>
        void CreateMethodBody(ref StringBuilder sbWork, String classNamespace, String methodName, AjaxMethodItem method, IList<Entities.Parameter> parameters, AjaxClassItem ajaxClass, bool useTraditionalParameterForm = false);

        /// <summary>
        /// Return auxiliar functions
        /// </summary>
        /// <returns></returns>
        String GetAuxiliarFunctions();
    }
}