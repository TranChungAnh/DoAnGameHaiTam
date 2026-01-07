using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ToolType {None,Axe,Pickaxe,Sickle,Hoe }
public static class ToolHelper 
{
    public static bool IsValidTool(ResourceType type, ToolType tool)
    {
        return type switch
        {
            ResourceType.Tree => tool == ToolType.Axe,
            ResourceType.Rock => tool == ToolType.Pickaxe,
            ResourceType.Ore => tool == ToolType.Pickaxe,
            ResourceType.Bush => tool == ToolType.Sickle,
            _ => false,
        };
    }
}
