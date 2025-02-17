using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] GameObject swordWeapon;

    [Header("Player Stats")]
    [SerializeField] float baseHP;
    [SerializeField] public float upgradedHP;

    [Header("Weapon Damagers")]
    [SerializeField] float baseSwordDamage;
    [SerializeField] public float upgradedSwordDamage;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        swordWeapon = GameObject.Find("Sword_Model");

        baseHP = gameManager.playerScript.maxHP;
        baseSwordDamage = swordWeapon.GetComponent<SwordWeapon>().damage;
        upgradedSwordDamage = baseSwordDamage + 10f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float GetUpgradedSwordDamage()
    {
        return upgradedSwordDamage;
    }
}
