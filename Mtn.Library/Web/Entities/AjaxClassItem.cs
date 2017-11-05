using System;
using System.Collections.Generic;
using Mtn.Library.Web.Attributes;

namespace Mtn.Library.Web.Entities
{
    /// <summary>
    /// Ajax class item to create ajax scripts and calls
    /// </summary>
    public class AjaxClassItem
    {
        /// <summary>
        /// <para>Key for cache.</para>
        /// </summary>
        public String Key { get; set; }
       
        /// <summary>
        /// Class name
        /// </summary>
        public String ClassName { get; set; }
        /// <summary>
        /// AjaxAttribute
        /// </summary>
        public AjaxClassAttribute AjaxClass { get; set; }

        private Dictionary<string, AjaxMethodItem> _ajaxMethodItemCollection = new Dictionary<string, AjaxMethodItem>();

        /// <summary>
        /// List of methods
        /// </summary>
        public Dictionary<String,AjaxMethodItem> AjaxMethodItemCollection
        {
            get { return _ajaxMethodItemCollection; }
            set { _ajaxMethodItemCollection = value; }
        }
    }
}
