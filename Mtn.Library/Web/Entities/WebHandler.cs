namespace Mtn.Library.Web.Entities
{
    /// <summary>
    /// <para>Representig a web handler, used by Ajax Handler.</para>
    /// </summary>
    public class WebHandler
    {
        /// <summary>
        /// <para>Assembly Name.</para>
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// <para>Handler name.</para>
        /// </summary>
        public string HandlerName { get; set; }
        /// <summary>
        /// <para>Handler Type.</para>
        /// </summary>
        public string HandlerType { get; set; }
    }
}
