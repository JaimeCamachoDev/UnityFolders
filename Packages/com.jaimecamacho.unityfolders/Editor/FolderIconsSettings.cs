using System;
using System.Collections.Generic;
using UnityEngine;

public class FolderIconsSettings : ScriptableObject
{
    public List<FolderIconRule> rules = new();

    public bool showPreview = true;
}

[Serializable]
public class FolderIconRule
{
    public string ruleName = "Nueva regla";
    public MatchType matchType = MatchType.Name;
    public string match = "";
    public Texture2D iconSmall;
    public Texture2D iconLarge;
    public Texture2D overlayIcon;
    [HideInInspector] public Texture2D previewSmall;
    [HideInInspector] public Texture2D previewLarge;
    public Color background = new(1f, 1f, 1f, 0.25f);
    public bool enabled = true;
    public bool eraseDefault = true;
    public int priority = 0;
}

public enum MatchType
{
    Name,
    Path,
    Regex
}
