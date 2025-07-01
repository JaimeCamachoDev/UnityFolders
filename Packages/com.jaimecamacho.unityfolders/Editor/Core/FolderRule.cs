using UnityEngine;

namespace UnityFolders
{
    public enum MatchMode { NameContains, PathContains }

    [System.Serializable]
    public class FolderRule
    {
        public MatchMode matchMode;
        public string matchValue;
        public Color backgroundColor = Color.gray;
        public Texture2D iconTexture;
    }
}