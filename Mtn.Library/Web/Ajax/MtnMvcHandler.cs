using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Mtn.Library.ExtensionsEntity;
using Mtn.Library.Extensions;
using Mtn.Library.Web.Enums;

namespace Mtn.Library.Web.Ajax
{
    /// <summary>
    /// 
    /// </summary>
    public class MtnMvcHandler:AjaxHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        public MtnMvcHandler(string route)
        {
            webmodeType = WebModeType.MvcController;
            Config.RouteCollection.Add(route);
        }
    }
}