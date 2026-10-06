using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/ Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move state")]
    public float movementVelocity = 10f;
    [Header("Dash State")]
    public float dashCooldown = 0.5f;
    public float holdTimeScale = 0.25f;
    public float maxHoldTime = 1f;
    public float dashTime = 0.2f;
    public float dashVelocity = 30f;
    public float dashEndYMultiplier = 0.2f;
    public float drag = 5f;
    public float distBetweenAfterImages = 0.5f;
  


}
