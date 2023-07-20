using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class DamageNumberGenerator : MonoBehaviour
{
    Character damageableCharacter;

    public DamagePopUp popUp;

    private void Awake()
    {
        damageableCharacter = GetComponent<Character>();
        damageableCharacter.OnDamage += CreateDamagePopUp;
    }
     
    void CreateDamagePopUp(float damage, Vector3 direction)
    {
        DamagePopUp newPopUp = Instantiate(popUp, transform.position, Quaternion.identity);
        newPopUp.SetUp((int) damage, direction);
    }
}
