using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LootTableEntry
{
    public ItemBase itemRef;
    public float dropChance = 1f;
    public int minCount = 1;
    public int maxCount = 1;
}

[CreateAssetMenu(fileName ="NewLootTable", menuName ="Game/Loot Table")]
public class LootTable : ScriptableObject
{
    public LootTableEntry[] entries;
}
