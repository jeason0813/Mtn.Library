using System;
using System.Linq;
using Mtn.Library.Extensions;
using Mtn.Library.Entities;
namespace Mtn.Library.Web.Page
{
    /// <summary>
    /// xx
    /// </summary>
    public class LoggedPage:BasePage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            if (Library.Attributes.Authorization.ValidateAction != null && Library.Attributes.Authorization.ValidateAction() == false)
            {
                if (Utils.Parameter.UnauthorizedPage.IsNullOrWhiteSpaceMtn() == false)
                    this.Response.Redirect(Utils.Parameter.UnauthorizedPage, true);
                else
                {
                    string msg  = null;
                    try
                    {
                        msg = (string) Library.Attributes.Authorization.UnauthorizedValue;
                    }
                    catch (Exception ex)
                    {
                        Service.Statistics.Add(ex.GetAllMessagesMtn());
                    }
                    if (msg.IsNullOrWhiteSpaceMtn())
                        msg = new DataPage<string>().GetUnauthorizedPage().Message;
                    if (msg != null) Response.Write(msg);
                    Response.End();
                    return;
                }
            }

            // Check permissions
            var attrs = GetType().GetCustomAttributes(typeof(Library.Attributes.Permission), true);
            if (attrs.Any())
            {
                foreach (var attr in attrs)
                {
                    var attrP = (attr as Library.Attributes.Permission);
                    if (attrP != null && attrP.HasPermission(attrP.Value1, attrP.Value2, attrP.Value3, attrP.Value4) == false)
                    {
                        if (Utils.Parameter.UnallowedPage.IsNullOrWhiteSpaceMtn() == false)
                            Response.Redirect(Utils.Parameter.UnallowedPage, true);
                        else
                        {
                            string msg = null;
                            try
                            {
                                msg = (string)Library.Attributes.Permission.UnallowedResult;
                            }
                            catch (Exception ex)
                            {
                                Service.Statistics.Add(ex.GetAllMessagesMtn());
                            }

                            if (msg.IsNullOrEmptyMtn(true))
                                msg = new DataPage<string>().GetUnallowedPage().Message;
                            if (msg != null) Response.Write(msg);
                            Response.End();
                        }
                    }
                }
            }
            base.OnPreInit(e);
        }
    }
}
