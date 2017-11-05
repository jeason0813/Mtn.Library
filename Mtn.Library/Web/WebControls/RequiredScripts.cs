using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Scripts;

namespace Mtn.Library.Web.WebControls
{
    /// <summary>
    /// <para>Class to generate code to access the Ajax Methods.</para>
    /// </summary>
    [ToolboxData("<{0}:MtnScripts runat=server ></{0}:MtnScripts>")]
    public class RequiredScript : WebControl
    {
        #region properties
       
        /// <summary>
        /// <para>If true will minify the code. Default is false.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(true)]
        [Localizable(true)]
        public bool Minify { get; set; }

        /// <summary>
        /// <para>If true will minify the code. Default is false.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(false)]
        [Localizable(true)]
        public bool WriteOnBody { get; set; }

       
        
        #endregion

        #region Render
        /// <summary>
        /// <para>Render the code.</para>
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.BeginRender();
            var script = ScriptProcessorEngine.GetRequiredCode("", this.WriteOnBody, this.Minify,WebModeType.AjaxHandler);
            writer.Write(script);
            writer.EndRender();
        }

        
        #endregion
        

    }
}