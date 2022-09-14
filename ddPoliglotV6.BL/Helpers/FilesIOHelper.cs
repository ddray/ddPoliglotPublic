using ddPoliglotV6.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Helpers
{
    public class FilesIOHelper
    {
        private static string _needFilesFolderName = "00-NeedFiles";
        public static string PauseAudioFileName = "pause4.mp3";

        IConfiguration _configuration;

        public FilesIOHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SavePhraseAudio(string strFileName, byte[] fileData, string rootPath)
        {
            this.SaveAudio(strFileName, fileData, _configuration["PhrasesAudioContainerName"], GetPhrasesAudioFolder(rootPath, _configuration));
        }

        public void SaveArticleAudio(string strFileName, byte[] fileData, string rootPath)
        {
            this.SaveAudio(strFileName, fileData, _configuration["ArticlesAudioContainerName"], GetArticlesAudioFolder(rootPath, _configuration));
        }

        public void SaveAudio(string strFileName, byte[] fileData, string container, string filesFolder)
        {
            //var _task = Task.Run(() => this.UploadFileToBlobAsync(strFileName, fileData, "audio/mpeg", containerName));
            //_task.Wait();
            //string fileUrl = _task.Result;
            //return fileUrl;

            // save as file
            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            File.WriteAllBytes($"{filesFolder}\\{strFileName}", fileData);
        }

        public static string GetPhrasesAudioFolder(string webRootPath, IConfiguration conf)
        {
            return GetProjectRootFolder(webRootPath) + $"\\{conf["PhrasesAudioContainerName"]}";
        }

        public static string GetArticlesAudioFolder(string webRootPath, IConfiguration conf)
        {
            return GetProjectRootFolder(webRootPath) + $"\\{conf["ArticlesAudioContainerName"]}";
        }
 
        public static string GetArticlesVideoFolder(string webRootPath, IConfiguration conf)
        {
            return GetProjectRootFolder(webRootPath) + $"\\{conf["ArticlesVideoContainerName"]}";
        }

        public static string GetLessonImagesFolder(string webRootPath, IConfiguration conf)
        {
            return GetProjectRootFolder(webRootPath) + $"\\{conf["LessonImagesContainerName"]}";
        }

        public static string GetLessonAudioFolder(string webRootPath, IConfiguration conf)
        {
            return GetProjectRootFolder(webRootPath) + $"\\{conf["LessonAudioContainerName"]}";
        }

        private static string GetProjectRootFolder(string webRootPath)
        {
            var removeString = @"\wwwroot";
            int index = webRootPath.IndexOf(removeString);
            var path = (index < 0) ? webRootPath : webRootPath.Remove(index, removeString.Length);
            return path.TrimEnd('\\');
        }

        public static string GetFfmpegExeFilePath(string webRootPath)
        {
            return $"{GetProjectRootFolder(webRootPath)}\\{_needFilesFolderName}\\ffmpeg.exe";
        }
        

        public static string TranslationKeyFileName(string webRootPath)
        {
            var path = NeedFilesPath(webRootPath) + $"\\TranslateText1-0729ae7c81e7.json";
            return path;
        }

        public static string NeedFilesPath(string webRootPath)
        {
            var path = GetProjectRootFolder(webRootPath) + $"\\{_needFilesFolderName}";
            return path;
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType, string containerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["AccessKey"]);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            string fileName = strFileName;

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            if (fileName != null && fileData != null)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                cloudBlockBlob.Properties.ContentType = fileMimeType;
                await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                return cloudBlockBlob.Uri.AbsoluteUri;
            }

            return "";
        }

        internal void ComposeAudioAndSave(string filename, List<string> files, string baseRootPath)
        {
            var inputPath = FilesIOHelper.GetPhrasesAudioFolder(baseRootPath, _configuration);
            var outputPath = FilesIOHelper.GetArticlesAudioFolder(baseRootPath, _configuration);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var fullFileName = $"{outputPath.Trim('\\')}\\{filename.Trim()}";

            using (Stream output = System.IO.File.Create(fullFileName))
            {
                foreach (string inputFileName in files)
                {
                    var inputFullPath = (inputFileName == FilesIOHelper.PauseAudioFileName)
                        ? $"{NeedFilesPath(baseRootPath)}\\{inputFileName}"
                        : $"{inputPath.Trim('\\')}\\{inputFileName}";

                    Mp3FileReader reader = new Mp3FileReader(inputFullPath);
                    if ((output.Position == 0) && (reader.Id3v2Tag != null))
                    {
                        output.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
                    }

                    Mp3Frame frame;
                    while ((frame = reader.ReadNextFrame()) != null)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                }
            }
        }

        public static void CheckAndCreateFolders(string pathTmp, string pathTmpImages, string pathTmpAudios, string pathTmpVideos)
        {
            if (!Directory.Exists(pathTmp))
            {
                Directory.CreateDirectory(pathTmp);
            }

            if (!Directory.Exists(pathTmpImages))
            {
                Directory.CreateDirectory(pathTmpImages);
            }

            if (!Directory.Exists(pathTmpAudios))
            {
                Directory.CreateDirectory(pathTmpAudios);
            }

            if (!Directory.Exists(pathTmpVideos))
            {
                Directory.CreateDirectory(pathTmpVideos);
            }
        }

        internal void CombineAndSaveVideo(string baseRootPath, Article article, List<ArticlePhrase> articlePhrases, string filename)
        {
            var pathRoot = GetProjectRootFolder(baseRootPath);
            var pathTmp = pathRoot + $"\\tmp\\{article.ArticleID}_{Guid.NewGuid().ToString().Replace("-", "")}";
            var pathTmpImages = pathTmp + "\\images";
            var pathTmpAudios = pathTmp + "\\videos";
            var pathTmpVideos = pathTmp + "\\videos";

            if (!Directory.Exists(pathRoot + $"\\tmp"))
            {
                Directory.CreateDirectory(pathRoot + $"\\tmp");
            }

            CheckAndCreateFolders(pathTmp, pathTmpImages, pathTmpAudios, pathTmpVideos);

            var videoBat = new StringBuilder();
            var prevBitmap = ImageHelper.GetEmptyBitmap();

            // make temporary video parts with audio
            foreach (var articlePhrase in articlePhrases)
            {
                // create one audio file for phrase
                var audioFileName = CreateAudioFromPhraseWithPausesAsync(articlePhrase, pathTmpAudios, baseRootPath);

                // create video file for phrase
                var videoFileName = "";
                prevBitmap = VideoHelper.CreateVideoFromPhraseWithPauses(articlePhrase, pathRoot, pathTmpVideos, pathTmpImages, audioFileName, prevBitmap, out videoFileName);

                videoBat.AppendLine($"file '{videoFileName}'");
            }

            // save bat file
            var phrasesVideoBatFileName = pathTmp + $"\\{article.ArticleID}.lst";
            System.IO.File.WriteAllText(phrasesVideoBatFileName, videoBat.ToString());

            // concat phrase videos to one file
            var tmpOutputVideoFileName = pathTmp + $"\\{article.ArticleID}.mp4";
            VideoHelper.ConcatVideo(phrasesVideoBatFileName, tmpOutputVideoFileName, pathRoot);

            // move file to file reposytory
            var pathToSaveFinishVideo = GetArticlesVideoFolder(baseRootPath, _configuration);
            if (!Directory.Exists(pathToSaveFinishVideo))
            {
                Directory.CreateDirectory(pathToSaveFinishVideo);
            }

            var outputPath = $"{pathToSaveFinishVideo}\\{filename}";

            if (File.Exists(outputPath))
            {
                new System.IO.DirectoryInfo(pathToSaveFinishVideo).Delete(true);
            }

            File.Copy(tmpOutputVideoFileName, outputPath);

            // delete all temporary files in pathTmp
            if (_configuration["Mode"] != "Debug")
            {
                new System.IO.DirectoryInfo(pathTmp).Delete(true);
            }
        }

        public string CreateAudioFromPhraseWithPausesAsync(ArticlePhrase articlePhrase, string pathTmpAudios, string baseRootPath)
        {
            var addPauseQty = 0;
            var fileList = new List<string>();

            if (articlePhrase.ActivityType == 0 || articlePhrase.ActivityType == 2)
            {
                // source text first
                if (!string.IsNullOrEmpty(articlePhrase.Text))
                {
                    fileList.Add(articlePhrase.TextSpeechFileName);
                    AudioHelper.AddPauseRepeat(fileList, articlePhrase.Pause + addPauseQty);
                }

                // translation text second
                if (!string.IsNullOrEmpty(articlePhrase.TrText))
                {
                    fileList.Add(articlePhrase.TrTextSpeechFileName);
                    AudioHelper.AddPauseRepeat(fileList, articlePhrase.TrPause + addPauseQty);
                }
            }
            else
            {
                // translation text
                if (!string.IsNullOrEmpty(articlePhrase.TrText))
                {
                    fileList.Add(articlePhrase.TrTextSpeechFileName);
                    AudioHelper.AddPauseRepeat(fileList, articlePhrase.TrPause + addPauseQty);
                }

                // source text
                if (!string.IsNullOrEmpty(articlePhrase.Text))
                {
                    fileList.Add(articlePhrase.TextSpeechFileName);
                    AudioHelper.AddPauseRepeat(fileList, articlePhrase.Pause + addPauseQty);
                }
            }

            var inputPath = GetPhrasesAudioFolder(baseRootPath, _configuration);

            var audioFileName = pathTmpAudios + $"\\{articlePhrase.ArticlePhraseID}.mp3";
            using (var output = System.IO.File.Create(audioFileName))
            {
                // combine all mp3 to one tmp
                foreach (string inputFileName in fileList)
                {
                    var inputFullPath = (inputFileName == PauseAudioFileName)
                        ? $"{NeedFilesPath(baseRootPath)}\\{inputFileName}"
                        : $"{inputPath.Trim('\\')}\\{inputFileName}";

                    Mp3FileReader reader = new Mp3FileReader(inputFullPath);
                    if ((output.Position == 0) && (reader.Id3v2Tag != null))
                    {
                        output.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
                    }

                    Mp3Frame frame;
                    while ((frame = reader.ReadNextFrame()) != null)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                }
            }

            return audioFileName;
        }

        // blob old version
        //internal void CombineAndSaveVideo(string baseRootPath, Article article, List<ArticlePhrase> articlePhrases)
        //{
        //    var pathRoot = baseRootPath;
        //    var pathTmp = pathRoot + $"\\tmp\\{article.ArticleID}_{Guid.NewGuid().ToString().Replace("-", "")}";
        //    var pathTmpImages = pathTmp + "\\images";
        //    var pathTmpAudios = pathTmp + "\\videos";
        //    var pathTmpVideos = pathTmp + "\\videos";

        //    if (!Directory.Exists(pathRoot + $"\\tmp"))
        //    {
        //        Directory.CreateDirectory(pathRoot + $"\\tmp");
        //    }

        //    CheckAndCreateFolders(pathTmp, pathTmpImages, pathTmpAudios, pathTmpVideos);

        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_configuration["AccessKey"]);
        //    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer cloudBlobContainerInput = cloudBlobClient.GetContainerReference(_configuration["PhrasesAudioContainerName"]);

        //    var videoBat = new StringBuilder();
        //    var prevBitmap = ImageHelper.GetEmptyBitmap();

        //    // make temporary video parts with audio
        //    foreach (var articlePhrase in articlePhrases)
        //    {
        //        // create one audio file for phrase
        //        var audioFileName = await AudioHelper.CreateAudioFromPhraseWithPausesAsync(cloudBlobContainerInput, articlePhrase, pathTmpAudios);

        //        // create video file for phrase
        //        var videoFileName = "";
        //        prevBitmap = VideoHelper.CreateVideoFromPhraseWithPauses(articlePhrase, pathRoot, pathTmpVideos, pathTmpImages, audioFileName, prevBitmap, out videoFileName);

        //        videoBat.AppendLine($"file '{videoFileName}'");
        //    }

        //    // save bat file
        //    var phrasesVideoBatFileName = pathTmp + $"\\{article.ArticleID}.lst";
        //    System.IO.File.WriteAllText(phrasesVideoBatFileName, videoBat.ToString());

        //    // concat phrase videos to one file
        //    var outputVideoFileName = pathTmp + $"\\{article.ArticleID}.mp4";
        //    VideoHelper.ConcatVideo(phrasesVideoBatFileName, outputVideoFileName, pathRoot);


        //    CloudBlobContainer cloudBlobContainerOutput = cloudBlobClient.GetContainerReference(_configuration["ArticlesVideoContainerName"]);
        //    if (await cloudBlobContainerOutput.CreateIfNotExistsAsync())
        //    {
        //        await cloudBlobContainerOutput.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        //    }

        //    CloudBlockBlob cloudBlockBlobOutput = cloudBlobContainerOutput.GetBlockBlobReference(filename);
        //    cloudBlockBlobOutput.Properties.ContentType = "video/mpeg";
        //    await cloudBlockBlobOutput.UploadFromFileAsync(outputVideoFileName);


        //    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        //    // delete all temporary files in pathTmp
        //    if (environment == "Production")
        //    {
        //        new System.IO.DirectoryInfo(pathTmp).Delete(true);
        //    }
        //}

        //public static void Combine(string[] inputFiles, string outpath, string filename)
        //{
        //    string tmpMp3 = $"{outpath}\\11111_{Guid.NewGuid().ToString()}.mp3";
        //    // combine all mp3 to one tmp
        //    using (Stream output = System.IO.File.Create(tmpMp3))
        //    {
        //        foreach (string file in inputFiles)
        //        {
        //            Mp3FileReader reader = new Mp3FileReader(file);
        //            if ((output.Position == 0) && (reader.Id3v2Tag != null))
        //            {
        //                output.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
        //            }
        //            Mp3Frame frame;
        //            while ((frame = reader.ReadNextFrame()) != null)
        //            {
        //                output.Write(frame.RawData, 0, frame.RawData.Length);
        //            }
        //        }
        //    }

        //    // convert to wav

        //    int outRate = 8000;
        //    var inFile = tmpMp3;
        //    var outFile = $"{outpath}\\{filename}";
        //    using (var reader = new MediaFoundationReader(inFile))
        //    {
        //        var outFormat = new WaveFormat(outRate, 1); //reader.WaveFormat.Channels);
        //        using (var resampler = new MediaFoundationResampler(reader, outFormat))
        //        {
        //            WaveFileWriter.CreateWaveFile(outFile, resampler);
        //        }
        //    }

        //    File.Delete(tmpMp3);
        //}

        public static string GetTextFromFile(string path)
        {
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);

            var sb = new StringBuilder();
            var line = "";
            while ((line = file.ReadLine()) != null)
            {
                //if (string.IsNullOrWhiteSpace(line))
                //{
                //    continue;
                //}

                sb.AppendLine(line);
            }

            file.Close();

            return sb.ToString();
        }

        public static string GetRndLine(string path)
        {
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);

            var lines = new List<string>();
            var line = "";
            while ((line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                lines.Add(line);
            }

            file.Close();

            Random r = new Random();
            var index = r.Next(1, lines.Count());

            return lines[index-1];
        }
    }

    public class AzureAccess
    { 
        public string DefaultEndpointsProtocol { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string EndpointSuffix { get; set; }
        public string TableEndpoint { get; set; }

        private string connStr;

        public AzureAccess(string connectionString)
        {
            this.connStr = connectionString;
            this.DefaultEndpointsProtocol = this.getValue("DefaultEndpointsProtocol=");
            this.AccountName = this.getValue("AccountName=");
            this.AccountKey = this.getValue("AccountKey=");
            this.EndpointSuffix = this.getValue("EndpointSuffix=");
            this.TableEndpoint = this.getValue("TableEndpoint=");
        }

        public string getValue(string key)
        {
            var arr = this.connStr.Split(key);
            if (arr.Length > 1)
            {
                var arr2 = arr[1].Split(";");
                return arr2[0];
            }
            else
            {
                return "";
            }
        }
    }
}
