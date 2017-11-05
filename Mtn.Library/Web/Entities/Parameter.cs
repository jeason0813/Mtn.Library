
using System;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace Mtn.Library.Web.Entities
{
    /// <summary>
    /// <para>This class is used for tests and internal methods, but can be used for another reasons.</para>
    /// </summary>
    [Serializable]
    public class Parameter
    {
        /// <summary>
        /// <para>Name.</para>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// <para>Value.</para>
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// <para>Position.</para>
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// <para>Name of type.</para>
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// <para>Type.</para>
        /// </summary>
        public Type Type { get; set; }

        private DateTime ? _mExactlyNow;
        /// <summary>
        /// <para>Returns DateTime.Now if not setted yet.</para>
        /// </summary>
        public DateTime ExactlyNow
        {
            get { if(_mExactlyNow.HasValue) return _mExactlyNow.Value; else return DateTime.Now; }
            set { _mExactlyNow = value; }
        }
        /// <summary>
        /// <para>List of Parameter.</para>
        /// </summary>
        public List<Parameter> List { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// <para>Name.</para>
        /// </param>
        /// <param name="value">
        /// <para>Value.</para>
        /// </param>
        /// <param name="position">
        /// <para>Position.</para>
        /// </param>
        /// <param name="typeName">
        /// <para>Name of type.</para>
        /// </param>
        public Parameter(string name, object value, int position, string typeName)
            : this(name, value, position, typeName, TypeBuilder.GetType(typeName))
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// <para>Name.</para>
        /// </param>
        /// <param name="value">
        /// <para>Value.</para>
        /// </param>
        /// <param name="position">
        /// <para>Position.</para>
        /// </param>
        /// <param name="typeName">
        /// <para>Name of type.</para>
        /// </param>
        /// <param name="type">
        /// <para>Type.</para>
        /// </param>
        public Parameter(string name, object value, int position, string typeName, Type type)
        {
            this.Name = name;
            this.Position = position;
            this.TypeName = typeName;
            this.Value = value;
            this.Type = type;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// <para>Name.</para>
        /// </param>
        /// <param name="value">
        /// <para>Value.</para>
        /// </param>
        /// <param name="position">
        /// <para>Position.</para>
        /// </param>
        public Parameter(string name, object value, int position)
        {
            this.Name = name;
            this.Position = position;
            this.Value = value;
        }
    }

}
