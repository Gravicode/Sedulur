﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sedulur.Data
{
    public class AppConstants
    {
        public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/sedulur";
        public const int FACE_WIDTH = 180;
        public const int FACE_HEIGHT = 135;
        public const string AppName = "Sedulur";
        public const string FACE_SUBSCRIPTION_KEY = "a068e60df8254cc5a187e3e8c644f316";
        public const string FACE_ENDPOINT = "https://southeastasia.api.cognitive.microsoft.com/";


        public static string ProxyIP = "";
        public static string SQLConn = "";
        public static string EmailAccount = "sedulur.app@gmail.com";
        public const string GemLic = "EDWG-SKFA-D7J1-LDQ5";
        public static string RedisCon { set; get; }

        public static string GMapApiKey { get; set; }
        public static string BlobConn { get; set; }
        public static string? DefaultPass { get; set; }
      
        
    }
}
