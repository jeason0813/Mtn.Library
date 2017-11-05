using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Mtn.Library.Entities;
using Mtn.Library.Extensions;
using ImageFormat = Mtn.Library.Enums.ImageFormat;

namespace Mtn.Library.Image
{
    /// <summary>
    /// Util class to work with images
    /// </summary>
    public class Utils
    {
        #region CreateResultListFromStream

        /// <summary>
        /// Create a ImageResult from Stream
        /// </summary>
        /// <param name="listModel"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static List<ImageResult> CreateResultListFromStream(List<Entities.ImageTaskModel> listModel, Stream stream)
        {
            var resultList = listModel.Select(model => CreateFromStream(model, stream)).ToList();
            return resultList;
        }

        #endregion

        #region CreateFromStream
        /// <summary>
        /// Create a ImageResult from Stream
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ImageResult CreateFromStream(Entities.ImageTaskModel model, Stream stream)
        {
            var result = new ImageResult(stream);
            var width = result.Image.Width;
            var height = result.Image.Height;
            System.Drawing.Image newImage = null;
            if (model.CropImage)
            {
                if (model.ScaleFirst && model.ScaleImage)
                {
                    newImage = ScaleImage(result.Image, model.MaxWidth, model.MaxHeight);
                }

                newImage = CropImage(newImage ?? result.Image, model.CropArea);
            }
            else if (model.ScaleImage && (!model.ScaleFirst || !model.CropImage))
            {
                newImage = ScaleImage(result.Image, model.MaxWidth, model.MaxHeight);
            }
            else
            {
                newImage = result.Image;
            }
            var format = model.ImageFormat == ImageFormat.None
                ? result.ImageFormat.GetImageFormatFromEnumMtn()
                : model.ImageFormat.GetImageFormatFromEnumMtn();
            var streamNew = new System.IO.MemoryStream();

            if (format.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) && model.Quality > 0)
            {
                var encoder = format.GetEncoderFromFormatMtn();
                
                // Create an Encoder object based on the GUID 
                // for the Quality parameter category.
                var myEncoder = Encoder.Quality;

                // Create an EncoderParameters object. 
                // An EncoderParameters object has an array of EncoderParameter 
                // objects. In this case, there is only one 
                // EncoderParameter object in the array.
                var encParms = new EncoderParameters(1);

                var encParm = new EncoderParameter(myEncoder, model.Quality);
                encParms.Param[0] = encParm;
                newImage.Save(streamNew, encoder, encParms);
                result = new ImageResult(streamNew);
            }
            else
            {
                newImage.Save(streamNew, format);
                result = new ImageResult(streamNew);
            }
            if (model.DoNotSave) return result;

            var path = Core.Parameter.GetRootPath(model.Folder);
            var filePath = Path.Combine((model.OverrideCurrentPath ? model.Folder : path), model.FileName);
            result.Image.Save(filePath);
            result.FilePath = filePath;
            return result;
        }
        #endregion

        #region Scale

        /// <summary>
        /// Scale Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        #endregion

        #region Crop
        /// <summary>
        /// Crop image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="destRectangle"></param>
        /// <returns></returns>
        public static System.Drawing.Image CropImage(System.Drawing.Image image, Rectangle destRectangle)
        {

            var newImage = new Bitmap(destRectangle.Width, destRectangle.Height);

            using (var graphics = Graphics.FromImage(image))
                graphics.DrawImage(newImage, destRectangle.X, destRectangle.Y, destRectangle.Width, destRectangle.Height);

            return newImage;
        }
        #endregion

    }
}