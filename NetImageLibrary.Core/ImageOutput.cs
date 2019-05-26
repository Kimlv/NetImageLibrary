﻿#region License and copyright notice
/*
 * Kaliko Image Library
 * 
 * Copyright (c) 2014 Fredrik Schultz and Contributors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */
#endregion

namespace Kaliko.ImageLibrary {
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
#if NET46
    using System.Web;
#endif

    internal class ImageOutput {
        private static ImageCodecInfo GetEncoderInfo(String mimeType) {
            var encoders = ImageCodecInfo.GetImageEncoders();

            foreach (var codecInfo in encoders) {
                if (codecInfo.MimeType == mimeType) {
                    return codecInfo;
                }
            }

            return null;
        }

#if NET46
    // httpresponse doesn't exist in netStandard, at least not in the same way. Can't
    // get the outputstream, even if you use the extensions & abstractons packages. 

        internal static Stream PrepareImageStream(string fileName, string mime) {
            HttpResponse stream = HttpContext.Current.Response;
            stream.Clear();
            stream.ClearContent();
            stream.ClearHeaders();
            stream.ContentType = mime;
            stream.AddHeader("Content-Disposition", "inline;filename=" + fileName);
            return stream.OutputStream;
        }
#endif

        internal static void SaveStream(KalikoImage image, Stream stream, long quality, string imageFormat, bool saveResolution) {
            var encoderParameters = GetEncoderParameters(quality);
            var encoderInfo = GetEncoderInfo(imageFormat);

            if (saveResolution) {
                using (var copy = new Bitmap(image.Image)) {
                    copy.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    copy.Save(stream, encoderInfo, encoderParameters);
                }
            }
            else {
                image.Image.Save(stream, encoderInfo, encoderParameters);
            }
        }

        private static EncoderParameters GetEncoderParameters(long quality) {
            var encparam = new EncoderParameters(1);
            encparam.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            return encparam;
        }

        internal static void SaveFile(KalikoImage image, string fileName, long quality, string imageFormat, bool saveResolution) {
            var encoderParameters = GetEncoderParameters(quality);
            var encoderInfo = GetEncoderInfo(imageFormat);

            if (saveResolution) {
                using (var copy = new Bitmap(image.Image)) {
                    copy.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    copy.Save(fileName, encoderInfo, encoderParameters);
                }
            }
            else {
                image.Image.Save(fileName, encoderInfo, encoderParameters);
            }
        }

        internal static void SaveFile(KalikoImage image, string fileName, ImageFormat imageFormat, bool saveResolution) {
            if (saveResolution) {
                using (var copy = new Bitmap(image.Image)) {
                    copy.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    copy.Save(fileName, imageFormat);
                }
            }
            else {
                image.Image.Save(fileName, imageFormat);
            }
        }

        internal static void SaveStream(KalikoImage image, Stream stream, ImageFormat imageFormat, bool saveResolution) {
            if (saveResolution) {
                using (var copy = new Bitmap(image.Image)) {
                    copy.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    copy.Save(stream, imageFormat);
                }
            }
            else {
                image.Image.Save(stream, imageFormat);
            }
        }
    }
}
