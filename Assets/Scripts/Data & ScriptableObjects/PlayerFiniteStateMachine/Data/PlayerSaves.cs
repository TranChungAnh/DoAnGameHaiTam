using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerSaves 
{
    public float playerStates;
    public Vector3 playerPossition;
    public int killCount;
    public int lives;

    public PlayerSaves(float playerStates, Vector3 playerPos, int killCount, int lives)
    {
        this.playerStates = playerStates;
        this.playerPossition = playerPos;
        this.killCount = killCount;
        this.lives = lives;
    }
}
