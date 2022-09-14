using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Helpers
{
    public static class AudioHelper
    {
        private static List<VoiceByLanguage> _voicesByLanguage = new List<VoiceByLanguage>() {
            new VoiceByLanguage() { ln= "sk", voice= "sk-SK-Wavenet-A", def= true, defTrans= true, defDict= true, defDictTransl= true, sex= "f", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.sk },
            new VoiceByLanguage() { ln= "sk", voice= "sk-SK-Wavenet-A", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 2, speed= 1, namePrefix= "AA", languageID = (int)Languages.sk },
            new VoiceByLanguage() { ln= "sk", voice= "sk-SK-Wavenet-A", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= -2, speed= 1, namePrefix= "BB", languageID = (int)Languages.sk },
            new VoiceByLanguage() { ln= "sk", voice= "sk-SK-Wavenet-A", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 1.5, speed= 1, namePrefix= "CC", languageID = (int)Languages.sk },
            new VoiceByLanguage() { ln= "sk", voice= "sk-SK-Wavenet-A", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= -1.5, speed= 1, namePrefix= "DD", languageID = (int)Languages.sk },

            new VoiceByLanguage() { ln= "en", voice= "en-US-Wavenet-A", def= false, defTrans= false, defDict= true, defDictTransl= true, sex= "m", pitch= 0, speed= 1, namePrefix= "AM1", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-US-Wavenet-B", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "m", pitch= 0, speed= 1, namePrefix= "AM2", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-US-Wavenet-C", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "AF1", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-US-Wavenet-D", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "m", pitch= 0, speed= 1, namePrefix= "AM3", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-US-Wavenet-E", def= true, defTrans= true, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "AF2", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-US-Wavenet-F", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "AF3", languageID = (int)Languages.en },

            new VoiceByLanguage() { ln= "en", voice= "en-GB-Wavenet-A", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "GF1", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-GB-Wavenet-B", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "m", pitch= 0, speed= 1, namePrefix= "GM1", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-GB-Wavenet-C", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "GF2", languageID = (int)Languages.en },
            new VoiceByLanguage() { ln= "en", voice= "en-GB-Wavenet-D", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "m", pitch= 0, speed= 1, namePrefix= "GM2", languageID = (int)Languages.en },

            new VoiceByLanguage() { ln= "ru", voice= "ru-RU-Wavenet-A", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.ru },
            new VoiceByLanguage() { ln= "ru", voice= "ru-RU-Wavenet-B", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "m", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.ru },
            new VoiceByLanguage() { ln= "ru", voice= "ru-RU-Wavenet-C", def= true, defTrans= true, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.ru },
            new VoiceByLanguage() { ln= "ru", voice= "ru-RU-Wavenet-D", def= false, defTrans= false, defDict= true, defDictTransl= true, sex= "m", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.ru },
            new VoiceByLanguage() { ln= "ru", voice= "ru-RU-Wavenet-E", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.ru },

            new VoiceByLanguage() { ln= "de", voice= "de-DE-Wavenet-A", def= true, defTrans= true, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.de },
            new VoiceByLanguage() { ln= "de", voice= "de-DE-Wavenet-B", def= false, defTrans= false, defDict= true, defDictTransl= true, sex= "m", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.de },
            new VoiceByLanguage() { ln= "de", voice= "de-DE-Wavenet-C", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "f", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.de },
            new VoiceByLanguage() { ln= "de", voice= "de-DE-Wavenet-D", def= false, defTrans= false, defDict= false, defDictTransl= false, sex= "m", pitch= 0, speed= 1, namePrefix= "", languageID = (int)Languages.de },
        };

        public static VoiceByLanguage GetDefaultVoiceByLanguage(int languageID)
        {
            return _voicesByLanguage.FirstOrDefault(x => x.languageID == languageID && x.def);
        }

        public static VoiceByLanguage GetDefaultDictorVoiceByLanguage(int languageID)
        {
            return _voicesByLanguage.FirstOrDefault(x => x.languageID == languageID && x.defDict);
        }

        //public static async Task<string> CreateAudioFromPhraseWithPausesToBlobAsync(CloudBlobContainer cloudBlobContainerInput, ArticlePhrase articlePhrase, string pathTmpAudios)
        //{
        //    var audioFileName = pathTmpAudios + $"\\{articlePhrase.ArticlePhraseID}.mp3";
        //    var addPauseQty = 0;
        //    using (var output = System.IO.File.Create(audioFileName))
        //    {
        //        var fileList = new List<string>();
        //        if (articlePhrase.ActivityType == 0 || articlePhrase.ActivityType == 2)
        //        {
        //            // source text first
        //            if (!string.IsNullOrEmpty(articlePhrase.Text))
        //            {
        //                fileList.Add(articlePhrase.TextSpeechFileName);
        //                AddPauseRepeat(fileList, articlePhrase.Pause + addPauseQty);
        //            }

        //            // translation text second
        //            if (!string.IsNullOrEmpty(articlePhrase.TrText))
        //            {
        //                fileList.Add(articlePhrase.TrTextSpeechFileName);
        //                AddPauseRepeat(fileList, articlePhrase.TrPause + addPauseQty);
        //            }
        //        }
        //        else
        //        {
        //            // translation text
        //            if (!string.IsNullOrEmpty(articlePhrase.TrText))
        //            {
        //                fileList.Add(articlePhrase.TrTextSpeechFileName);
        //                AddPauseRepeat(fileList, articlePhrase.TrPause + addPauseQty);
        //            }

        //            // source text
        //            if (!string.IsNullOrEmpty(articlePhrase.Text))
        //            {
        //                fileList.Add(articlePhrase.TextSpeechFileName);
        //                AddPauseRepeat(fileList, articlePhrase.Pause + addPauseQty);
        //            }
        //        }

        //        // combine all mp3 to one tmp
        //        foreach (string file in fileList)
        //        {
        //            CloudBlockBlob cloudBlockBlob = cloudBlobContainerInput.GetBlockBlobReference(file);

        //            Mp3FileReader reader = new Mp3FileReader(await cloudBlockBlob.OpenReadAsync());
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

        //    return audioFileName;
        //}

        public static void AddPauseRepeat(List<string> files, int pause)
        {
            for (int i = 0; i < pause; i++)
            {
                files.Add(FilesIOHelper.PauseAudioFileName);
            }
        }

        public static string TextToSsml(string text)
        {
            // 
            // ~|Live |{"Pron":"lɪv"}
            // ~~||Live ||{"Pron":"lɪv"}
            if (text == "Lead, led, led {\"Pron\":\"liːd, led, led\"}")
            {
                text = "Lead. led. led. {\"Pron\":\"liːd, led, led\"}";
            }

            if (text == "~|Lead, |led, |led |{\"Pron\":\"liːd, |led, |led\"}")
            {
                text = "~|Lead. |led. |led |{\"Pron\":\"liːd, |led, |led\"}";
            }

            if (text == "~~||Lead, ||led, ||led ||{\"Pron\":\"liːd, ||led, ||led\"}")
            {
                text = "~~||Lead. ||led. ||led ||{\"Pron\":\"liːd, ||led, ||led\"}";
            }

            if (text == "Live {\"Pron\":\"lɪv\"}")
            {
                text = "Live, {\"Pron\":\"lɪv\"}";
            }

            if (text == "~|Live |{\"Pron\":\"lɪv\"}")
            {
                text = "~|Live, |{\"Pron\":\"lɪv\"}";
            }

            if (text == "~~||Live ||{\"Pron\":\"lɪv\"}")
            {
                text = "~~||Live, ||{\"Pron\":\"lɪv\"}";
            }


            string clearText = (new TextWithProps(text)).Text;
            var result = clearText
            .Replace("|||||+", "<break time=\"1s\" strength=\"x-strong\" />")
            .Replace("|||||-", "<break time=\"1s\" strength=\"x-weak\" />")
            .Replace("||||+", "<break time=\"800ms\" strength=\"x-strong\" />")
            .Replace("||||-", "<break time=\"800ms\" strength=\"x-weak\" />")
            .Replace("|||+", "<break time=\"600ms\" strength=\"x-strong\" />")
            .Replace("|||-", "<break time=\"600ms\" strength=\"x-weak\" />")
            .Replace("||+", "<break time=\"400ms\" strength=\"x-strong\" />")
            .Replace("||-", "<break time=\"400ms\" strength=\"x-weak\" />")
            .Replace("|+", "<break time=\"200ms\" strength=\"x-strong\" />")
            .Replace("|-", "<break time=\"200ms\" strength=\"x-weak\" />")
            .Replace("|||||", "<break time=\"1s\" />")
            .Replace("||||", "<break time=\"800ms\" />")
            .Replace("|||", "<break time=\"600ms\" />")
            .Replace("||", "<break time=\"400ms\" />")
            .Replace("|", "<break time=\"200ms\" />")
            .Replace("(*", "<emphasis level=\"reduce\">")
            .Replace("(", "<emphasis level=\"strong\">")
            .Replace(")", "</emphasis>");

            var startSlow = "";
            var endSlow = "";
            if (text.StartsWith("~~"))
            {
                startSlow = "<prosody rate = \"40%\"> ";
                endSlow = "</prosody>";
            }
            else if (text.StartsWith("~"))
            {
                startSlow = "<prosody rate = \"65%\"> ";
                endSlow = "</prosody>";
            }

            result = result.Replace("~", "");

            var sb = new StringBuilder();
            sb.AppendLine("<speak> ");

            if (startSlow != "")
            {
                sb.AppendLine(startSlow);
            }

            // var ar = result.Split(new char[] { '.', '!', '?', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            var ar = System.Text.RegularExpressions.Regex.Split(result, @"(?<=[.!?;:])");
            ar = ar.ToList().Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (ar.Length > 0)
            {
                sb.AppendLine(" <p> ");
                foreach (var item in ar)
                {
                    sb.AppendLine($" <s> {item.Trim()} </s> ");
                }

                sb.AppendLine(" </p> ");
            }
            else
            {
                sb.AppendLine(text.Trim());
            }

            if (startSlow != "")
            {
                sb.AppendLine(endSlow);
            }

            sb.AppendLine(" </speak>");
            return sb.ToString();
        }

        public static string formatTextAsSlowAudio(string text, int slowLevel)
        {
            var arr = text.Split(" ");
            var result = slowLevel == 1 ? "~" : slowLevel == 2 ? "~~" : "";
            var pause = slowLevel == 1 ? "|" : slowLevel == 2 ? "||" : "";
            foreach (var str in arr)
            {
                result += (pause + str + " ");
            }

            return result.Trim();
        }

        public static Speechiable MakeSpeechIfNeed(
            ddPoliglotDbContext context,
            IConfiguration configuration,
            VoiceByLanguage voice,
            string rootPath,
            string text,
            Int64 hashCode,
            string textSpeechFileName,
            int speachDuration,
            Func<string, int> getHash
            )
        {
            var result = new Speechiable()
            {
                Text = text,
                Changed = false,
                HashCode = hashCode,
                SpeachDuration = speachDuration,
                TextSpeechFileName = textSpeechFileName
            };

             if (string.IsNullOrWhiteSpace(result.Text)
                && !string.IsNullOrWhiteSpace(result.TextSpeechFileName))
            {
                // text empty but speach exists, clear speech
                result.TextSpeechFileName = "";
                result.SpeachDuration = 0;
                result.HashCode = 0;
                result.Changed = true;
            }
            else
            {
                var oldPhraseSpeachHashCode = HashManager.GetHashCodeFromFileName(result.TextSpeechFileName);

                // calc new hash
                if (getHash != null)
                {
                    result.HashCode = getHash("");
                }
                else
                {
                    result.HashCode = HashManager.GetHashFromTextAndVoice(text
                        , voice.ln
                        , voice.voice
                        , (decimal)voice.pitch
                        , (decimal)voice.speed);
                }

                if (!string.IsNullOrWhiteSpace(result.Text) // text exists
                    && oldPhraseSpeachHashCode != result.HashCode // and file is empty or wrong for this phrase
                    )
                {
                    var duration = 0;
                    var filename = "";

                    var speechFile = context.SpeechFiles.FirstOrDefault(x => x.HashCode == result.HashCode);

                    if (speechFile == null)
                    {
                        // there is no speach with such text before
                        var credential = GoogleCredential.FromFile(FilesIOHelper.TranslationKeyFileName(rootPath))
                            .CreateScoped(TextToSpeechClient.DefaultScopes);
                        var channel = new Grpc.Core.Channel(TextToSpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
                        var clientTTS = TextToSpeechClient.Create(channel);

                        var resultSsml = AudioHelper.TextToSsml(result.Text);

                        var response = clientTTS.SynthesizeSpeech(new SynthesizeSpeechRequest
                        {
                            Input = new SynthesisInput
                            {
                                Ssml = resultSsml
                            },
                            Voice = new VoiceSelectionParams
                            {
                                LanguageCode = voice.ln.ToLower() == "en"
                                ? "en-US"
                                : $"{voice.ln.ToLower()}-{voice.ln.ToUpper()}",
                                Name = voice.voice
                            },
                            AudioConfig = new AudioConfig
                            {
                                AudioEncoding = AudioEncoding.Mp3,
                                SpeakingRate = (double)voice.speed,
                                Pitch = (double)voice.pitch
                            }
                        });

                        duration = AudioHelper.GetAudioDuration(response.AudioContent.ToByteArray());
                        filename = $"{result.HashCode.ToString().Replace("-", "n")}_{duration}_phrase.mp3";

                        (new FilesIOHelper(configuration))
                            .SavePhraseAudio(filename, response.AudioContent.ToByteArray(), rootPath);

                        context.SpeechFiles.Add(new SpeechFile()
                        {
                            Duration = duration,
                            HashCode = result.HashCode,
                            SpeechFileName = filename,
                            Version = 1
                        });

                        context.SaveChanges();
                    }
                    else
                    {
                        // speach for text with such hash already exists
                        filename = speechFile.SpeechFileName;
                        duration = speechFile.Duration;
                    }

                    result.TextSpeechFileName = filename;
                    result.SpeachDuration = duration;

                    result.Changed = true;
                }
            }

            return result;
        }

        public class DictorVariants
        { 
            public string name { get; set; }
            public List<List<string>> variants { get; set; }
        }

        public static List<List<List<ArticlePhrase>>> UpdateDictorAudio(
            ddPoliglotDbContext context,
            IConfiguration configuration,
            string rootPath,
            Language language,
            string text,
            bool clean = false)
        {
            var result = new List<List<List<ArticlePhrase>>>();
            if (!string.IsNullOrEmpty(text))
            {
                var version = 0;

                if (text.Trim().StartsWith("{"))
                {
                    var dictorVariants = JsonConvert.DeserializeObject<DictorVariants>(text.Trim());
                    foreach (var variant in dictorVariants.variants)
                    {
                        var resultBlock = new List<List<ArticlePhrase>>();
                        var blocks = variant.ToArray();
                        for (int i = 0; i < blocks.Length; i++)
                        {
                            blocks[i] = blocks[i].Replace("\n", "").Replace("\r", "").Replace("-nl-", "\n");
                        }

                        foreach (var block in blocks)
                        {
                            var linesList = new List<ArticlePhrase>();
                            string[] lines;
                            lines = block.Split(new[] { '\r', '\n' });

                            foreach (var line in lines)
                            {
                                if (!string.IsNullOrEmpty(line.Trim()))
                                {
                                    var voice = GetDefaultDictorVoiceByLanguage(language.LanguageID);
                                    var tmpPhrase = new ArticlePhrase()
                                    {
                                        ArticleActor = new ArticleActor()
                                        {
                                            Language = language.ShortName,
                                            VoiceName = voice.voice,
                                            VoicePitch = (decimal)voice.pitch,
                                            VoiceSpeed = (decimal)voice.speed
                                        },
                                        Text = line.ToOneSpace().Trim()
                                    };

                                    var speechData = AudioHelper.MakeSpeechIfNeed(
                                        context,
                                        configuration,
                                        voice,
                                        rootPath,
                                        line,
                                        0,
                                        "",
                                        0,
                                        tmpPhrase.GetPhraseHash
                                        );

                                    tmpPhrase.HashCode = speechData.HashCode;
                                    tmpPhrase.SpeachDuration = speechData.SpeachDuration;
                                    tmpPhrase.TextSpeechFileName = speechData.TextSpeechFileName;
                                    if (clean)
                                    {
                                        tmpPhrase.Article = null;
                                        tmpPhrase.ArticleActor = null;
                                        tmpPhrase.TrArticleActor = null;
                                        tmpPhrase.ParentKeyGuid = "";

                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                        //tmpPhrase
                                    }
                                    linesList.Add(tmpPhrase);
                                }
                            }

                            if (linesList.Count > 0)
                            {
                                resultBlock.Add(linesList);
                            }
                        }

                        result.Add(resultBlock);
                    }

                    version = 1;
                }
                else
                {
                    var blocks = text.Split("///");
                    version = 0;
                    var resultBlock = new List<List<ArticlePhrase>>();

                    foreach (var block in blocks)
                    {
                        var linesList = new List<ArticlePhrase>();
                        string[] lines;
                        lines = block.Split(new[] { '\r', '\n' });

                        foreach (var line in lines)
                        {
                            if (!string.IsNullOrEmpty(line.Trim()))
                            {
                                var voice = GetDefaultDictorVoiceByLanguage(language.LanguageID);
                                var tmpPhrase = new ArticlePhrase()
                                {
                                    ArticleActor = new ArticleActor()
                                    {
                                        Language = language.ShortName,
                                        VoiceName = voice.voice,
                                        VoicePitch = (decimal)voice.pitch,
                                        VoiceSpeed = (decimal)voice.speed
                                    },
                                    Text = line.Trim()
                                };

                                var speechData = AudioHelper.MakeSpeechIfNeed(
                                    context,
                                    configuration,
                                    voice,
                                    rootPath,
                                    line,
                                    0,
                                    "",
                                    0,
                                    tmpPhrase.GetPhraseHash
                                    );

                                tmpPhrase.HashCode = speechData.HashCode;
                                tmpPhrase.SpeachDuration = speechData.SpeachDuration;
                                tmpPhrase.TextSpeechFileName = speechData.TextSpeechFileName;
                                linesList.Add(tmpPhrase);
                            }
                        }

                        if (linesList.Count > 0)
                        {
                            resultBlock.Add(linesList);
                        }
                    }
                }                
            }

            return result;
        }

        public static int GetAudioDuration(byte[] allBytes)
        {
            ///int byterate = BitConverter.ToInt32(new[] { allBytes[28], allBytes[29], allBytes[30], allBytes[31] }, 0);
            int duration = (allBytes.Length - 8) / 4000;
            return duration == 0 ? 1 : duration;
        }
    }
}
