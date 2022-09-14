using ddPoliglotV6.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Helpers
{
    public static class VideoHelper
    {
        private static int _imagesPerSecond = 1;
        private static bool _isDevEnv = false;

        public static Bitmap CreateVideoFromPhraseWithPauses(ArticlePhrase articlePhrase, string rootPath, string pathTmpVideos, string pathTmpImages, string audioFileName, System.Drawing.Bitmap prevBitmap, out string videoFileName)
        {
            StringBuilder videoBat = null;
            // create images for phrase and translation and add them to bat file
            var newPrevBitmap = GenerateBatForSimplePhrase(articlePhrase, rootPath, pathTmpVideos, pathTmpImages, prevBitmap, out videoBat);

            // save bat file
            var phraseVideoBatFileName = pathTmpVideos + $"\\{articlePhrase.ArticlePhraseID}.lst";
            File.WriteAllText(phraseVideoBatFileName, videoBat.ToString());

            // make video
            var phraseVideoFileName = pathTmpVideos + $"\\{articlePhrase.ArticlePhraseID}.mp4";
            ConvertImagesToVideo(phraseVideoBatFileName, phraseVideoFileName, audioFileName, rootPath);

            videoFileName = phraseVideoFileName;

            return newPrevBitmap;
        }

        private static Bitmap GenerateBatForSimplePhrase(ArticlePhrase articlePhrase, string rootPath, string pathTmpVideos, string pathTmpImages, System.Drawing.Bitmap prevBitmap, out StringBuilder videoBat)
        {
            var videoFileList = new List<string>();

            var imgName = $"{pathTmpImages}\\{articlePhrase.ArticlePhraseID}_0.png";
            var trImgName = $"{pathTmpImages}\\{articlePhrase.ArticlePhraseID}_1.png";

            var imgDuration = articlePhrase.SpeachDuration + articlePhrase.Pause;
            var trImgDuration = articlePhrase.TrSpeachDuration + articlePhrase.TrPause;

            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            // privateFontCollection.AddFontFile($"{rootPath}\\fonts\\HelveticaRegular.ttf");

            privateFontCollection.AddFontFile($"{FilesIOHelper.NeedFilesPath(rootPath)}\\fonts\\HelveticaRegular.ttf");
            // privateFontCollection.AddFontFile($"{FilesIOHelper.NeedFilesPath(rootPath)}\\fonts\\RotondaC.otf");

            //var emptyFileName = $"{pathTmpImages}\\empty.png";
            //if (!File.Exists(emptyFileName))
            //{

            //    var emptyFBmp = ImageHelper.GetEmptyBitmap();
            //    emptyFBmp.Save(emptyFileName, ImageFormat.Png);
            //}

            System.Drawing.Bitmap textBitmap = null;
            System.Drawing.Bitmap trTextBitmap = null;

            // make image for text
            if (!string.IsNullOrEmpty(articlePhrase.Text))
            {
                textBitmap = ImageHelper.MakeAndSaveSimpleBitmap(articlePhrase, imgName, 0, privateFontCollection);
            }

            // make image for trText
            if (!string.IsNullOrEmpty(articlePhrase.TrText))
            {
                trTextBitmap = ImageHelper.MakeAndSaveSimpleBitmap(articlePhrase, trImgName, 1, privateFontCollection);
            }

            if (articlePhrase.ActivityType == 0 || articlePhrase.ActivityType == 2)
            {
                if (!string.IsNullOrEmpty(articlePhrase.Text))
                {
                    AddLineToFileList(videoFileList, imgName, imgDuration);
                }

                if (!string.IsNullOrEmpty(articlePhrase.TrText))
                {
                    AddLineToFileList(videoFileList, trImgName, trImgDuration);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(articlePhrase.TrText))
                {
                    AddLineToFileList(videoFileList, trImgName, trImgDuration);
                }

                if (!string.IsNullOrEmpty(articlePhrase.Text))
                {
                    AddLineToFileList(videoFileList, imgName, imgDuration);
                }
            }

            // videoFileList[videoFileList.Count-1] = emptyFileName;

            //if (videoFileList.Count == 0)
            //{
            //    int a = 2;
            //}

            //if (videoFileList.Count < 10) {
            //    int a = 1;
            //}

            var firstBitmap = videoFileList[0].EndsWith("_0.png")
                ? textBitmap
                : trTextBitmap;

            ////// bigining fade

            ////// replace 10 first gitmaps 
            //////for (int i = 0; i < 10; i++)
            //////{
            //////    videoFileList[i] = $"{pathTmpImages}\\{articlePhrase.ArticlePhraseID}_f{i}.png";
            //////    ImageHelper.CombineBitmaps(prevBitmap, firstBitmap, ((float)i / 10f), videoFileList[i]);
            //////}

            //var emptyBmp = ImageHelper.GetEmptyBitmap();


            //for (int i = 0; i < _imagesPerSecond / 3; i++)
            //{
            //    videoFileList[i] = $"{pathTmpImages}\\{articlePhrase.ArticlePhraseID}_f{i}.png";
            //    ImageHelper.CombineBitmaps(emptyBmp, firstBitmap, ((float)(i + 1) / ((float)(_imagesPerSecond / 3))), videoFileList[i]);
            //}

            //var lastBitmap = videoFileList[videoFileList.Count - 1].EndsWith("_0.png")
            //    ? textBitmap
            //    : trTextBitmap;

            //for (int i = 0; i < _imagesPerSecond / 3; i++)
            //{
            //    var ind = (videoFileList.Count - 1) - ((_imagesPerSecond / 3 - 1) - i);
            //    videoFileList[ind] = $"{pathTmpImages}\\{articlePhrase.ArticlePhraseID}_e{i}.png";
            //    ImageHelper.CombineBitmaps(lastBitmap, emptyBmp, ((float)(i + 1) / ((float)(_imagesPerSecond / 3))), videoFileList[ind]);
            //}

            videoBat = new StringBuilder();
            foreach (var item in videoFileList)
            {
                videoBat.AppendLine($"file '{item}'");
            }

            // dublicate last image in list for audio should be shorte then video to cut by audio duration
            videoBat.AppendLine($"file '{videoFileList[videoFileList.Count - 1]}'");

            // last image in clip, new parev bitmap for next fade
            prevBitmap = videoFileList[videoFileList.Count - 1].EndsWith("_0.png")
                ? textBitmap
                : trTextBitmap;

            return prevBitmap;
        }

        public static void CopyEmptyFileIfNeed(string src, string dist)
        {
            if (!File.Exists(src))
            {
                File.Copy(src, dist);
            }
        }

        private static void AddLineToFileList(List<string> videoBat, string imgName, int imgDuration)
        {
            for (int i = 0; i < imgDuration * _imagesPerSecond; i++)
            {
                videoBat.Add(imgName);
            }
        }

        private static void ConvertImagesToVideo(string batchFileName, string outputFileName, string audioFileName, string rootPath)
        {
            var parameters = $"-y -r {_imagesPerSecond}/1 -f concat -safe 0 -i \"{batchFileName}\" -i \"{audioFileName}\" -c:v libx264 -c:a copy -shortest -vf \"fps=30,format=yuv420p\" \"{outputFileName}\" ";
            RunFfMpeg(parameters, rootPath);
        }

        public static void ConcatVideo(string batchFileName, string outputFileName, string rootPath)
        {
            var parameters = $" -f concat -safe 0 -i \"{batchFileName}\" -c copy \"{outputFileName}\" ";
            RunFfMpeg(parameters, rootPath);
        }

        private static void RunFfMpeg(string parameters, string rootPath)
        {
            var path = FilesIOHelper.GetFfmpegExeFilePath(rootPath);
            ProcessStartInfo oInfo = new ProcessStartInfo(path, parameters);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            //oInfo.UseShellExecute = false;
            //oInfo.CreateNoWindow = true;
            //oInfo.RedirectStandardOutput = true;
            //oInfo.RedirectStandardError = true;

            string output = null;
            StreamReader srOutput = null;

            try
            {
                Process proc = System.Diagnostics.Process.Start(oInfo);
                proc.WaitForExit();
                //srOutput = proc.StandardError;
                //output = srOutput.ReadToEnd();
                proc.Close();
            }
            catch (Exception ex)
            {
                output = string.Empty;
            }
            finally
            {
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
        }
    }
}


