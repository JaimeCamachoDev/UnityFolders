using System.Collections.Generic;
using UnityEngine;

namespace UnityFolders
{
    public class FolderRuleset : ScriptableObject
    {
        public List<FolderRule> rules = new();
    }
}