using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Mtn.Library.Attributes;
using Mtn.Library.Extensions;
using Mtn.Library.Service;
using Mtn.Library.Web.Entities;
using Mtn.Library.Web.Enums;
using Mtn.Library.Web.Extensions;
using Mtn.Library.Web.Scripts;

namespace Mtn.Library.Web.Ajax
{
    /// <summary>
    /// 
    /// </summary>
    public class AjaxEngine
    {
        #region Contants
        const string MtnMethodHash = "__MtnMethodHash";
        const string MtnCallBackName = "__MtnCallBackName";
        const string MtnClassHash = "__MtnClassHash";
        const string MtnRandomGet = "_";
        #endregion

        #region ProcessRequest [Main]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webModeType"> </param>
        /// <returns></returns>
        public static Boolean ProcessRequest(HttpContext context, WebModeType webModeType)
        {
            bool result = true;
            var classKey = context.Request[MtnClassHash];
            var methodKey = context.Request[MtnMethodHash];
            var initTime = Init(context);
            if (classKey.IsNullOrWhiteSpaceMtn() == false && methodKey.IsNullOrWhiteSpaceMtn() == false)
            {
                if (Mtn.Library.Web.Utils.Parameter.EnableHeaderAccessControlAllowOrigin)
                {
                    foreach (string domain in Mtn.Library.Web.Utils.Parameter.AccessControlDomains)
                        context.Response.AddHeader("Access-Control-Allow-Origin", domain);
                }


                if (Render(context, classKey, methodKey) == false && webModeType == WebModeType.AjaxHandler)
                {
                    processHandlers(context);
                }
            }
            else
            {
                AddHeaderMtn(context, "Error", "Method hash or class hash not found");
            }
            End(context, initTime);

            return result;
        }
        #endregion

        #region Utils
        #region Process ajax handlers
        private static void processHandlers(HttpContext context)
        {
            var strResponse = new StringBuilder("");
            foreach (var webHand in Utils.Parameter.GetWebHandlers)
            {
                try
                {
                    AddHeaderMtn(context, "Call_AnotherWebHandler_" + webHand.HandlerName,
                                 "called at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                    var hand =
                        Assembly.Load(webHand.AssemblyName).GetTypes().FirstOrDefault(
                            h => h.Name.ToLower() == webHand.HandlerName.ToLower());
                    var types = new Type[] { };
                    if (hand != null)
                    {
                        var resHand = hand.GetConstructor(types);
                        if (webHand.HandlerType.Equals("ihttphandlerfactory"))
                        {
                            if (resHand != null)
                            {
                                var httpHandlerFactory = resHand.Invoke(new object[] { }) as IHttpHandlerFactory;
                                if (httpHandlerFactory != null)
                                    httpHandlerFactory.GetHandler(context,
                                                                  context.Request.
                                                                      RequestType,
                                                                  context.Request.RawUrl,
                                                                  context.Request.
                                                                      PhysicalApplicationPath)
                                        .ProcessRequest(context);
                            }
                        }
                        else if (resHand != null)
                        {
                            var httpHandler = resHand.Invoke(new object[] { }) as IHttpHandler;
                            if (httpHandler != null)
                                httpHandler.ProcessRequest(context);
                        }
                    }
                    AddHeaderMtn(context, "AnotherWebHandler_" + webHand.HandlerName,
                                 "Returns at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                }
                catch (Exception ex)
                {
                    context.ClearError();
                    strResponse.Append("Mtn Error:" + ex.GetAllMessagesMtn() + "\n");
                    AddHeaderMtn(context, "ERROR_DATE_" + webHand.HandlerName,
                                 "Returns error at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                    AddHeaderMtn(context, "ERROR_DESC_" + webHand.HandlerName, "Error: " + ex.GetAllMessagesMtn());
                }
            }

            if (Utils.Parameter.GetWebHandlers.Count == 0)
            {

                context.ClearError();
                context.Response.ContentType = "text/plain";
                strResponse.Append("Mtn Error: Not found this method!\n");

                for (int i = 0; i < strResponse.Length; i++)
                    context.Response.Write(strResponse[i]);
            }
        }
        #endregion

        #region ReturnElapsedTime
        /// <summary>
        /// <para>Return the time in milliseconds since time informed in init.</para>
        /// </summary>
        /// <param name="init">
        /// <para>Time informed.</para>
        /// </param>
        /// <returns>
        /// <para>Return the time in milliseconds since time informed in init.</para>
        /// </returns>
        private static double ReturnElapsedTime(double init)
        {
            return Math.Round((1000 * ((Stopwatch.GetTimestamp() - init) / Stopwatch.Frequency)), 6);
        }
        #endregion

        #region AddHeaderMtn
        private static void AddHeaderMtn(HttpContext context, string headerName, string value)
        {
            context.Response.AddHeader("Mtn.LD." + headerName, value);
        }
        #endregion

        #region Parser
        /// <summary>
        /// <para>Return the object parsed using the correct type</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object Parser(Type type, object value)
        {
            string typeName = type.Name;
            switch (typeName)
            {
                case "Byte":
                    return byte.Parse(value.ToString());
                case "Int16":
                    return Int16.Parse(value.ToString());
                case "Int32":
                    return Int32.Parse(value.ToString());
                case "Int64":
                    return Int64.Parse(value.ToString());
                case "Decimal":
                    return Decimal.Parse(value.ToString());
                case "Single":
                    return Single.Parse(value.ToString());
                case "Double":
                    return Double.Parse(value.ToString());
                case "Boolean":
                    return Boolean.Parse(value.ToString());
                case "Char":
                    return Char.Parse(value.ToString());
                case "DateTime":
                    return DateTime.Parse(value.ToString());
                case "String":
                    return (String)value.ToString();
                case "HttpPostedFile":
                    return value;
                case "List<HttpPostedFile>":
                    return value;
                case "IList<HttpPostedFile>":
                    return value;
                default:
                    if (type.FullName.Contains("HttpPostedFile"))
                    {
                        if (type.FullName.Contains("List"))
                        {
                            if (value.GetType().FullName.Contains("List") == false)
                            {
                                var lst = new List<HttpPostedFile>();
                                lst.Add((HttpPostedFile)value);
                                return lst;
                            }
                        }
                        return value;
                    }
                    break;
            }
            try
            {
                var newEntity = value.ToString().ToObjectFromJsonMtn(type);
                return newEntity;
            }
            catch
            {
                return value;
            }
        }
        #endregion

        #region Execute
        private static object ExecuteAjaxMethod(MethodInfo method, object[] parameters, string constructorObjHash)
        {

            #region Invoke

            var attrs = method.GetCustomAttributes(typeof(MtnBaseAttribute), true);
            foreach (var attr in attrs.OrderBy(a => (a as MtnBaseAttribute).Hierarchy))
            {
                if (attr is Authorization)
                {
                    if ((attr as Authorization).IsAuthorized() == false)
                        return Authorization.UnauthorizedValue;
                    else
                        continue;
                }
                if (attr is Permission)
                {
                    var attrP = (attr as Permission);
                    if (attrP.HasPermission(attrP.Value1, attrP.Value2, attrP.Value3, attrP.Value4) == false)
                        return Permission.UnallowedResult;
                    else
                        continue;

                }

                if (attr is CacheResult)
                {
                    string key = CacheResult.GetKeyPrefix(method) + CacheResult.GetKey(parameters);
                    var result = CacheResult.GetCache((Enum)(attr as CacheResult).ContainerType, key);
                    if (result != null)
                        return result;
                }

            }
            object resultObj = null;

            if (method.IsStatic)
            {
                resultObj = method.Invoke(null, parameters);
                return resultObj;
            }
            else
            {
                object constructorObj = null;

                if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxContructor, constructorObjHash))
                    constructorObj = Cache.Instance[MtnAjaxCacheType.AjaxContructor, constructorObjHash];
                else
                {
                    var ctr = method.ReflectedType.GetConstructor(Type.EmptyTypes);
                    if (ctr != null)
                        constructorObj = ctr.Invoke(new object[] { });
                    Cache.Instance.Add(MtnAjaxCacheType.AjaxContructor, constructorObj, constructorObjHash);
                }

                if (constructorObj != null)
                {
                    resultObj = method.Invoke(constructorObj, parameters);

                    return resultObj;
                }
            }

            #endregion

            return null;
        }

        #endregion

        #region Serialize

        /// <summary>
        /// <para>Serialize the ajax method result.</para>
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="objectToSerialize">
        /// <para>Object to be serialized.</para>
        /// </param>
        /// <param name="responseType">
        /// <para>Response type.</para>
        /// </param>
        /// <param name="requestType">
        /// <para>Request type.</para>
        /// </param>
        /// <param name="ignoreNullData">
        /// <para>Indicates if must serialize null data.</para>
        /// </param>
        /// <param name="indented">
        /// <para>Indicates if must ident data (JSON).</para>
        /// </param>
        /// <param name="callbackName">
        /// <para>Name of callback function (JSONP only).</para>
        /// </param>
        /// <returns>
        /// <para>Returns the serialized string.</para>
        /// </returns>
        private static String Serialize(HttpContext context, object objectToSerialize, ResponseType responseType, RequestType requestType, bool ignoreNullData, bool indented, string callbackName)
        {
            string retValue = "";

            switch (responseType)
            {
                case ResponseType.Json:
                    retValue = objectToSerialize.ToJsonMtn(ignoreNullData: ignoreNullData, indented: indented);
                    context.Response.ContentType = "application/json";
                    break;
                case ResponseType.JsonP:
                    retValue = callbackName + "(";
                    var valueSerializeJsonp = objectToSerialize.ToJsonMtn(ignoreNullData: ignoreNullData, indented: indented);
                    retValue += valueSerializeJsonp.IsNullOrEmptyMtn() ? "" : valueSerializeJsonp + ");";
                    context.Response.ContentType = "application/jsonp";
                    //if (requestType != RequestType.Form)
                    //    retValue = callbackName + "(" + retValue + ")";

                    break;
                case ResponseType.Text:
                    retValue = (string)Convert.ChangeType(objectToSerialize, typeof(string));
                    context.Response.ContentType = "text/html";
                    break;
                case ResponseType.Xml:
                    var memoryStream = new MemoryStream();
                    var xml = new XmlSerializer(objectToSerialize.GetType());
                    var ns = new XmlSerializerNamespaces();

                    // Add prefix-namespace pairs.
                    ns.Add("Mtn", "http://www.metanoianet.com");

                    var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                    xml.Serialize(xmlTextWriter, objectToSerialize, ns);
                    var enc = new UTF8Encoding();
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    retValue = enc.GetString(memoryStream.ToArray());
                    context.Response.ContentType = "application/xml";
                    break;
            }
            if (context.Request.Headers["X-Requested-With"] == null)
                context.Response.ContentType = "text/html";

            return retValue;
        }
        #endregion
        #endregion

        #region Init

        private static double Init(HttpContext context)
        {
            double mtnInitStamp = System.Diagnostics.Stopwatch.GetTimestamp();
            AddHeaderMtn(context, "00_Start", "Starts at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
            if (Utils.Parameter.UseCompression)
                context.AllowCompressionMtn();


            return mtnInitStamp;
        }


        #endregion

        #region Render

        private static bool Render(HttpContext context, String classKey, String methodKey)
        {
            AddHeaderMtn(context, "Cache", "01_Starts at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
            var ajaxClass = Config.AjaxClassItemCollection[classKey];
            if (ajaxClass != null)
            {
                var ajaxMethodItem = ajaxClass.AjaxMethodItemCollection[methodKey];
                if (ajaxMethodItem != null)
                {
                    AddHeaderMtn(context, "02_Cache", "Ends at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                    var parametersWeb = new List<Parameter>();
                    var callBackName = GetWebParameters(context, ajaxMethodItem.AjaxMethod.RequestType, ref parametersWeb, System.Attribute.GetCustomAttributes(ajaxMethodItem.Method));
                    AddHeaderMtn(context, "03_ParmsFound", "Parameters found at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                    var parametersParsed = ParseWebParameters(ajaxMethodItem.Parameters, parametersWeb);
                    AddHeaderMtn(context, "04_ParmsParsed", "Parameters parsed at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                    try
                    {
                        var result = ExecuteAjaxMethod(ajaxMethodItem.Method, parametersParsed, ajaxMethodItem.ConstructorHash);
                        AddHeaderMtn(context, "05_Method_Invoked", "Method invoked at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                        string returnData = Serialize(context, result, ajaxMethodItem.AjaxMethod.ResponseType, ajaxMethodItem.AjaxMethod.RequestType, ajaxMethodItem.AjaxMethod.IgnoreNullData, ajaxMethodItem.AjaxMethod.Indented, callBackName);

                        context.Response.Write(returnData);
                        AddHeaderMtn(context, "06_Return_Data", "Returns serialized data at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                    }
                    catch (Exception ex)
                    {
                        #region catch

                        string error = ex.GetAllMessagesMtn();
                        context.ClearError();
                        //strResponse.Append("Mtn Error:" + exp.Message + "\n");
                        AddHeaderMtn(context, "07_ERROR_DATE", "Returns error at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                        AddHeaderMtn(context, "08_ERROR_DESC", "Mtn Error: " + error);

                        Exception expInner = ex.InnerException;
                        int nCountExp = 1;
                        int nCountErroInfo = 9;
                        while (expInner != null)
                        {
                            //strResponse.Append("Mtn Error:" + expInner.Message + "\n");
                            AddHeaderMtn(context, (nCountErroInfo++).ToString("00") + "_ERROR_DATE" + nCountExp.ToString(CultureInfo.InvariantCulture), "Returns error at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                            AddHeaderMtn(context, (nCountErroInfo++).ToString("00") + "_ERROR_DESC" + nCountExp.ToString(CultureInfo.InvariantCulture), "Mtn Error: " + expInner.Message);
                            expInner = expInner.InnerException;
                            nCountExp++;
                        }


                        #endregion

                        return false;
                    }
                    return true;
                }
                AddHeaderMtn(context, "02_Not_Found", "Not found any method at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
                return false;
            }
            return false;
        }

        #region ParseWebParameters
        private static object[] ParseWebParameters(IList<Parameter> methodParameters, IList<Parameter> parametersWeb)
        {
            var parametersParsed = (from p in methodParameters
                                    join pW in parametersWeb
                                                         on p.Name.ToLower() equals pW.Name.ToLower()
                                    orderby p.Position
                                    select Parser(p.Type, pW.Value)
             ).ToArray<object>();

            return parametersParsed;
        }
        #endregion

        #region GetWebParameters
        private static String GetWebParameters(HttpContext context, RequestType requestType, ref List<Parameter> parametersWeb, System.Attribute[] attrs)
        {
            parametersWeb = new List<Entities.Parameter>();
            string callBackName = context.Request["__MtnJsonpCallback"];

            string lastFile = "\x001";
            for (var i = 0; i < context.Request.Files.Count; i++)
            {
                var obj = context.Request.Files[i];
                if (context.Request.Files.AllKeys[i].Contains(lastFile))
                    continue;

                if (context.Request.Files.AllKeys[i].Contains("["))
                {
                    lastFile = context.Request.Files.AllKeys[i].SplitMtn("[")[0];
                    var listFile = context.Request.Files.AllKeys.Where(x => x.StartsWith(lastFile + "["));
                    var listFileParm = listFile.Select(fileName => context.Request.Files[fileName]).ToList();
                    var parm = new Entities.Parameter(lastFile, listFileParm, i,
                                                                 "List<HttpPostedFile>", typeof(List<HttpPostedFile>));
                    parametersWeb.Add(parm);
                }
                else
                {
                    var parm = new Entities.Parameter(context.Request.Files.AllKeys[i], obj, i,
                                                                     "HttpPostedFile", typeof(HttpPostedFile));
                    parametersWeb.Add(parm);
                }

            }

            if (requestType == RequestType.Post)
            {
                for (int i = 0; i < context.Request.Form.Count; i++)
                {
                    string formKey = context.Request.Form.AllKeys[i];
                    if (formKey.Equals(MtnMethodHash) || formKey.Equals(MtnCallBackName) ||
                        formKey.Equals(MtnClassHash) || formKey.Equals(MtnRandomGet))
                    {
                        continue;
                    }

                    string data = "";
                    try
                    {
                        data = context.Request.Form[formKey];
                    }
                    catch (Exception exp)
                    {
                        var needRethrow = true;
                        var attribute = attrs.FirstOrDefault(x => x.GetType().Name.Contains("ValidateInput"));

                        if (attribute != null)
                        {
                            var prop = attribute.GetType()
                                    .GetProperties()
                                    .FirstOrDefault(x => x.Name == "EnableValidation");
                            if (prop != null)
                            {
                                if (prop.GetValue(attribute, null).ToString().ToBooleanMtn() == false)
                                {
                                    dynamic ctxD = context;
                                    data = ctxD.Request.Unvalidated.Form[formKey];
                                    needRethrow = false;
                                }
                            }

                        }


                        if (needRethrow)
                            throw exp;
                    }

                    var parm = new Entities.Parameter(formKey, data, i, "string", typeof(string));
                    parametersWeb.Add(parm);
                }
            }
            if (requestType == RequestType.Get)
            {
                for (int i = 0; i < context.Request.QueryString.Count; i++)
                {
                    string queryKey = context.Request.QueryString.AllKeys[i];
                    if (queryKey.Equals(MtnMethodHash) || queryKey.Equals(MtnCallBackName) ||
                        queryKey.Equals(MtnClassHash) || queryKey.Equals(MtnRandomGet))
                    {
                        continue;
                    }
                    string data = context.Request.QueryString[queryKey];
                    var parm = new Entities.Parameter(queryKey, data, i, "string", typeof(string));
                    parametersWeb.Add(parm);
                }
            }

            if (context.Request.ContentType.Contains("json"))
            {
                var sr = new System.IO.StreamReader(context.Request.InputStream);
                string jsonRequest = sr.ReadToEnd();

                var objParm = (System.Collections.Generic.Dictionary<string, object>)jsonRequest.ToObjectFromJsonMtn();
                int idx = 0;
                foreach (var objJson in objParm.ToList())
                {
                    var parm = new Entities.Parameter(objJson.Key, objJson.Value.ToJsonMtn(), idx,
                                                                     "string", typeof(string));
                    parametersWeb.Add(parm);
                    idx++;
                }
            }

            return callBackName;
        }
        #endregion

        #endregion

        #region End

        private static void End(HttpContext context, double mtnInitStamp)
        {
            AddHeaderMtn(context, "99_End", "End process at " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffff"));
            AddHeaderMtn(context, "99_End_Total_Time", "End process using " + ReturnElapsedTime(mtnInitStamp) + " miliseconds");

            try
            {
                // todo: this is the only way to avoid 2 requests and errors, need study the better solution for this
                context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }
        }
        #endregion

        #region [Scripts]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContext"></param>
        /// <param name="webModeType"> </param>
        /// <returns></returns>
        public static Boolean WriteScript(String url, HttpContext httpContext, WebModeType webModeType = WebModeType.MvcController)
        {
            string scriptName = ((!Utils.Parameter.UseAjaxExtension && webModeType == WebModeType.MvcController)
                               ? "mtnajaxmethods/"
                               : "mtnajaxmethods." + Utils.Parameter.AjaxExtension);

            if (url.ToLowerInvariant().Contains(scriptName.ToLowerInvariant()))
            {
                string hashtag = httpContext.Request["hashtag"];
                if (hashtag.IsNullOrWhiteSpaceMtn())
                    return false;
                string script;
                if (Cache.Instance.ContainsKey(MtnAjaxCacheType.AjaxScriptMethod, hashtag) == false)
                    script = ScriptProcessorEngine.GetStartScriptCode(Utils.Parameter.DisableMtnScriptForClasses);
                else
                    script = ScriptProcessorEngine.GetStartScriptCode(Utils.Parameter.DisableMtnScriptForClasses) + (string)Cache.Instance[MtnAjaxCacheType.AjaxScriptMethod, hashtag];
                httpContext.Response.Write(script);
                httpContext.Response.ContentType = "text/javascript; charset=UTF-8";
                httpContext.ApplicationInstance.CompleteRequest();

                return true;
            }
            else
            {
                scriptName = httpContext.Request.ApplicationPath.ToLowerInvariant();
                scriptName += ((!Utils.Parameter.UseAjaxExtension && webModeType == WebModeType.MvcController)
                               ? "mtnlibrary/"
                               : "mtnlibrary." + Utils.Parameter.AjaxExtension);
                var minScriptName = ((!Utils.Parameter.UseAjaxExtension && webModeType == WebModeType.MvcController)
                               ? "mtnlibrary-min/"
                               : "mtnlibrary.min." + Utils.Parameter.AjaxExtension);
                if (url.ToLowerInvariant().Contains(scriptName.ToLowerInvariant()) || url.ToLowerInvariant().Contains(minScriptName.ToLowerInvariant()))
                {
                    string script = ScriptProcessorEngine.GetStartScriptCode(false, url.ToLowerInvariant().Contains(minScriptName.ToLowerInvariant()));
                    httpContext.Response.Write(script);
                    httpContext.Response.ContentType = "text/javascript; charset=UTF-8";
                    httpContext.ApplicationInstance.CompleteRequest();
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}
