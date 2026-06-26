#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VzFolders.IO
{
    [Serializable]
    public class VzFoldersConfig
    {
        public string vzFoldersConfig = "1.0";
        public string exportedAt;
        public string exportedFrom;
        public List<FolderConfig> folders = new();
        public PaletteConfig palette;
    }

    [Serializable]
    public class FolderConfig
    {
        public string path;
        public string name;
        public int colorIndex;
        public bool isColorRecursive;
        public string iconName;
        public bool isIconRecursive;
        public string customIconOriginalPath;
        public string customIconEmbedded;
        public string customIconFileName;
    }

    [Serializable]
    public class PaletteConfig
    {
        public List<ColorEntry> colors = new();
        public float colorSaturation = 1f;
        public float colorBrightness = 1f;
        public bool colorGradientsEnabled = true;

        [Serializable]
        public class ColorEntry
        {
            public float r, g, b, a;
        }
    }
}
#endif
