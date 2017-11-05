using System;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Scripts.Template;

namespace Mtn.Library.Web.Attributes
{
    

    #region AjaxAttribute

    /// <summary>
    /// <para>Using this attribute in the class will indicate that it must be interpreted as a code ajax</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), System.Runtime.InteropServices.ComVisible(false)]
    public class AjaxClassAttribute : Attribute
    {
        
        /// <summary>
        /// Your RootNamespace ("prefix"), normaly your company like "Metanoia, to use in javascript functions like "Metanoia.MyClass.MyMethod"
        /// </summary>
        public String RootNameSpace { get; set; }

        /// <summary>
        /// <para>Indicates the name to be called via javascript / ajax</para>
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Indicates if will write all scripts in body, or create a link for javascript tag
        /// </summary>
        public Boolean WriteOnBody { get; set; }

        /// <summary>
        /// Indicates if will generate a code without minify
        /// </summary>
        public Boolean DebugMode { get; set; }
        /// <summary>
        /// Only for needed for Custom Script Processor type
        /// </summary>
        public IScriptTemplate ScriptTemplate { get; set; }

        private ScriptProcessorType _scriptProcessorType = ScriptProcessorType.JQuery;
        /// <summary>
        /// Script processor for Ajax
        /// </summary>
        public ScriptProcessorType ScriptProcessor
        {
            get { return _scriptProcessorType; }
            set { _scriptProcessorType = value; }
        }

        /// <summary>
        /// Use traditional parameters, like function(parm1,parm2,parm3) instead function(options), calling myFunc({data:'data'});
        /// </summary>
        public Boolean UseTraditionalParameterForm { get; set; }

        /// <summary>
        /// Tag to find the class, if null will be a name with md5 hash (base 64), but you can force to any word you like, beware to avoid the same name for another ajax class in your project 
        /// </summary>
        public String HashTag { get; set; }

        /// <summary>
        /// Prefix for library, like jQuery instead $, default is the community default for library
        /// </summary>
        public String ProcessorPrefix { get; set; }

        /// <summary>
        /// <para>Using this attribute in the class will indicate that it must be interpreted as a code ajax</para>
        /// </summary>
        public AjaxClassAttribute()
        {   
        }

    }
    #endregion

    #region AjaxMethod
    /// <summary>
    /// <para>Using this attribute in the class will indicate that it must be interpreted as a ajax code.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false), System.Runtime.InteropServices.ComVisible(false)]
    public class AjaxMethod : System.Attribute
    {
        #region Attributes

        private RequestType _requestType = RequestType.Get;
        private ResponseType _responseType = ResponseType.Json;
        #endregion

        #region properties
        /// <summary>
        /// <para>Indicates the type of request that will be used.</para>
        /// </summary>
        public RequestType RequestType { get { return _requestType; } set { _requestType = value; } }
        /// <summary>
        /// <para>Indicates the return type</para>
        /// </summary>
        public ResponseType ResponseType { get { return _responseType; } set { _responseType = value; } }

        /// <summary>
        /// <para>Indicates the name to be called via javascript / ajax.</para>
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// <para>Indicates indented mode in Json return. default false .</para>
        /// </summary>
        public bool Indented { get; set; }

        /// <summary>
        /// <para>IgnoreNullData.</para>
        /// </summary>
        public bool IgnoreNullData { get; set; }

        /// <summary>
        /// Indicates if wil use cache based in cache atribute
        /// </summary>
        public Boolean UseCache { get; set; }

        /// <summary>
        /// Tag to find a method, if null will be a name with md5 hash (base 64), but you can force to any word you like, beware to avoid the same name for another methods in ajax class
        /// </summary>
        public String HashTag { get; set; }

        #endregion

        /// <summary>
        /// <para>Using this attribute in the class will indicate that it must be interpreted as a ajax code.</para>
        /// </summary>
        public AjaxMethod()
        {
            Indented = false;
        }
    }
    #endregion
}


