using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolbarData", menuName = "ScriptableObjects/ToolbarData", order = 1)]
public class ToolbarData : ScriptableObject
{
    public List<string> ScenePaths = new List<string>();
}
