using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Diagnostics;
using Mtn.Library.Extensions;

namespace Mtn.Library.Web.Page
{
    /// <summary>
    /// <para>Base page with simple features inherited from System.Web.UI.Page.</para>
    /// </summary>
    public class BasePage:System.Web.UI.Page
    {
        #region Attributes
        private Type _mType;
        private readonly Stopwatch _mSwTime = Stopwatch.StartNew();
        private double _mLoadTime;
        #endregion

        #region properties
        /// <summary>
        /// <para>Return this Page Type.</para>
        /// </summary>
        public Type PageType
        {
            get
            {
                if (_mType == null)
                    _mType = this.GetType();
                return _mType;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises the System.Web.UI.Control.PreRender event.
        /// </summary>
        /// <param name="e">
        /// An System.EventArgs object that contains the event data.
        /// </param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            // must be after the prerender to capture the load time of the html page
            this._mLoadTime = this._mSwTime.Elapsed.TotalMilliseconds;

            // needs to be reset and then started to give the loading time of the controls
            this._mSwTime.Reset();
            this._mSwTime.Start();
        }
        /// <summary>
        /// Outputs server control content to a provided System.Web.UI.HtmlTextWriter
        ///     object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">
        /// The System.Web.UI.HTmlTextWriter object that receives the control content.
        /// </param>
        public override void RenderControl(HtmlTextWriter writer)
        {   
            base.RenderControl(writer);
            this.RenderInfo(writer);
        }

        private void RenderInfo(HtmlTextWriter writer)
        {
            if (Utils.Parameter.WriteStatisticsOnBasePage)
            {
                double total = this._mSwTime.Elapsed.TotalMilliseconds;
                writer.Write(string.Format("\r\n<!-- Load Time:{0:0.00000} milliseconds (After Page_Load) -->", this._mLoadTime));
                writer.Write(string.Format("\r\n<!-- Controls Time:{0:0.00000} milliseconds (After RenderControl) -->", total));
                writer.Write(string.Format("\r\n<!-- Total Time:{0:0.00000} milliseconds (Sum both)-->", total + this._mLoadTime));
            }
            if(Utils.Parameter.WritePathOnBasePage)
                writer.Write(string.Format("\r\n<!-- URL: {0} -->", Request.Url.ToString()));
        }
        #endregion

        #region Methods
        
        /// <summary>
        /// <para>Insert script block with tags &lt;script&gt; script code informed in script parameter &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddScriptWithTagsMtn(string script)
        {
            string key = script.Md5Mtn(true);
            AddScriptWithTagsMtn(key, script);
        }
      
        /// <summary>
        /// <para>Insert script block with tags &lt;script&gt; script code informed in script parameter &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Key name to avoid duplicate code.</para>
        /// </param>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddScriptWithTagsMtn(string key, string script)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(key))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.PageType, key, script, true);
            }
        }
   
        /// <summary>
        /// <para>Insert script block without tags &lt;script&gt; &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddScriptWithoutTagsMtn(string script)
        {
            string key = script.Md5Mtn(true);
            AddScriptWithoutTagsMtn(key, script);
        }
  
        /// <summary>
        /// <para>Insert script block without tags &lt;script&gt; &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Key name to avoid duplicate code.</para>
        /// </param>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddScriptWithoutTagsMtn(string key, string script)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(key))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.PageType, key, script, false);
            }
        }
    
        /// <summary>
        /// <para>Insert startup script block with tags &lt;script&gt; script code informed in script parameter &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddStartupScriptWithTagsMtn(string script)
        {
            string key = script.Md5Mtn(true);
            AddStartupScriptWithTagsMtn(key, script);
        }

    
        /// <summary>
        /// <para>Insert startup script block with tags &lt;script&gt; script code informed in script parameter &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Key name to avoid duplicate code.</para>
        /// </param>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddStartupScriptWithTagsMtn(string key, string script)
        {
            if (!Page.ClientScript.IsStartupScriptRegistered(key))
            {
                Page.ClientScript.RegisterStartupScript(this.PageType, key, script, true);
            }
        }
     
        /// <summary>
        /// <para>Insert startup script block without tags &lt;script&gt; &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddStartupScriptWithoutTagsMtn(string script)
        {
            string key = script.Md5Mtn(true);
            AddStartupScriptWithoutTagsMtn(key, script);
        }

    
        /// <summary>
        /// <para>Insert startup script block without tags &lt;script&gt; &lt;/script&gt;.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Key name to avoid duplicate code.</para>
        /// </param>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddStartupScriptWithoutTagsMtn(string key, string script)
        {
            if (!Page.ClientScript.IsStartupScriptRegistered(key))
            {
                Page.ClientScript.RegisterStartupScript(this.PageType, key, script, false);
            }
        }
   
        /// <summary>
        /// <para>Insert OnSubmit script code .</para>
        /// </summary>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddOnSubmitScriptMtn(string script)
        {
            string key = script.Md5Mtn(true);
            AddOnSubmitScriptMtn(key, script);
        }


        /// <summary>
        /// <para>Insert OnSubmit script code.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Key name to avoid duplicate code.</para>
        /// </param>
        /// <param name="script">
        /// <para>Code to insert.</para>
        /// </param>
        public void AddOnSubmitScriptMtn(string key, string script)
        {
            if (!Page.ClientScript.IsOnSubmitStatementRegistered(key))
            {
                Page.ClientScript.RegisterOnSubmitStatement(this.PageType, key, script);
            }
        }



        /// <summary>
        /// <para>Find a control of TEntity type by id on MasterPage.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Type of control.</para>
        /// </typeparam>
        /// <param name="id">
        /// <para>Control ID.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a control of TEntity type or null.</para>
        /// </returns>
        public TEntity GetMasterControlMtn<TEntity>(string id) where TEntity : Control
        {
            if (Master != null)
            {
                Control controlObj;
                controlObj = Master.FindControl(id);
                if (controlObj == null)
                {
                    foreach (Control control in Master.Controls)
                    {
                        if (control is ContentPlaceHolder)
                        {
                            controlObj = control.FindControl(id);
                            if (controlObj != null)
                                return (TEntity) controlObj;
                        }
                    }
                }
                return (TEntity) controlObj;
            }
            else
            {
            }
            return null;
        }

  
        /// <summary>
        /// <para>Find a control of TEntity type by id on Page or MasterPage.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Type of control.</para>
        /// </typeparam>
        /// <param name="id">
        /// <para>Control ID.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a control of TEntity type or null.</para>
        /// </returns>
        public TEntity GetControlMtn<TEntity>(string id) where TEntity : Control
        {
            var controlObj = Page.FindControl(id);
            
            if (controlObj == null)
            {
                controlObj = GetMasterControlMtn<TEntity>(id);
            }            
            return (TEntity)controlObj;
        }

    
        /// <summary>
        /// <para>Find all controls of TEntity type on MasterPage or Page.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Type of control.</para>
        /// </typeparam>
        /// <returns>
        /// <para>Returns a control list of TEntity type.</para>
        /// </returns>
        public IList<TEntity> FindControlsMtn<TEntity>() where TEntity : Control
        {   
            var controls = new List<TEntity>();
            if(Page != null)
                FindControlsMtn<TEntity>(Page, controls);
            if(Master != null)
                FindControlsMtn<TEntity>(Master, controls);

            return controls;
        }

        /// <summary>
        /// <para>Find all controls of TEntity type onbaseControl.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Type of control.</para>
        /// </typeparam>
        /// <param name="baseControl">
        /// <para>Control to be traversed to list the controls of a specific type.</para>
        /// </param>
        /// <returns>
        /// <para>Returns a control list of TEntity type.</para>
        /// </returns>
        public IList<TEntity> FindControlsMtn<TEntity>(Control baseControl) where TEntity : Control
        {
            var controls = new List<TEntity>();
            FindControlsMtn<TEntity>(baseControl, controls);
            
            return controls;
        }

        private void FindControlsMtn<TEntity>(Control baseControl, IList<TEntity> controls) where TEntity : Control
        {
            foreach (Control control in baseControl.Controls)
            {
                if (control is TEntity)
                    controls.Add(control as TEntity);

                if (control.Controls.Count > 0)
                    FindControlsMtn<TEntity>(control, controls);
            }
        }
        #endregion

    }
}
