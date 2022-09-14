using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data.Models;

namespace ddPoliglotV6.BL.Helpers
{
    public static class ImageHelper
    {
        //private static int _width = 1280;
        //private static int _height = 720;

        //private static int _startTextFontSize = 70;
        //private static int _startTrTextFontSize = 30;
        //private static int _startPronTextFontSize = 40;

        private static int _width = 854;
        private static int _height = 480;

        private static int _startTextFontSize = 50;
        private static int _startTrTextFontSize = 25;
        private static int _startPronTextFontSize = 25;

        private static int _paddingLeft = _width / 30; // ~~50
        private static int _trHeight = _height / 7; // ~~100

        private static Color _bgrColor = Color.White;
        private static Color _textColor = Color.Black;
        // private static Color _trTextColor = Color.LightGray;
        private static Color _trTextColor = Color.FromArgb(255, 75, 75, 75);

        private static Color _pronTextColor = Color.Gray;

        private static StringFormat _format = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Near
        };

        internal static Bitmap MakeAndSaveSimpleBitmap(ArticlePhrase articlePhrase, string imgName, int imgType, PrivateFontCollection privateFontCollection)
        {
            if (File.Exists(imgName))
            {
                return null;
            }

            FontFamily fontFamily = null;
            if (privateFontCollection?.Families?[0] != null)
            {
                fontFamily = privateFontCollection.Families[0];
            }

            RectangleF textRect;
            SizeF textSize;

            RectangleF trTextRect;
            SizeF trTextSize;

            RectangleF pronTextRect;
            SizeF pronTextSize;

            TextWithProps text;
            TextWithProps trText;
            TextWithProps textHidden;
            TextWithProps trTextHidden;

            //bool isTextBox = false;

            // SetRectanglesBigTop(out textRect, out trTextRect);

            text = textHidden = RemovePhraseSimbols(articlePhrase.Text);
            trText = trTextHidden = RemovePhraseSimbols(articlePhrase.TrText);


            // speaker
            if (imgType == 0)
            {
                // image for foreign text
                if (articlePhrase.ActivityType == 0)
                {
                    // speaker, image for foreign text, foreign text first
                        SetRectanglesBigTop(out textRect, out trTextRect, out pronTextRect, text);
                    trText.Text = "";
                }
                else
                {
                    // speaker, image for foreign text, native text first
                    SetRectanglesSmallTop(out textRect, out trTextRect, out pronTextRect, text);
                }
            }
            else 
            {
                // image for native (tr) text
                if (articlePhrase.ActivityType == 0)
                {
                    // speaker, image for native (tr) text, foreign text first
                    SetRectanglesBigTop(out textRect, out trTextRect, out pronTextRect, text);
                }
                else
                {
                    // speaker, image for native (tr) text, native text first
                    SetRectanglesSmallTop(out textRect, out trTextRect, out pronTextRect, text);
                    text.Text = ""; //  articlePhrase.Text;
                }
            }

            if (articlePhrase.Type != 0)
            {
                // diktor

                if (text.Text == "")
                {
                    textHidden.Text = text.Text = "!";
                }

                if (trText.Text == "")
                {
                    trTextHidden.Text = trText.Text = "!";
                }
            }

            var bmp = GetEmptyBitmap();
            var graphics = MakeAndTuneGraphics(bmp);

            if (string.IsNullOrEmpty(text.TextProps.Pron))
            {
                // text without prononse text
                Font font = CalcFontToFitToRectangle(graphics, textHidden, textRect, _startTextFontSize, fontFamily, out textSize);
                Font trFont = CalcFontToFitToRectangle(graphics, trTextHidden, trTextRect, _startTrTextFontSize, fontFamily, out trTextSize);

                // calc align textes vertical
                var textesY = (_height / 2) - ((textSize.Height + trTextSize.Height + _paddingLeft) / 2 + (_paddingLeft));

                textRect.Location = new PointF(textRect.Location.X, textesY);

                if (articlePhrase.ActivityType == 0)
                {
                    // plase trText at the bottom of the text
                    trTextRect.Location = new PointF(trTextRect.Location.X, textRect.Location.Y + textSize.Height + _paddingLeft);
                }
                else
                {
                    textRect.Location = new PointF(textRect.Location.X, trTextRect.Location.Y + trTextSize.Height + _paddingLeft);
                }

                graphics.DrawString(text.Text, font, new SolidBrush(_textColor), textRect, _format);
                graphics.DrawString(trText.Text, trFont, new SolidBrush(_trTextColor), trTextRect, _format);
            }
            else
            {
                // text with prononce

                Font font = CalcFontToFitToRectangle(graphics, textHidden, textRect, _startTextFontSize, fontFamily, out textSize);

                var fontPron = fontFamily == null
                    ? new Font("Arial", _startPronTextFontSize, FontStyle.Regular)
                    : new Font(fontFamily, _startPronTextFontSize, FontStyle.Regular);

                pronTextSize = graphics.MeasureString(text.TextProps.Pron, fontPron, width: _width);

                Font trFont = CalcFontToFitToRectangle(graphics, trTextHidden, trTextRect, _startTrTextFontSize, fontFamily, out trTextSize);

                // calc align textes vertical
                var textesY = (_height / 2) - ((textSize.Height + trTextSize.Height + pronTextSize.Height + _paddingLeft + _paddingLeft) / 2 + (_paddingLeft));

                textRect.Location = new PointF(textRect.Location.X, textesY);
                
                if (articlePhrase.ActivityType == 0)
                {
                    // plase trText at the bottom of the text
                    pronTextRect.Location = new PointF(pronTextRect.Location.X, textRect.Location.Y + textSize.Height + _paddingLeft);
                    trTextRect.Location = new PointF(trTextRect.Location.X, pronTextRect.Location.Y + pronTextSize.Height + _paddingLeft);
                }
                else
                {
                    textRect.Location = new PointF(textRect.Location.X, trTextRect.Location.Y + trTextSize.Height + _paddingLeft);
                    pronTextRect.Location = new PointF(pronTextRect.Location.X, textRect.Location.Y + textSize.Height + _paddingLeft);
                }

                graphics.DrawString(text.Text, font, new SolidBrush(_textColor), textRect, _format);
                if (!string.IsNullOrEmpty(text.Text))
                {
                    graphics.DrawString(text.TextProps.Pron, fontPron, new SolidBrush(_pronTextColor), pronTextRect, _format);
                }

                graphics.DrawString(trText.Text, trFont, new SolidBrush(_trTextColor), trTextRect, _format);
            }

            graphics.Flush();
            bmp.Save(imgName, ImageFormat.Png);

            return bmp;
        }

        private static TextWithProps RemovePhraseSimbols(string text)
        {
            var result = new TextWithProps(text);
            result.Text = result.Text.Replace("~", "")
                .Replace("|", "")
                .Replace("+", "")
                //.Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("*", "");

            if (!string.IsNullOrWhiteSpace(result.TextProps?.Pron))
            {
                result.TextProps.Pron = result.TextProps.Pron.Replace("~", "")
                    .Replace("|", "")
                    .Replace("+", "")
                    //.Replace("-", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("*", "");
            }

            return result;
        }

        public static void CombineBitmaps(Bitmap bmp1, Bitmap bmp2, float opacity, string imgName)
        {
            var bmp = bmp1.Clone(new Rectangle(0, 0, _width, _height), PixelFormat.Format24bppRgb);
            var graphics = MakeAndTuneGraphics(bmp);

            ColorMatrix colorMatrix1 = new ColorMatrix();
            ImageAttributes ia1 = new ImageAttributes();
            colorMatrix1.Matrix33 = opacity;
            ia1.SetColorMatrix(colorMatrix1);

            graphics.DrawImage(bmp2, new Rectangle(0, 0, _width, _height), 0, 0, _width, _height, GraphicsUnit.Pixel, ia1);
            graphics.Flush();

            bmp.Save(imgName, ImageFormat.Png);
        }

        private static void SetRectanglesBigTop(out RectangleF rect, out RectangleF trRect, out RectangleF pronRect, TextWithProps text)
        {
            if (string.IsNullOrEmpty(text.TextProps.Pron))
            {

                var bigH = _height - (_trHeight + (_paddingLeft * 3));

                rect = new RectangleF(
                    _paddingLeft,
                    _paddingLeft,
                    _width - (_paddingLeft * 2),
                    bigH
                    );

                trRect = new RectangleF(
                    _paddingLeft,
                    bigH + (_paddingLeft * 2),
                    _width - (_paddingLeft * 2),
                    _trHeight
                    );
                pronRect = new RectangleF(0, 0, 0, 0);
            }
            else
            {
                var pronH = _startPronTextFontSize * 2;
                var bigH = _height - (_trHeight + pronH + (_paddingLeft * 5));

                rect = new RectangleF(
                    _paddingLeft,
                    _paddingLeft,
                    _width - (_paddingLeft * 2),
                    bigH
                    );

                pronRect = new RectangleF(
                    _paddingLeft,
                    bigH + (_paddingLeft * 2),
                    _width - (_paddingLeft * 2),
                    pronH
                    );

                trRect = new RectangleF(
                    _paddingLeft,
                    bigH + pronH + (_paddingLeft * 4),
                    _width - (_paddingLeft * 2),
                    _trHeight
                    );
            }
        }

        private static void SetRectanglesSmallTop(out RectangleF rect, out RectangleF trRect, out RectangleF pronRect, TextWithProps text)
        {
            if (string.IsNullOrEmpty(text.TextProps.Pron))
            {
                trRect = new RectangleF(
                _paddingLeft,
                _paddingLeft * 2,
                _width - (_paddingLeft * 2),
                _trHeight);

                rect = new RectangleF(
                    _paddingLeft,
                    _trHeight + (_paddingLeft * 3),
                    _width - (_paddingLeft * 2),
                    _height - (_trHeight + (_paddingLeft * 4)));

                pronRect = new RectangleF(0, 0, 0, 0);
            }
            else
            {
                var pronH = _startPronTextFontSize * 2;
                var bigH = _height - (_trHeight + pronH + (_paddingLeft * 5));

                trRect = new RectangleF(
                    _paddingLeft,
                    _paddingLeft * 2,
                    _width - (_paddingLeft * 2),
                    _trHeight);

                rect = new RectangleF(
                    _paddingLeft,
                    _trHeight + (_paddingLeft * 3),
                    _width - (_paddingLeft * 2),
                    bigH);

                pronRect = new RectangleF(
                    _paddingLeft,
                    _trHeight + bigH + (_paddingLeft * 5),
                    _width - (_paddingLeft * 2),
                    pronH
                    );
            }
        }


        //public static void MakeAndSaveBitmap(string text, string filename, int type)
        //{
        //    if (File.Exists(filename))
        //    {
        //        return;
        //    }

        //    var bmp = GetEmptyBitmap();
        //    RectangleF rectf = GetTopPlace();
        //    var graphics = MakeAndTuneGraphics(bmp);
        //    if (type == 0)
        //    {
        //        // Speach
        //        Font font = CalcFontToFitToRectangle(graphics, text, rectf);
        //        graphics.DrawString(text, font, _penColor, rectf, _format);
        //    }
        //    else 
        //    {
        //        // Dictor
        //        Font font = CalcFontToFitToRectangle(graphics, text, rectf);
        //        graphics.DrawString(text, font, _penColor, rectf, _format);
        //    }

        //    graphics.Flush();
        //    bmp.Save(filename, ImageFormat.Png);
        //}

        public static Graphics MakeAndTuneGraphics(Bitmap bmp)
        {

            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            return g;
        }

        private static Font CalcFontToFitToRectangle(Graphics g, TextWithProps textWithProps, RectangleF rectf, int startSize, FontFamily fontFamily, out SizeF textSize)
        {
            var text = string.IsNullOrEmpty(textWithProps.Text) ? "Tlll" : textWithProps.Text;

            Font stringFont = null;
            textSize = new SizeF();

            for (int fontSize = startSize; fontSize > 10; fontSize -= 10)
            {
                if (fontFamily == null)
                {
                    stringFont = new Font("Arial", fontSize, FontStyle.Regular);
                }
                else
                {
                    stringFont = new Font(fontFamily, fontSize, FontStyle.Regular);
                }

                textSize = g.MeasureString(text, stringFont, width: (int)rectf.Size.Width);
                if (textSize.Height < rectf.Size.Height)
                {
                    break;
                }
            }

            return stringFont;
        }

        //private static RectangleF GetTopPlace()
        //{
        //    return new RectangleF(_paddingLeft,
        //        _paddingLeft, _width - (_paddingLeft * 2), _trHeight);
        //}

        //private static RectangleF GetBottomPlace()
        //{
        //    return new RectangleF(_paddingLeft,
        //        _trHeight + _paddingLeft,
        //        _width - (_paddingLeft * 2),
        //        _height - (_trHeight + (_paddingLeft * 2)));
        //}

        //private static void DrawStringWithCharacterBounds(Graphics gr,
        //    string text, Font font, Rectangle rect, Color color)
        //{
        //    using (StringFormat string_format = new StringFormat())
        //    {
        //        string_format.Alignment = StringAlignment.Center;
        //        string_format.LineAlignment = StringAlignment.Center;

        //        var squizeSize = 5;

        //        if (text.Length > 32)
        //        {
        //            // print text whole string if it more then > 32
        //            gr.DrawString(text, font, new SolidBrush(color), rect, string_format);
        //            return;
        //        }

        //        // print as lines

        //        var range_list = new List<CharacterRange>();
        //        for (int i = 0; i < text.Length; i++)
        //        {
        //            range_list.Add(new CharacterRange(i, 1));
        //        }

        //        string_format.SetMeasurableCharacterRanges(range_list.ToArray());

        //        Region[] regions = gr.MeasureCharacterRanges(
        //            text, font, rect, string_format);

        //        var chars = text.ToCharArray();

        //        float em_height = font.FontFamily.GetEmHeight(font.Style);
        //        var emHeightPixels = ConvertUnits(gr, font.Size, font.Unit, GraphicsUnit.Pixel);
        //        float designToPixels = emHeightPixels / em_height;
        //        var ascentUnits = font.FontFamily.GetCellAscent(font.Style);
        //        var descentUnits = font.FontFamily.GetCellDescent(font.Style);

        //        var clearHeightPixel = ((ascentUnits -
        //            (ascentUnits + descentUnits - em_height))
        //            * designToPixels) / (float)1.25;

        //        var addHeight = Convert.ToInt32(clearHeightPixel / 3);

        //        for (int i = 0; i < text.Length; i++)
        //        {
        //            var charRect = Rectangle.Round(regions[i].GetBounds(gr));
        //            var squizedCharRect = new Rectangle(
        //                charRect.X + squizeSize,
        //                charRect.Y + squizeSize + (addHeight),
        //                charRect.Width - (squizeSize * 2),
        //                Convert.ToInt32(clearHeightPixel)
        //                );

        //            if (chars[i].ToString() == " ")
        //            {
        //                continue;
        //            }

        //            // draw box
        //            //gr.DrawRectangle(new Pen(color), squizedCharRect);

        //            gr.DrawLine(new Pen(color),
        //            squizedCharRect.X,
        //            squizedCharRect.Y + squizedCharRect.Height,
        //            squizedCharRect.X + squizedCharRect.Width,
        //            squizedCharRect.Y + squizedCharRect.Height
        //                );

        //            // gr.DrawString(chars[i].ToString(), font, new SolidBrush(color), charRect, string_format);
        //        }
        //    }
        //}

        //private static float ConvertUnits(Graphics gr, float value,
        //    GraphicsUnit from_unit, GraphicsUnit to_unit)
        //{
        //    if (from_unit == to_unit) return value;

        //    // Convert to pixels. 
        //    switch (from_unit)
        //    {
        //        case GraphicsUnit.Document:
        //            value *= gr.DpiX / 300;
        //            break;
        //        case GraphicsUnit.Inch:
        //            value *= gr.DpiX;
        //            break;
        //        case GraphicsUnit.Millimeter:
        //            value *= gr.DpiX / 25.4F;
        //            break;
        //        case GraphicsUnit.Pixel:
        //            // Do nothing.
        //            break;
        //        case GraphicsUnit.Point:
        //            value *= gr.DpiX / 72;
        //            break;
        //        default:
        //            throw new Exception("Unknown input unit " +
        //                from_unit.ToString() + " in FontInfo.ConvertUnits");
        //    }

        //    // Convert from pixels to the new units. 
        //    switch (to_unit)
        //    {
        //        case GraphicsUnit.Document:
        //            value /= gr.DpiX / 300;
        //            break;
        //        case GraphicsUnit.Inch:
        //            value /= gr.DpiX;
        //            break;
        //        case GraphicsUnit.Millimeter:
        //            value /= gr.DpiX / 25.4F;
        //            break;
        //        case GraphicsUnit.Pixel:
        //            // Do nothing.
        //            break;
        //        case GraphicsUnit.Point:
        //            value /= gr.DpiX / 72;
        //            break;
        //        default:
        //            throw new Exception("Unknown output unit " +
        //                to_unit.ToString() + " in FontInfo.ConvertUnits");
        //    }

        //    return value;
        //}

        public static Bitmap GetEmptyBitmap()
        {
            Bitmap bmp = new Bitmap(_width, _height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, _width, _height);
                graph.FillRectangle(new SolidBrush(_bgrColor), ImageSize);
            }

            return bmp;
        }

        public static void GenerateLessonImages(string baseName, string thumb, string text, string webRootPath, Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            var inputFullPath = $"{FilesIOHelper.NeedFilesPath(webRootPath)}\\eng01.png";
            Image image1 = Image.FromFile(inputFullPath);
            Graphics objGraphic = Graphics.FromImage(image1);
            PrivateFontCollection myFont = new PrivateFontCollection();
            myFont.AddFontFile($"{FilesIOHelper.NeedFilesPath(webRootPath)}\\fonts\\RotondaC-Bold.otf");
            myFont.AddFontFile($"{FilesIOHelper.NeedFilesPath(webRootPath)}\\fonts\\RotondaC.otf");
            FontFamily thisFont = myFont.Families[0];
            Font objFont = new Font(thisFont, 194, FontStyle.Bold);
            SolidBrush objBrushWrite = new SolidBrush(Color.FromArgb(255, 255, 188, 59));
            objGraphic.DrawString(text, objFont, objBrushWrite, new Rectangle(75, 734, 800, 200));

            var outputFileName = $"{FilesIOHelper.GetLessonImagesFolder(webRootPath, _configuration)}\\{baseName}";
            image1.Save(outputFileName, ImageFormat.Jpeg);
            objGraphic.Dispose();

            var ratio = (double)320 / image1.Width;
            var newWidth = (int)(image1.Width * ratio);
            var newHeight = (int)(image1.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image1, 0, 0, newWidth, newHeight);
            }

            var ext = "jpg";

            ImageCodecInfo imageCodecInfo = null;
            var imageFormat = ImageFormat.Jpeg;
            // Get an ImageCodecInfo object that represents the JPEG codec.
            if (ext == "jpg" || ext == "jpeg")
            {
                imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
                imageFormat = ImageFormat.Jpeg;
            }
            else if (ext == "png")
            {
                imageCodecInfo = GetEncoderInfo(ImageFormat.Png);
                imageFormat = ImageFormat.Png;
            }
            else if (ext == "gif")
            {
                imageCodecInfo = GetEncoderInfo(ImageFormat.Gif);
                imageFormat = ImageFormat.Gif;
            }

            outputFileName = $"{FilesIOHelper.GetLessonImagesFolder(webRootPath, _configuration)}\\{thumb}";
            newImage.Save(outputFileName, imageFormat);
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
    }
}
