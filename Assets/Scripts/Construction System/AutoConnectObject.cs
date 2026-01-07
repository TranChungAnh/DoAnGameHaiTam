using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class AutoConnectObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private  string buildingGroup; // nhóm chung, ví dụ "Fence"
    private string buildingName;
    private float tileSize = 1f;
    private Sprite[] sprites;

    private void Awake()
    {
        sr=GetComponent<SpriteRenderer>();
    }
    public void Init(string name, string group,float gridSize, Sprite[] connectSprites)
    {
        buildingName = name;
        buildingGroup = group;
        tileSize = gridSize;
        sprites = connectSprites;
        UpdateAppearance();
        Invoke(nameof(UpdateNeighbors), 0.05f);
    }
    public void UpdateAppearance()
    {
        int mask = 0;
        if (HasNeighbor(Vector2.up)) mask |= 1;
        if (HasNeighbor(Vector2.right)) mask |= 2;
        if (HasNeighbor(Vector2.down)) mask |= 4;
        if (HasNeighbor(Vector2.left)) mask |= 8;
        //
        if(sprites !=null && sprites.Length>mask && sprites[mask] != null)
        {
            sr.sprite = sprites[mask];
        }
    }
    // dùng cho ghost 
    public void PreviewConnect(string name, string group, float size, Sprite[] spriteSet)
    {
        buildingName = name;
        buildingGroup = group;
        tileSize = size;
        sprites = spriteSet;
        UpdateNeighbors(isPreview: true);
    }

    private bool HasNeighbor(Vector2 dir)
    {
        Vector2 checkPos = (Vector2)transform.position + dir * tileSize;
        Collider2D hit =Physics2D.OverlapPoint(checkPos);
        if (hit != null)
        {
            var neighbor=hit.GetComponent<AutoConnectObject>();
            if (neighbor != null && neighbor.buildingGroup == buildingGroup)
                return true;


        }
        return false;
    }
    public void UpdateNeighbors(bool isPreview = false)
    {
        int mask = 0;
        Vector2[] dirs = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        for (int i = 0; i < dirs.Length; i++)
        {
            Collider2D hit = Physics2D.OverlapPoint((Vector2)transform.position + dirs[i] * tileSize * 3f);
            if (hit != null && hit.GetComponent<AutoConnectObject>() != null)
            {
                mask |= (1 << i);
            }
        }

        if (sprites != null && mask < sprites.Length)
            sr.sprite = sprites[mask];
    }
    private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Vector2[] dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    foreach (var dir in dirs)
    {
        Vector2 checkPos = (Vector2)transform.position + dir * tileSize;
        Gizmos.DrawWireCube(checkPos, Vector3.one * 0.2f);
    }
}



}
