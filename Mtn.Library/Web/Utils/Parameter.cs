using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mtn.Library.Configuration;
using Mtn.Library.Enums;
using Mtn.Library.Extensions;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;

namespace Mtn.Library.Web.Utils
{

    /// <summary>    
    /// <para>Class Mtn.Library to get parameters for the features of the library.</para>
    /// </summary>
    public static class Parameter
    {
        #region attributes

        private static List<string> _lstAssembliesToInclude;
        private static List<string> _lstAssembliesToIgnore;
        private static List<WebHandler> _lstAssembliesWebHandlers;
        private static bool? _useCompression = false;
        private static bool? _overrideMinifyOption;
        private static bool? _overrideUseJsServerOption;
        private static bool? _enableAjaxScriptClearCache;
        private static bool? _writePathOnBasePage;
        private static bool? _writeStatisticsOnBasePage;
        private static bool? _disableMtnScriptForClasses;
        private static bool? _disableAjaxDebug;
        private static int? _maxScriptKeyCacheForHash;
        private static List<string> _permittedCacheKeyList;
        private static string _unauthorizedPage;
        private static string _unallowedPage;
        private static bool? _enableStatistics;
        private static bool? _enableHeaderStatistics;
        private static bool? _enableHeaderAllow;
        private static WebModeType? _ajaxWebModeType;
        private static string _versionPassword;
        #endregion

        private static string _seoCharacter;

        /// <summary>
        /// Name of action, MVC only
        /// </summary>
        public static String SeoCharacter
        {
            get
            {
                if (!_seoCharacter.IsNullOrWhiteSpaceMtn()) return _seoCharacter;

                _seoCharacter = Config.GetString("Mtn.Library.Web.SeoCharacter");
                if (_seoCharacter.IsNullOrWhiteSpaceMtn())
                    _seoCharacter = "-";
                return _seoCharacter;
            }
            set { _seoCharacter = value; }
        }
        private static bool? _mUseAjaxExtension;
        private static bool? _enableMultiPortal;

        /// <summary>
        /// Get or Set if use extension on web forms, by default if "Mtn.Library.Web.AjaxExtension" exists and is not empty will be true
        /// </summary>
        public static Boolean UseAjaxExtension
        {
            get
            {
                if (_mUseAjaxExtension.HasValue) return _mUseAjaxExtension.Value;
                var retVal = Config.GetString("Mtn.Library.Web.AjaxExtension");
                _mUseAjaxExtension = !retVal.IsNullOrWhiteSpaceMtn();
                return _mUseAjaxExtension.Value;
            }
            set { _mUseAjaxExtension = value; }
        }
        #region overrideUseJsServerOption

        /// <summary>    
        /// <para>Indicates whether all the modules should be used as an external file (overrides the parameter of UseJsInServer AjaxScript).</para>
        /// </summary>
        public static Boolean? OverrideUseJsServerOption
        {
            get
            {
                _overrideUseJsServerOption =
                    Config.GetNullableBoolean("Mtn.Library.Web.overrideUseJsServerOption");
                return _overrideUseJsServerOption;
            }
        }

        #endregion

        #region OverrideMinifyOption

        /// <summary>    
        /// <para>Indicates whether all methods should be created minified (overrides the parameter of Minify AjaxScript).</para>
        /// </summary>
        public static Boolean? OverrideMinifyOption
        {
            get
            {
                _overrideMinifyOption = Config.GetNullableBoolean("Mtn.Library.Web.OverrideMinifyOption");
                return _overrideMinifyOption;
            }
        }

        #endregion

        #region MaxMinutesScriptCacheOption

        /// <summary>    
        /// <para>How many minutes ajax scripts should be cached, default is 1 minute (only valid when the CacheTimeType is different from Forever).</para>
        /// </summary>
        public static Single MaxMinutesScriptCacheOption
        {
            get
            {
                const float defaultValue = 1;
                var retVal = Config.GetNulllableSingle("Mtn.Library.Web.MaxMinutesScriptCacheOption");
                return retVal.HasValue ? retVal.Value : defaultValue;
            }
        }

        #endregion

        #region CacheTimeType

        /// <summary>    
        /// <para>Indicates the type of cache for the ajax scripts, the default is UpdateByAccess.</para>
        /// </summary>
        public static CacheTimeType CacheTimeType
        {
            get
            {
                string retVal = Config.GetString("Mtn.Library.Web.CacheTimeType");

                switch (retVal.ToLower())
                {
                    case "absolute":
                        return CacheTimeType.Absolute;
                    case "forever":
                        return CacheTimeType.Forever;
                    case "updatebyaccess":
                        return CacheTimeType.UpdateByAccess;
                    default:
                        return CacheTimeType.UpdateByAccess;
                }
            }
        }

        #endregion

        #region UseCompression

        /// <summary>    
        /// <para>Indicates whether to use web compression, the default is false.</para>
        /// </summary>
        public static Boolean UseCompression
        {
            get
            {
                if (_useCompression.HasValue == false)
                    _useCompression = Config.GetNullableBoolean("Mtn.Library.Web.UseCompression");

                return _useCompression != null && _useCompression.Value;
            }
        }

        #endregion

        #region AjaxExtension

        /// <summary>    
        /// <para>Extension for Ajax, the default is ashx. Very useful when there is already a handler of ajax methods using the ashx extension.</para>
        /// </summary>
        public static String AjaxExtension
        {
            get
            {
                var retVal = Config.GetString("Mtn.Library.Web.AjaxExtension");
                return retVal.IsNullOrWhiteSpaceMtn() ? "ashx" : retVal;
            }
        }

        #endregion

        #region DefaultScriptName

        /// <summary>    
        /// <para>Default name for the scripts stored on the server and not on the page. Default is MtnDefaultAjaxScriptBuffer.</para>
        /// </summary>   
        public static String DefaultScriptName
        {
            get
            {
                var retVal = Config.GetString("Mtn.Library.Web.DefaultScriptName");
                return retVal.IsNullOrWhiteSpaceMtn() ? "MtnDefaultAjaxScriptBuffer" : retVal;
            }
        }

        #endregion

        #region GetAssemblies

        /// <summary>    
        /// <para>Name of the assemblies to be checked to see if has Mtn Ajax code in there, by default all will be checked. This could accelerate the start of the web server.</para>
        /// </summary>  
        public static IList<string> GetAssemblies
        {
            get
            {
                try
                {
                    if (_lstAssembliesToInclude != null) return _lstAssembliesToInclude;
                    _lstAssembliesToInclude = new List<string>();
                    var retVal = Config.GetString("Mtn.Library.Web.Assemblies");
                    if (!retVal.IsNullOrWhiteSpaceMtn())
                        _lstAssembliesToInclude = retVal.ToLower().Split(',').ToList();
                    if (!_lstAssembliesToInclude.Exists(x => x.Equals("mtn.library")))
                        _lstAssembliesToInclude.Add("mtn.library");
                    return _lstAssembliesToInclude;
                }
                catch
                {
                    _lstAssembliesToInclude = new List<string>();
                    return _lstAssembliesToInclude;
                }
            }
        }

        #endregion

        #region GetCompanyToIgnore

        /// <summary>    
        /// <para>Names of companies in the assemblies that are ignored, by default only microsoft is ignored (for all the assemblies required for web application are from Microsoft's and they do not contain Mtn code ;-) ). This could accelerate the start of the web server and avoid conflicts.</para>
        /// </summary>  
        public static IList<string> GetCompanyToIgnore
        {
            get
            {
                try
                {
                    if (_lstAssembliesToIgnore != null) return _lstAssembliesToIgnore;

                    _lstAssembliesToIgnore = new List<string>();
                    var retVal = Config.GetString("Mtn.Library.Web.CompanyToIgnore");

                    _lstAssembliesToIgnore = !retVal.IsNullOrWhiteSpaceMtn() ? retVal.ToLowerInvariant().Split(',').ToList() : new List<string> { "microsoft" };
                    return _lstAssembliesToIgnore;
                }
                catch
                {
                    _lstAssembliesToIgnore = new List<string> { "microsoft" };
                    // only to ignore assemblies referenced by default, like system.dll
                    return _lstAssembliesToIgnore;
                }
            }
        }

        #endregion

        #region GetWebHandlers

        /// <summary>    
        /// <para>Other ajax handlers using the same extension (ashx for example), so if you can not find the method in Mtn, these handlers are called. Attention Mtn does not control the outcome of these, so it is possible to have error and not be mapped by Mtn.</para>
        /// </summary>
        public static IList<WebHandler> GetWebHandlers
        {
            get
            {
                try
                {
                    if (_lstAssembliesWebHandlers != null) return _lstAssembliesWebHandlers;

                    _lstAssembliesWebHandlers = new List<WebHandler>();
                    var retVal = Config.GetString("Mtn.Library.Web.WebHandlers");
                    foreach (var strVal in retVal.Split(',').ToList())
                    {
                        var hand = new WebHandler();
                        var handArray = strVal.Split(':');

                        hand.AssemblyName = handArray[0];
                        hand.HandlerName = handArray[1];
                        hand.HandlerType = handArray[2];
                        _lstAssembliesWebHandlers.Add(hand);
                    }
                    return _lstAssembliesWebHandlers;
                }
                catch
                {
                    _lstAssembliesWebHandlers = new List<WebHandler>();
                    return _lstAssembliesWebHandlers;
                }
            }
        }

        #endregion

        #region EnableAjaxScriptClearCache

        /// <summary>    
        /// <para>Enable the clearcache functionality for Ajax Scripts. Default is false. For security reason.</para>
        /// </summary>
        public static Boolean EnableAjaxScriptClearCache
        {
            get
            {
                if (_enableAjaxScriptClearCache.HasValue == false)
                {
                    _enableAjaxScriptClearCache =
                        Config.GetNullableBoolean("Mtn.Library.Web.EnableAjaxScriptClearCache");
                }
                return _enableAjaxScriptClearCache != null && _enableAjaxScriptClearCache.Value;

            }
            set { _enableAjaxScriptClearCache = value; }
        }

        #endregion

        #region MaxScriptKeyCacheForHash

        /// <summary>    
        /// <para>Maximum number of cacheKey for hash (for each module / method is created a hash). For safety the default is 200 (for small applications is enough). Zero (0) means no limit.</para>
        /// </summary>
        public static Int32? MaxScriptKeyCacheForHash
        {
            get
            {
                if (_maxScriptKeyCacheForHash.HasValue) return _maxScriptKeyCacheForHash.Value;
                _maxScriptKeyCacheForHash =
                    Config.GetNullableInt32("Mtn.Library.Web.MaxScriptKeyCacheForHash");
                _maxScriptKeyCacheForHash = _maxScriptKeyCacheForHash.HasValue == false ? 200 : Math.Abs(_maxScriptKeyCacheForHash.Value);
                return _maxScriptKeyCacheForHash.Value;

            }
            set { _maxScriptKeyCacheForHash = value; }
        }

        #endregion

        #region PermittedCacheKeyList

        /// <summary>    
        /// <para>To increase security you can limit the cacheKey's only to be used from the list, if it is empty (Count == 0) this feature is ignored allowing any cacheKey (default).</para>
        /// </summary>
        public static IList<String> PermittedCacheKeyList
        {
            get
            {
                if (_permittedCacheKeyList != null) return _permittedCacheKeyList;

                var retVal = Config.GetString("Mtn.Library.Web.PermittedCacheKeyList");
                if (retVal.IsNullOrWhiteSpaceMtn())
                {
                    _permittedCacheKeyList = new List<string>();
                }
                else
                {
                    _permittedCacheKeyList = new List<string>();
                    var retValList = retVal.ToLowerInvariant().SplitMtn(",");
                    _permittedCacheKeyList.AddRange(retValList);
                }
                return _permittedCacheKeyList;

            }
            set { _permittedCacheKeyList = value.ToList(); }
        }

        #endregion

        #region Page Info

        /// <summary>    
        /// <para>Page to redirect in unauthorized cases (not logged in).</para>
        /// </summary>
        public static String UnauthorizedPage
        {
            get
            {
                if (_unauthorizedPage.IsNullOrWhiteSpaceMtn())
                    _unauthorizedPage = Config.GetString("Mtn.Library.Web.UnauthorizedPage");

                return _unauthorizedPage;

            }
            set { _unauthorizedPage = value; }
        }

        /// <summary>    
        /// <para>Page to redirect in unallowed cases (don't have permission) .</para>
        /// </summary>
        public static String UnallowedPage
        {
            get
            {
                if (_unallowedPage.IsNullOrWhiteSpaceMtn())
                {
                    _unallowedPage = Config.GetString("Mtn.Library.Web.UnallowedPage");
                }
                return _unallowedPage;

            }
            set { _unallowedPage = value; }
        }

        /// <summary>    
        /// <para>Enable page inherited from Mtn.Library.Web.Page.BasePage to write URL on page.</para>
        /// </summary>
        public static Boolean WritePathOnBasePage
        {
            get
            {
                if (_writePathOnBasePage.HasValue) return _writePathOnBasePage.Value;
                _writePathOnBasePage =
                    Config.GetNullableBoolean("Mtn.Library.Web.WritePathOnBasePage");
                if (_writePathOnBasePage.HasValue == false)
                    _writePathOnBasePage = true;
                return _writePathOnBasePage.Value;

            }
            set { _writePathOnBasePage = value; }
        }

        /// <summary>    
        /// <para>Enable page inherited from Mtn.Library.Web.Page.BasePage to write Statistics on page.</para>
        /// </summary>
        public static bool WriteStatisticsOnBasePage
        {
            get
            {
                if (_writeStatisticsOnBasePage.HasValue == false)
                {
                    _writeStatisticsOnBasePage =
                        Config.GetNullableBoolean("Mtn.Library.Web.WriteStatisticsOnBasePage");
                    if (_writeStatisticsOnBasePage.HasValue == false)
                        _writeStatisticsOnBasePage = true;
                }
                return _writeStatisticsOnBasePage.Value;

            }
            set { _writeStatisticsOnBasePage = value; }
        }

        #endregion

        #region AjaxWebModeType

        // <summary>
        // Indicates if will cache scripts on startup
        // </summary>
        //public static Boolean CacheAjaxOnStartup
        //{
        //    get
        //    {
        //        var res = Configuration.Config.GetNullableBoolean("Mtn.Library.Web.CacheAjaxOnStartup");
        //        return res.HasValue?res.Value:false;
        //    }
        //}

        /// <summary>
        /// Indicates what type mode will cache scripts on startup (only if CacheAjaxOnStartup is true)
        /// </summary>
        public static WebModeType AjaxWebModeType
        {
            get
            {
                if (!_ajaxWebModeType.HasValue)
                {
                    var mode = Config.GetNullableEnum<WebModeType>("Mtn.Library.Web.CacheAjaxType");
                    _ajaxWebModeType = mode.HasValue ? mode.Value : WebModeType.MvcController;
                }


                return _ajaxWebModeType.Value;
            }
            set { _ajaxWebModeType = value; }
        }

        #endregion

        #region Ajax Debug



        /// <summary>
        /// Disable the full code, its good for production, to avoid memory overhead
        /// </summary>
        public static Boolean DisableAjaxDebug
        {
            get
            {
                if (_disableAjaxDebug.HasValue == false)
                {
                    _disableAjaxDebug = Config.GetNullableBoolean("Mtn.Library.Web.DisableAjaxDebug");
                    if (_disableAjaxDebug.HasValue == false)
                        _disableAjaxDebug = false;
                }
                return _disableAjaxDebug.Value;
            }
            set { _disableAjaxDebug = value; }
        }

        #endregion

        #region DisableMtnScriptForClasses
        /// <summary>
        /// Disable the script code, like Mtn.ns for any class
        /// </summary>
        public static Boolean DisableMtnScriptForClasses
        {
            get
            {
                if (_disableMtnScriptForClasses.HasValue == false)
                {
                    _disableMtnScriptForClasses = Config.GetNullableBoolean("Mtn.Library.Web.DisableMtnScriptForClasses");
                    if (_disableMtnScriptForClasses.HasValue == false)
                        _disableMtnScriptForClasses = false;
                }
                return _disableMtnScriptForClasses.Value;
            }
            set { _disableMtnScriptForClasses = value; }
        }
        #endregion

        #region EnableMultiPortal
        /// <summary>
        /// Enable check for multiportal, if is always the same script and dont have a problem to go for another url, 
        /// set this to false, will use less memory
        /// </summary>
        public static Boolean EnableMultiPortal
        {
            get
            {
                if (_enableMultiPortal.HasValue == false)
                {
                    _enableMultiPortal = Config.GetNullableBoolean("Mtn.Library.Web.EnableMultiPortal");
                    if (_enableMultiPortal.HasValue == false)
                        _enableMultiPortal = true;
                }
                return _enableMultiPortal.Value;
            }
            set { _enableMultiPortal = value; }
        }
        #endregion

        #region EnableStatistics
        /// <summary>
        /// Disable the script code, like Mtn.ns for any class
        /// </summary>
        public static Boolean EnableStatistics
        {
            get
            {
                if (_enableStatistics.HasValue == false)
                {
                    _enableStatistics = Config.GetNullableBoolean("Mtn.Library.Web.EnableStatistics");
                    if (_enableStatistics.HasValue == false)
                        _enableStatistics = false;
                }
                return _enableStatistics.Value;
            }
            set { _enableStatistics = value; }
        }
        #endregion

        #region EnableStatistics
        /// <summary>
        /// Disable the script code, like Mtn.ns for any class
        /// </summary>
        public static Boolean EnableHeaderStatistics
        {
            get
            {
                if (_enableHeaderStatistics.HasValue == false)
                {
                    _enableHeaderStatistics = Config.GetNullableBoolean("Mtn.Library.Web.EnableStatistics");
                    if (_enableHeaderStatistics.HasValue == false)
                        _enableHeaderStatistics = true;
                }
                return _enableHeaderStatistics.Value;
            }
            set { _enableHeaderStatistics = value; }
        }
        #endregion

        #region EnableHeaderAccessControlAllowOrigin
        /// <summary>
        /// 
        /// </summary>
        public static Boolean EnableHeaderAccessControlAllowOrigin
        {
            get
            {
                if (_enableHeaderAllow.HasValue == false)
                {
                    _enableHeaderAllow = Config.GetNullableBoolean("Mtn.Library.Web.EnableHeaderAccessControlAllowOrigin");
                    if (_enableHeaderAllow.HasValue == false)
                        _enableHeaderAllow = true;
                }
                return _enableHeaderAllow.Value;
            }
            set { _enableHeaderAllow = value; }
        }



        /// <summary>    
        /// <para>Extension for Ajax, the default is ashx. Very useful when there is already a handler of ajax methods using the ashx extension.</para>
        /// </summary>
        public static IList<String> AccessControlDomains
        {
            get
            {
                var result = new List<string>();
                var retVal = Config.GetString("Mtn.Library.Web.AccessControlAllowOriginDomains");
                if (retVal.IsNullOrWhiteSpaceMtn())
                    result.Add("*");
                else
                    result.AddRange(retVal.Split(',').ToList());
                return result;
            }
        }



        #endregion

        #region Version
        /// <summary>
        /// Return the current version of Mtn.Library
        /// </summary>
        public static Version Version
        {
            get
            {
                var a = Assembly.GetExecutingAssembly();
                return a.GetName().Version;
            }
        }
        /// <summary>
        /// Get or Set the password to consulting version
        /// </summary>
        public static String VersionPassword
        {
            get
            {
                if (!_versionPassword.IsNullOrWhiteSpaceMtn()) return _versionPassword;
                _versionPassword = Config.GetString("Mtn.Library.VersionPassword");
                if (_versionPassword.IsNullOrWhiteSpaceMtn())
                    _versionPassword = "mtn123";
                return _versionPassword;
            }
            set { _versionPassword = value; }
        }

        #endregion

    }
}