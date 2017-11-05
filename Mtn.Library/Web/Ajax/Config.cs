using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mtn.Library.Service;
using Mtn.Library.Web.Attributes;
using Mtn.Library.Web.Entities;
using Mtn.Library.Extensions;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Scripts;
using Mtn.Library.Web.Utils;
using Parameter = Mtn.Library.Web.Utils.Parameter;

namespace Mtn.Library.Web.Ajax
{
    /// <summary>
    /// 
    /// </summary>
    public static class Config
    {
        #region Attributes

        private static IList<String> _routes = new List<string>();
        #endregion

        #region RouteCollection
        /// <summary>
        /// 
        /// </summary>
        public static IList<String> RouteCollection
        {
            get { return _routes; }
            set { _routes = value; }
        }

        #endregion

        #region AddAjaxClassItem

        /// <summary>
        /// Add a AjaxClassItem on collection
        /// </summary>
        /// <param name="asm">Assembly where classes have AjaxClassAtribute and AjaxMethod's</param>
        /// <param name="ajaxClassItemCollectionGlobal"></param>
        public static void AddAjaxClassItem(Assembly asm, ref Dictionary<String, AjaxClassItem> ajaxClassItemCollectionGlobal)
        {
            //const bool cacheAjaxOnStartup = false; // Web.Utils.Parameter.CacheAjaxOnStartup - initial context is always null;
            //WebModeType modeType;
            //if (asm.FullName.ToLower().Contains("mtn.library"))
            //    return;

            var ajaxClassItemCollection = ajaxClassItemCollectionGlobal;

            // ToDo : Verificar dlls que nao tem customattribute
            List<Type> listClasses;
            try
            {
                listClasses = asm.GetTypes().Where(tp => tp.GetCustomAttributes(typeof(AjaxClassAttribute), true).Any()).ToList();
            }
            catch (Exception ex)
            {
                if (!Parameter.EnableStatistics) return;
                var msg = "Source:" + ex.Source + "\n\nMessage:" + ex.Message + "\n\nStack:" + ex.StackTrace;
                Statistics.Add(msg);
                return;
            }

            var sync = new Object();

            foreach(var ajaxReflectionClass in listClasses)
            {
                lock (sync)
                {
                    var ajaxClass = new AjaxClassItem();
                    var ajaxAttribute = (AjaxClassAttribute)
                        ajaxReflectionClass.GetCustomAttributes(typeof(AjaxClassAttribute), true).
                            FirstOrDefault();

                    if (ajaxAttribute != null)
                    {

                        if (ajaxAttribute.Name.IsNullOrWhiteSpaceMtn())
                            ajaxAttribute.Name = ajaxReflectionClass.FullName;

                        var name = ajaxAttribute.Name;


                        // ajaxAttribute.ClassName.IsNullOrWhiteSpaceMtn() ? ajaxReflectionClass.FullName : ajaxAttribute.ClassName;

                        while (name.Contains(".."))
                            name = name.Replace("..", ".");

                        var key = ajaxAttribute.HashTag.IsNullOrWhiteSpaceMtn() ? name.ToLowerInvariant().Md5Mtn(true) : ajaxAttribute.HashTag.HtmlRemoveMtn().Replace("&", "").Replace("?", "").Replace(".", "").Replace("=", "");
                        key = key.IsNullOrWhiteSpaceMtn() ? name.ToLowerInvariant().Md5Mtn(true) : key;

                        ajaxClass.ClassName = name;
                        ajaxClass.AjaxMethodItemCollection = new Dictionary<string, AjaxMethodItem>();
                        ajaxClass.Key = key;
                        ajaxClass.AjaxClass = ajaxAttribute;

                        var methodList =
                            ajaxReflectionClass.GetMethods().Where(
                                ajx => ajx.GetCustomAttributes(typeof(AjaxMethod), true).Length > 0);


                        //ScriptProcessorEngine scriptProcessor = null;
                        //AjaxScriptItem ajaxScriptItem = null;
                        //if (cacheAjaxOnStartup)
                        //{
                        //    scriptProcessor = ScriptProcessorEngine.CreateInstance(ajaxClass, modeType);

                        //    ajaxScriptItem = new AjaxScriptItem();
                        //    ajaxScriptItem.UseTraditionalParameterForm = ajaxClass.AjaxClass.UseTraditionalParameterForm;
                        //    ajaxScriptItem.AjaxClassKey = ajaxClass.Key;
                        //    ajaxScriptItem.Key = ajaxClass.Key;
                        //    ajaxScriptItem.ScriptProcessor = ajaxClass.AjaxClass.ScriptProcessor;
                        //    ajaxScriptItem.ProcessorPrefix = ajaxClass.AjaxClass.ProcessorPrefix;
                        //    ajaxScriptItem.ScriptTemplate = ajaxClass.AjaxClass.ScriptTemplate;
                        //    if(!ajaxScriptItem.FullScript.IsNullOrWhiteSpaceMtn())
                        //        ajaxScriptItem.FullScriptMinified = Utils.JavaScriptMinifier.MinifyCode(ajaxScriptItem.FullScript);
                        //}

                        foreach(var method in methodList)
                         {
                             var newAjaxMethodItem = new AjaxMethodItem();
                             newAjaxMethodItem.Method = method;
                             if (method.ReflectedType != null)
                                 newAjaxMethodItem.ConstructorHash = method.ReflectedType.FullName.Md5Mtn(true);
                             newAjaxMethodItem.AjaxMethod = method.GetCustomAttributes(typeof(AjaxMethod), true)
                                 .Select(i => (i as AjaxMethod)).FirstOrDefault();

                             if (newAjaxMethodItem.AjaxMethod != null)
                             {

                                 newAjaxMethodItem.Parameters = (from mt in newAjaxMethodItem.Method.GetParameters()
                                                                 where mt.IsRetval == false
                                                                 orderby mt.Position
                                                                 select
                                                                     new Entities.Parameter(mt.Name, "",
                                                                                                            mt.Position,
                                                                                                            mt.ParameterType.Name,
                                                                                                            mt.ParameterType)
                                                                ).ToList();
                                 if (newAjaxMethodItem.AjaxMethod.Name.IsNullOrWhiteSpaceMtn())
                                     newAjaxMethodItem.AjaxMethod.Name = method.Name;

                                 var nameOfMethod = newAjaxMethodItem.AjaxMethod.Name;

                                 var keyMeth = newAjaxMethodItem.AjaxMethod.HashTag.IsNullOrWhiteSpaceMtn() ? nameOfMethod.ToLowerInvariant().Md5Mtn(true) : newAjaxMethodItem.AjaxMethod.HashTag.HtmlRemoveMtn().Replace("&", "").Replace("?", "").Replace(".", "").Replace("=", "");
                                 keyMeth = keyMeth.IsNullOrWhiteSpaceMtn() ? nameOfMethod.ToLowerInvariant().Md5Mtn(true) : keyMeth;

                                 newAjaxMethodItem.Key = keyMeth;
                                 if (ajaxClass.AjaxMethodItemCollection.Any(x => x.Key == newAjaxMethodItem.Key))
                                     ajaxClass.AjaxMethodItemCollection.Remove(newAjaxMethodItem.Key);

                                 ajaxClass.AjaxMethodItemCollection.Add(newAjaxMethodItem.Key, newAjaxMethodItem);

                                 //if (cacheAjaxOnStartup)
                                 //{
                                 //    var ajaxScriptMethodItem = new AjaxScriptMethodItem();
                                 //    ajaxScriptMethodItem.Script = scriptProcessor.CreateMethodCode(newAjaxMethodItem, ajaxClass.AjaxClass.UseTraditionalParameterForm);
                                 //    ajaxScriptMethodItem.ScriptMinified =
                                 //        Utils.JavaScriptMinifier.MinifyCode(ajaxScriptMethodItem.Script);
                                 //    ajaxScriptItem.AjaxScriptMethodCollection.Add(newAjaxMethodItem.Key, ajaxScriptMethodItem);
                                 //}

                             }
                             if (ajaxClassItemCollection.Any(x => x.Key.Equals(ajaxClass.Key)) != false) continue;
                             //if (cacheAjaxOnStartup)
                             //{
                             //    ajaxScriptItem.FullScript = scriptProcessor.CreateFullCode(ajaxClass.AjaxClass.UseTraditionalParameterForm);
                             //    var _ajaxScriptCollection = AjaxScriptCollection;
                             //    _ajaxScriptCollection.Add(ajaxScriptItem.Key, ajaxScriptItem);
                             //    AjaxScriptCollection = _ajaxScriptCollection;
                             //}
                             try
                             {
                                 if (ajaxClassItemCollection.Any(x => x.Key.Equals(ajaxClass.Key)) == false)
                                     ajaxClassItemCollection.Add(ajaxClass.Key, ajaxClass);
                             }
                             catch (Exception ex)
                             {

                                 Statistics.Add(ex.GetAllMessagesMtn());
                             }
                         };
                    }
                }
            };
            ajaxClassItemCollectionGlobal = ajaxClassItemCollection;
        }
        #endregion

        #region LoadAjaxCollection

        /// <summary>
        /// Load the ajax list
        /// </summary>
        public static Dictionary<String, AjaxClassItem> LoadAjaxCollection()
        {

            var assemblies =
                        AppDomain.CurrentDomain.GetAssemblies().Where(
                            i => i.ManifestModule.ScopeName != "RefEmit_InMemoryManifestModule");
            var list = new Dictionary<string, AjaxClassItem>();
            Parallel.ForEach(assemblies.Distinct(), asm =>
            {
                try
                {
                    var modulo =
                                asm.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).FirstOrDefault() as
                                AssemblyCompanyAttribute;

                    if (modulo == null ||
                        (Parameter.GetCompanyToIgnore.Any(company => modulo.Company.ToLower().Contains(company)))) return;

                    if (Parameter.GetAssemblies.Any())
                    {
                        if (
                            Parameter.GetAssemblies.Any(
                                assembly => asm.GetName().Name.ToLower().Contains(assembly)))
                        {
                            AddAjaxClassItem(asm, ref list);
                        }

                    }
                    else
                    {
                        AddAjaxClassItem(asm, ref list);
                    }
                }
                catch (Exception ex)
                {
                    Statistics.Add(ex.GetAllMessagesMtn());
                }
            });

            return list;
        }
        #endregion

        #region AjaxClassItemCollection

        /// <summary>
        /// Return a SortedDictionary of AjaxClassItem
        /// </summary>
        public static Dictionary<String, AjaxClassItem> AjaxClassItemCollection
        {
            get
            {
                if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxClassItemCollection"))
                {
                    var obj = Cache.Instance[MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxClassItemCollection"];
                    if (obj != null)
                        return (Dictionary<String, AjaxClassItem>)obj;
                    
                    var list = LoadAjaxCollection();
                    try
                    {
                        Cache.Instance.Remove(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxClassItemCollection");
                        Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxClassItemCollection");
                    }
                    catch (Exception ex)
                    {
                        Statistics.Add(ex.GetAllMessagesMtn());
                    }
                    return list;
                }
                else
                {
                    var list = LoadAjaxCollection();
                    try
                    {
                        Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxClassItemCollection");
                    }
                    catch (Exception ex)
                    {
                        Statistics.Add(ex.GetAllMessagesMtn());
                    }
                    return list;
                }
            }
            internal set
            {
                try
                {
                    if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxClassItemCollection"))
                    {
                        var obj = Cache.Instance[MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxClassItemCollection"];
                        if (obj != null)
                        {
                            var list = value;
                            Cache.Instance.Remove(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxClassItemCollection");
                            Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxClassItemCollection");
                        }
                        else
                        {
                            var list = value;
                            Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxClassItemCollection");
                        }
                    }
                    else
                    {
                        var list = value;
                        Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxClassItemCollection");
                    }
                }
                catch (Exception ex)
                {
                    Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
        }
        #endregion

        #region AjaxScriptCollection

        /// <summary>
        /// Return a SortedDictionary of AjaxScriptItem
        /// </summary>
        public static Dictionary<String, AjaxScriptItem> AjaxScriptCollection
        {
            get
            {

                if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxScriptCollection"))
                {
                    var obj = Cache.Instance[MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxScriptCollection"];
                    if (obj != null)
                        return (Dictionary<String, AjaxScriptItem>)obj;
                }

                var ajaxScriptCollection = new Dictionary<string, AjaxScriptItem>();

                var modeType = Parameter.AjaxWebModeType; 

                foreach (var ajaxClass in AjaxClassItemCollection.Select(x => x.Value))
                {
                    if (ajaxClass == null)
                        continue;
                    var scriptProcessor = ScriptProcessorEngine.CreateInstance(ajaxClass, modeType);
                    var scriptFull = scriptProcessor.CreateFullCode(ajaxClass.AjaxClass.UseTraditionalParameterForm);
                    var scriptClass = scriptProcessor.CreateClassCode().ToString();
                    var ajaxScriptItem = new AjaxScriptItem
                                         {
                                             AjaxClassKey = ajaxClass.Key,
                                             Key = ajaxClass.Key,
                                             ScriptProcessor = ajaxClass.AjaxClass.ScriptProcessor,
                                             ProcessorPrefix = ajaxClass.AjaxClass.ProcessorPrefix,
                                             Script = (Parameter.DisableAjaxDebug ? "" : scriptClass),
                                             ScriptMinified = Minifier.MinifyCode(scriptClass),
                                             ScriptTemplate = ajaxClass.AjaxClass.ScriptTemplate,
                                             FullScript = (Parameter.DisableAjaxDebug ? "" : scriptFull),
                                             FullScriptMinified = Minifier.MinifyCode(scriptFull),
                                             UseTraditionalParameterForm = ajaxClass.AjaxClass.UseTraditionalParameterForm
                                         };
                    foreach(var ajaxMethodItem in ajaxClass.AjaxMethodItemCollection.Select(x => x.Value))
                    {
                        var ajaxScriptMethodItem = new AjaxScriptMethodItem();
                        var scriptMethodFull = scriptProcessor.CreateMethodCode(ajaxMethodItem, ajaxClass.AjaxClass.UseTraditionalParameterForm);
                        ajaxScriptMethodItem.Script = (Parameter.DisableAjaxDebug ? "" : scriptMethodFull);
                        ajaxScriptMethodItem.Key = ajaxMethodItem.Key;
                        ajaxScriptMethodItem.ScriptMinified = Minifier.MinifyCode(scriptMethodFull);

                        ajaxScriptItem.AjaxScriptMethodCollection.Add(ajaxMethodItem.Key, ajaxScriptMethodItem);
                    }

                    try
                    {
                        ajaxScriptCollection.Add(ajaxScriptItem.Key, ajaxScriptItem);
                    }
                    catch (Exception ex)
                    {
                        Statistics.Add(ex.GetAllMessagesMtn());
                    }
                }

                try
                {
                    Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, ajaxScriptCollection, "Mtn.library._ajaxScriptCollection");
                }
                catch (Exception ex)
                {
                    Statistics.Add(ex.GetAllMessagesMtn());
                }

                return ajaxScriptCollection;
            }
            internal set
            {
                try
                {
                    if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxScriptCollection"))
                    {
                        var obj = Cache.Instance[MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxScriptCollection"];
                        if (obj != null)
                        {
                            var list = value;
                            Cache.Instance.Remove(MtnAjaxCacheType.AjaxClass, "Mtn.library._ajaxScriptCollection");
                            Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxScriptCollection");
                        }
                        else
                        {
                            var list = value;
                            Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxScriptCollection");
                        }
                    }
                    else
                    {
                        var list = value;
                        Cache.Instance.Add(MtnAjaxCacheType.AjaxClass, list, "Mtn.library._ajaxScriptCollection");
                    }
                }
                catch (Exception ex)
                {
                    Statistics.Add(ex.GetAllMessagesMtn());
                }
            }
        }
        #endregion

    }
}
