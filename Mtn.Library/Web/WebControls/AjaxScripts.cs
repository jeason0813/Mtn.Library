using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mtn.Library.Extensions;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Scripts;

namespace Mtn.Library.Web.WebControls
{
    /// <summary>
    /// <para>Class to generate code to access the Ajax Methods.</para>
    /// </summary>
    [ToolboxData("<{0}:MtnScripts runat=server ></{0}:MtnScripts>")]
    public class AjaxScripts : WebControl
    {
        #region properties
       
        /// <summary>
        /// <para>If empty will generate all classes with attribute AjaxAttribute. Use to generate only classes that need (comma separated)</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Modules { get; set; }

        /// <summary>
        /// <para>If empty will generate all the methods with attribute AjaxMethod. Use to generate only the methods you need (separated by commas)</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Methods { get; set; }


        /// <summary>
        /// <para>If true will minify the code. Default is false.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(true)]
        [Localizable(true)]
        public Boolean Minify { get; set; }

        /// <summary>
        /// <para>If true will minify the code. Default is false.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(false)]
        [Localizable(true)]
        public Boolean WriteOnBody { get; set; }

        /// <summary>
        /// <para>Script processor, default is jQuery.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(ScriptProcessorType.JQuery)]
        [Localizable(true)]
        public ScriptProcessorType ScriptProcessorType { get; set; }


        /// <summary>
        /// <para>If true will minify the code. Default is false.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(false)]
        [Localizable(true)]
        public Boolean IsDeferScript { get; set; }

        /// <summary>
        /// <para>If true will minify the code. Default is false.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(false)]
        [Localizable(true)]
        public Boolean IsAsync { get; set; }

        /// <summary>
        /// <para>Use traditional parameters, like function(parm1,parm2,parm3) instead function(options), calling myFunc({data:'data'});.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(false)]
        [Localizable(true)]
        public Boolean UseTraditionalParameterForm { get; set; }

        /// <summary>
        /// <para>If true will save a file on VirtualPath.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(false)]
        [Localizable(true)]
        public Boolean ExportFile { get; set; }

        /// <summary>
        /// <para>Hash tag for script, can be any world, be careful to avoid reuse the same name for diferent scripts.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(null)]
        [Localizable(true)]
        public String VirtualPath { get; set; }

        /// <summary>
        /// <para>Hash tag for script, can be any world, be careful to avoid reuse the same name for diferent scripts.</para>
        /// </summary>
        [Bindable(false)]
        [Category("Configuration")]
        [DefaultValue(null)]
        [Localizable(true)]
        public String HashTag { get; set; }
        #endregion

        #region Render
        /// <summary>
        /// <para>Render the code.</para>
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                writer.BeginRender();
                Mtn.Library.Web.Utils.Parameter.AjaxWebModeType = WebModeType.AjaxHandler;
                
                var script = ScriptProcessorEngine.GetScriptCode(routeName: "", classList: Modules, methodList: Methods, writeOnBody: WriteOnBody, writeMinified: Minify,
                                                        scriptProcessorType: ScriptProcessorType, webModeType: WebModeType.AjaxHandler, isDeferScript: IsDeferScript, isAsync: IsAsync,
                                                        hashTag: HashTag, exportFile: ExportFile, virtualPath: VirtualPath, useTraditionalParameterForm: UseTraditionalParameterForm);
                writer.Write(script);
                writer.EndRender();
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }
            
        }

        
        #endregion
        

    }
}