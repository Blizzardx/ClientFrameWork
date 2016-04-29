using UnityEngine;
using System.Collections;
using System.IO;

namespace Assets.Scripts.Framework.Network
{
    public class AssetFile
    {
        public enum AssetFileType
        {
            NONE = 0,
            TEXT = 1,
            BYTE = 2
        }

        public string Url;
        public string LocalPath;
        public AssetFileType Type;
        public string Name;
        public bool IsSaveToFile;

        public bool Exists
        {
            get { return File.Exists(this.LocalPath); }
        }

        public AssetFile(string name, string localPath, string url)
        {
            Name = name;
            Url = url;
            LocalPath = localPath;

            if (LocalPath.IndexOf(".txt") > 0 || LocalPath.IndexOf(".xml") > 0)
            {
                Type = AssetFileType.TEXT;
            }
            else
            {
                Type = AssetFileType.BYTE;
            }
            IsSaveToFile = !string.IsNullOrEmpty(LocalPath);
        }
    }
}
