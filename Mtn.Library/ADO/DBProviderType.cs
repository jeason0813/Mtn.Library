namespace Mtn.Library.ADO
{
    /// <summary>
    /// Provider type 
    /// </summary>
    public enum DbProviderType
    {
        /// <summary>
        /// System.Int64. A 64-bit signed integer.
        /// </summary>
        Int64 = 1,
        /// <summary>
        /// System.Array of type System.Byte. BLOB, IMAGE,ETC
        /// </summary>
        Binary = 2,
        /// <summary>
        /// System.Boolean. An unsigned numeric value that can be 0, 1, or null.
        /// </summary>
        Boolean = 3,
        /// <summary>
        /// System.String. A fixed-length stream of non-Unicode characters ranging between 1 and 8,000 characters.
        /// </summary>
        String = 4,
        /// <summary>
        /// System.DateTime. Date and time data ranging in value from January 1, 1753 to December 31, 9999 to an accuracy of 3.33 milliseconds.
        /// </summary>
        DateTime = 5,
        /// <summary>
        /// System.Decimal. A fixed precision and scale numeric value between -10 38  -1 and 10 38 -1.
        /// </summary>
        Decimal = 6,
        /// <summary>
        /// System.Double. A floating point number within the range of -1.79E +308 through  1.79E +308.
        /// </summary>
        Float = 7,
        /// <summary>
        /// System.Int32. A 32-bit signed integer.
        /// </summary>
        Int = 8,
        /// <summary>
        /// System.Single. A floating point number within the range of -3.40E +38 through   3.40E +38.
        /// </summary>
        Single = 9,
        /// <summary>
        ///  System.Guid. A globally unique identifier (or GUID).
        /// </summary>
        UniqueIdentifier = 10,
        /// <summary>
        ///     An XML value. Obtain the XML as a string using the System.Data.SqlClient.SqlDataReader.GetValue(System.Int32)
        ///     method or System.Data.SqlTypes.SqlXml.Value property, or as an System.Xml.XmlReader
        ///     by calling the System.Data.SqlTypes.SqlXml.CreateReader() method.
        /// </summary>
        Xml = 11,
        /// <summary>
        /// Text, Clob
        /// </summary>
        Text = 12,
        /// <summary>
        ///     Smallint
        /// </summary>
        Int16 = 13,

        /// <summary>
        ///     Date
        /// </summary>
        Date = 14,

        /// <summary>
        ///     Time
        /// </summary>
        Time = 15,

        /// <summary>
        ///     Money, real
        /// </summary>
        Money = 16,

        /// <summary>
        ///     Char
        /// </summary>
        Char = 17,

        /// <summary>
        ///     Tinyint
        /// </summary>
        Byte = 18
    }
}