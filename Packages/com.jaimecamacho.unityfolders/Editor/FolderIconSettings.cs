using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FolderIconRule
{
    public string name = "Nueva Regla";
    public RuleType ruleType = RuleType.Name;
    public string match = "FolderName";
    public Texture2D icon;
    public Color color = Color.white;
    public bool enabled = true;
    public int priority = 0;
}

public enum RuleType
{
    Name,
    Path,
    Regex
}

public class FolderIconSettings : ScriptableObject
{
    public List<FolderIconRule> rules = new List<FolderIconRule>();
}
