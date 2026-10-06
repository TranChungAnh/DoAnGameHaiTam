using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newAggressiveWeaponData", menuName = "Data/Weapon Data/Aggressive Weapon")]
public class SO_AggressiveWeaponData : SO_weaponData
{
    [SerializeField] private WeaponAttackDetails[] weaponAttackDetails;

    public WeaponAttackDetails[] AttackDetails
    {
        get => weaponAttackDetails;
        private set => weaponAttackDetails = value;
    }

    private void OnEnable()
    {
        if (weaponAttackDetails == null) return;

        amountOfAttack = weaponAttackDetails.Length;
        movementSpeed = new float[amountOfAttack];

        for (int i = 0; i < amountOfAttack; i++)
        {
            movementSpeed[i] = weaponAttackDetails[i].movementSpeed;
        }
    }

    public WeaponAttackDetails GetAttackDetails(int attackIndex)
    {
        return weaponAttackDetails[attackIndex];
    }
}