using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class WorldItem : MonoBehaviour
{
    [Header("Thông tin item ")]
    public ItemBase item;
    public int amount = 1;

    [Header("Cài đặt nhặt")]
    public float pickupRadius = 1.2f;
    public float moveSpeed = 5f;

    private Transform player;
    private bool isMovingToplayer = false;

    private void Start()
    {
        player= GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (player == null) return;
        float dist=Vector2.Distance(transform.position, player.position);
        if (dist <= pickupRadius)
        {
            isMovingToplayer = true;
        }
        if (isMovingToplayer)
        {
            transform.position=Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            if (dist < 0.3f)
            {
                TryPickup();
            }
        }
    }
    private void TryPickup()
    {
        var storage = player.GetComponentInChildren<StorageContainer>();
        if (storage != null)
        {
            if (storage.AddItem(item, amount))
            {
                Debug.Log($"Đã nhặt {amount} {item.itemName}");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Kho đầy, không thể nhặt thêm");
                isMovingToplayer = false;
            }
        }
    }
}
