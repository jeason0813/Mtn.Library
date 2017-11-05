using System;
using System.Reflection;
using Mtn.Library.Extensions;
using Mtn.Library.Service;

namespace Mtn.Library.Web.Utils
{
    /// <summary>
    /// <para>Minify the script code.</para>
    /// </summary>
    public class Minifier
    {
        /// <summary>
        /// 
        /// </summary>
        public enum MinifyType
        {
            /// <summary>
            /// 
            /// </summary>
            Javascript,
            /// <summary>
            /// 
            /// </summary>
            Css
        }

        /// <summary>
        /// 
        /// </summary>
        public enum MinifyConverterType
        {
            /// <summary>
            /// 
            /// </summary>
            Internal,
            /// <summary>
            /// 
            /// </summary>
            AjaxMin,
            /// <summary>
            /// 
            /// </summary>
            AjaxMinCrunch 
        }

        private static dynamic _minify;
        private static dynamic _codeSettings;
        private static MinifyConverterType _minifyConverType = MinifyConverterType.AjaxMin;


        /// <summary>
        /// <para>Minify the script code.</para>
        /// </summary>
        /// <param name="source">
        /// <para>script code.</para>
        /// </param>
        /// <param name="type"></param>
        /// <returns>
        /// <para>Minified string.</para>
        /// </returns>
        public static string MinifyCode(String source, MinifyType type = MinifyType.Javascript)
        {
            try
            {
                if (_minify == null)
                {
                    Assembly assembly;
                    try
                    {
                        assembly = Assembly.Load("AjaxMin");
                    }
                    catch (Exception exp)
                    {
                        assembly = null;
                    }

                    if (assembly != null)
                    {
                        var typeClass = assembly.GetType("Microsoft.Ajax.Utilities.Minifier");
                        if (typeClass != null)
                        {
                            _minify = assembly.CreateInstance("Microsoft.Ajax.Utilities.Minifier");
                            _codeSettings = assembly.CreateInstance("Microsoft.Ajax.Utilities.CodeSettings");
                            _minifyConverType = MinifyConverterType.AjaxMin;
                        }
                        else // old assembly
                        {
                            _minify = assembly.CreateInstance("Microsoft.Ajax.Utilities.ScriptCruncher");
                            _minifyConverType = MinifyConverterType.AjaxMinCrunch;
                        }
                    }
                    else
                    {
                        _minify = (int) 0;
                        _minifyConverType = MinifyConverterType.Internal;
                    }
                }

                switch (type)
                {
                    case MinifyType.Javascript:
                        switch (_minifyConverType)
                        {
                            case MinifyConverterType.AjaxMinCrunch:
                                return _minify.Crunch(source) + ";"; // resolve error on minifier
                            case MinifyConverterType.AjaxMin:
                                _codeSettings.TermSemicolons = true;
                                return _minify.MinifyJavaScript(source, _codeSettings);
                            case MinifyConverterType.Internal:
                                return Mtn.Library.Web.Utils.JsMinifier.GetMinifiedCode(source);
                        }
                        break;
                    case MinifyType.Css:
                        switch (_minifyConverType)
                        {
                            case MinifyConverterType.AjaxMinCrunch:
                                return _minify.Crunch(source);
                            case MinifyConverterType.AjaxMin:
                                return _minify.MinifyStyleSheet(source);
                            case MinifyConverterType.Internal:
                                return Mtn.Library.Web.Utils.CssMinifier.CssMinify(source);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            }
            catch (Exception exp)
            {
                Statistics.Add(exp.GetAllMessagesMtn());
                return source;
            }
            return source;
        }
    }
}