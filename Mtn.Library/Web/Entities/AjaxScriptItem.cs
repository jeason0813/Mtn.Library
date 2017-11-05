using System;
using System.Collections.Generic;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Scripts.Template;

namespace Mtn.Library.Web.Entities
{
    /// <summary>
    /// Script for ajax class
    /// </summary>
    public class AjaxScriptItem
    {
        /// <summary>
        /// <para>Key for cache.</para>
        /// </summary>
        public String Key { get; set; }
        
        /// <summary>
        /// <para>Key for ajax class cache.</para>
        /// </summary>
        public String AjaxClassKey { get; set; }

        /// <summary>
        /// Only for needed for Custom Script Processor type
        /// </summary>
        public IScriptTemplate ScriptTemplate { get; set; }

        /// <summary>
        /// Prefix for library, like jQuery instead $, default is the community default for library
        /// </summary>
        public String ProcessorPrefix { get; set; }

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
        /// Full script, with class and all methods
        /// </summary>
        public String Script { get; set; }
        /// <summary>
        /// Full script, with class and all methods
        /// </summary>
        public String ScriptMinified { get; set; }
        /// <summary>
        /// Full script, with class and all methods
        /// </summary>
        public String FullScript { get; set; }
        /// <summary>
        /// Full script, with class and all methods minified
        /// </summary>
        public String FullScriptMinified { get; set; }

        private SortedDictionary<string, AjaxScriptMethodItem> _ajaxScriptMethodCollection =
            new SortedDictionary<string, AjaxScriptMethodItem>();

        /// <summary>
        /// Collection of AjaxScriptMethodItem
        /// </summary>
        public SortedDictionary<String, AjaxScriptMethodItem> AjaxScriptMethodCollection
        {
            get { return _ajaxScriptMethodCollection; }
            set { _ajaxScriptMethodCollection = value; }
        }
    }

    /// <summary>
    /// Script fot ajaxMethod
    /// </summary>
    public class AjaxScriptMethodItem
    {
        /// <summary>
        /// <para>Key for cache.</para>
        /// </summary>
        public String Key { get; set; }

        /// <summary>
        /// Script, with method
        /// </summary>
        public String Script { get; set; }
        /// <summary>
        /// Script, with method minified
        /// </summary>
        public String ScriptMinified { get; set; }
    }
}
