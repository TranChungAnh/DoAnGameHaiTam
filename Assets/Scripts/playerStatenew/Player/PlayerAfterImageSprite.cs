using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet = 0.8f;
    [SerializeField]
    private float alphaDecay = 0.85f;

    private Transform player;

    private SpriteRenderer SR;
    private SpriteRenderer playerSR;

    private Color color;

    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();

        if (SR == null)
        {
            Debug.LogError("SpriteRenderer is missing on " + gameObject.name);
            return;  // Không làm gì thêm nếu thiếu SpriteRenderer
        }

        player = GameObject.Find("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        if (playerSR == null)
        {
            Debug.LogError("SpriteRenderer missing on player!");
            return;  // Dừng lại nếu không tìm thấy SpriteRenderer trên player
        }

        alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }


    private void Update()
    {
        alpha -= alphaDecay * Time.deltaTime;
        color = new Color(1f, 1f, 1f, alpha);
        SR.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }

    }

}
