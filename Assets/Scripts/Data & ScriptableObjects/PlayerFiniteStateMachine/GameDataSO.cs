using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameDataSO : ScriptableObject
{
    public int killEnemy = 0;
    public int lives = 3;
}
