using System;
using System.Collections.Generic;
using System.Reflection;
using Mtn.Library.Web.Attributes;

namespace Mtn.Library.Web.Entities
{
    /// <summary>    
    /// <para>This class represents a Ajax method Item.</para>
    /// </summary>
    public class AjaxMethodItem
    {
        /// <summary>
        /// <para>Key for cache.</para>
        /// </summary>
        public String Key { get; set; }
        /// <summary>
        /// <para>Method.</para>
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// <para>Constructor.</para>
        /// </summary>
        public string ConstructorHash { get; set; }
        /// <summary>
        /// <para>AjaxMethod.</para>
        /// </summary>
        public AjaxMethod AjaxMethod { get; set; }

        /// <summary>
        /// <para>List of parameters.</para>
        /// </summary>
        public IList<Entities.Parameter> Parameters { get; set; }
        
    }
}
