using System;
using System.IO;
using System.Web;
using System.IO.Compression;

namespace Mtn.Library.Web.Extensions
{
    /// <summary>
    /// Extensions to WebForms 
    /// </summary>
    public static class WebExtensions
    {
        #region Compression
        
        /// <summary>
        /// <para>Indicates whether compression is possible in the HttpContext.</para>
        /// </summary>
        /// <param name="context">
        /// <para>HttpContext to check.</para>
        /// </param>
        /// <returns>
        /// <para>true if allow.</para>
        /// </returns>
        public static bool AllowCompressionMtn(this HttpContext context)
        {
            return AllowCompressionMtn(context, Mtn.Library.Enums.CompressionType.GZip) ||
                AllowCompressionMtn(context, Mtn.Library.Enums.CompressionType.Deflate);
        }
        
        /// <summary>
        /// <para>Indicates whether compression is possible in the HttpContext.</para>
        /// </summary>
        /// <param name="context">
        /// <para>HttpContext to check.</para>
        /// </param>
        /// <param name="compType">
        /// <para>Compression type.</para>
        /// </param>
        /// <returns>
        /// <para>true if allow.</para>
        /// </returns>
        public static bool AllowCompressionMtn(this HttpContext context, Mtn.Library.Enums.CompressionType compType)
        {
            string isOkToCompress = context.Request.Headers["Accept-Encoding"];
            Stream io = context.Response.Filter;

            if (isOkToCompress == null)
                return false;

            if (isOkToCompress.ToLower().Contains(compType.ToString().ToLower()) == false)
                return false;

            if (io == null)
                io = context.Response.OutputStream;

            context.Response.Filter = new CompressionStream(io, compType);

            context.Response.AppendHeader("Content-Encoding", compType.ToString().ToLower());
            return true;
        }

        /// <summary>
        /// <para>Represents the stream to compression.</para>
        /// </summary>
        public class CompressionStream : System.IO.Stream
        {
            #region Fields
            private readonly System.IO.Stream _mIo;
            #endregion Fields

            #region Constructor
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="baseStream">
            /// <para>Represents the stream to compression.</para>
            /// </param>
            /// <param name="compType">
            /// <para>Compression type.</para>
            /// </param>
            public CompressionStream(System.IO.Stream baseStream, Mtn.Library.Enums.CompressionType compType)
            {
                if (compType == Mtn.Library.Enums.CompressionType.GZip)
                    this._mIo = new GZipStream(baseStream, CompressionMode.Compress);
                else
                    this._mIo = new DeflateStream(baseStream, CompressionMode.Compress);
            }
            #endregion Constructor

            #region Implemented
            /// <summary>
            /// <para>Close stream.</para>
            /// </summary>
            public override void Close()
            {
                this._mIo.Close();
            }
            /// <summary>
            /// <para>Indicates if supports reading.</para>
            /// </summary>
            public override bool CanRead
            {
                get { return this._mIo.CanRead; }
            }
            /// <summary>
            /// <para>Indicates if supports seeking.</para>
            /// </summary>
            public override bool CanSeek
            {
                get { return this._mIo.CanSeek; }
            }
            /// <summary>
            /// <para>Indicates if supports writing.</para>
            /// </summary>
            public override bool CanWrite
            {
                get { return this._mIo.CanWrite; }
            }
            /// <summary>
            /// <para>Clear all buffers to stream.</para>
            /// </summary>
            public override void Flush()
            {
                this._mIo.Flush();
            }
            /// <summary>
            /// <para>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written..</para>
            /// </summary>
            /// <param name="buffer">
            /// <para>An array of bytes. This method copies count bytes from buffer to the current stream.</para>
            /// </param>
            /// <param name="offset">
            /// <para>The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</para>
            /// </param>
            /// <param name="count">
            /// <para>The number of bytes to be written to the current stream.</para>
            /// </param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this._mIo.Write(buffer, offset, count);
            }
            #endregion

            #region default abstract not NotImplementedException
            /// <summary>
            /// <para>Not implemented.</para>
            /// </summary>
            public override long Length
            {
                get { throw new NotImplementedException(); }
            }
            /// <summary>
            /// <para>Not implemented.</para>
            /// </summary>
            public override long Position
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
            /// <summary>
            /// <para>Not implemented.</para>
            /// </summary>
            /// <param name="buffer">
            /// <para>Not implemented.</para>
            /// </param>
            /// <param name="count">
            /// <para>Not implemented.</para>
            /// </param>
            /// <param name="offset">
            /// <para>Not implemented.</para>
            /// </param>
            /// <returns>
            /// <para>Not implemented.</para>
            /// </returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// <para>Not implemented.</para>
            /// </summary>
            /// <param name="offset">
            /// <para>Not implemented.</para>
            /// </param>
            /// <param name="origin">
            /// <para>Not implemented.</para>
            /// </param>
            /// <returns>
            /// <para>Not implemented.</para>
            /// </returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// <para>Not implemented.</para>
            /// </summary>
            /// <param name="value">
            /// <para>Not implemented.</para>
            /// </param>
            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
        #endregion
    }
}
