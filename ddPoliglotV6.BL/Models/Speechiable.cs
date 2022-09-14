using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ddPoliglotV6.BL.Models
{
    public class Speechiable
    {
        public string Text { get; set; }

        public Int64 HashCode { get; set; }

        public string TextSpeechFileName { get; set; }

        public int SpeachDuration { get; set; }

        public bool Changed { get; set; }
    }
}
