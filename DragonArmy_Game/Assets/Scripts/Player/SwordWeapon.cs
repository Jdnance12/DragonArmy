using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MonoBehaviour
{
    [SerializeField] public float damage;
    [SerializeField] float currentDamage;
    [SerializeField] public Collider bladeCollider;

    [SerializeField] public GameObject GameManager;
    [SerializeField] public UpgradeManager UpgradeManager;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        UpgradeManager = GameManager.GetComponent<UpgradeManager>();

        bladeCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentDamage = UpgradeManager.GetUpgradedSwordDamage();
    }
    public void EnableCollider()
    {
        bladeCollider.enabled = true;
    }
    public void DisableCollider()
    {
        bladeCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage damageable = other.GetComponent<IDamage>();

        if(damageable != null)
        {
            damageable.takeDamage(currentDamage);
        }
    }
}
