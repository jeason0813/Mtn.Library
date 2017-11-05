using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mtn.Library.Extensions;

namespace Mtn.Library.Web.Ajax.Internals
{
   
   /// <summary>
   /// Utilizada para retornar a versão corrente
   /// </summary>
    [Mtn.Library.Web.Attributes.AjaxClass(Name = "Mtn.Library.Internals.Utilities")]
    public class Utilities
    {
        #region Version
        /// <summary>
        /// Return version checking password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [Mtn.Library.Web.Attributes.AjaxMethod(Name = "version",
            RequestType = Mtn.Library.Web.Enums.RequestType.Post,
            ResponseType = Mtn.Library.Web.Enums.ResponseType.Json)]
        public virtual String Version(String password)
        {
            return !password.Equals(Web.Utils.Parameter.VersionPassword) ? "Password incorrect" : Web.Utils.Parameter.Version.ToString();
        }

       #endregion
    }
}