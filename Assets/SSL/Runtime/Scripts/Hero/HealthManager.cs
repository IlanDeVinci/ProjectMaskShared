using System.Collections;
using System.Collections.Generic;
using System.Xml;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GlobalUpgrades;

public class HealthManager : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] private int maxHealth;
    [SerializeField] private int totalLives;
    [SerializeField] private HeroEntity hero;
    [SerializeField] private int damageMultiplier = 1;
    [SerializeField] private GameObject dmgText;

    public int currentHealth;
    public int currentLives;

    private void Awake()
    {
        currentLives = totalLives;
        healthSlider = GameObject.FindGameObjectWithTag("HPSlider").GetComponent<Slider>();
        var upgrade = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.Hp);
        maxHealth = upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
        currentHealth = maxHealth;

        healthSlider.value = currentHealth / maxHealth;
    }

    private void Update()
    {
        var upgrade = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.Hp);
        int newHealth = upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
        if(newHealth != maxHealth)
        {
            Tween.Custom(startValue: currentHealth, endValue: currentHealth + newHealth - maxHealth, duration: 1, ease: Ease.OutSine,
onValueChange: newVal => healthSlider.value = newVal / maxHealth);
            currentHealth += newHealth - maxHealth;
            maxHealth = newHealth;

        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }

        if (currentHealth <= 0)
        { 
            //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    public int GetHP()
    {
        return currentHealth;
    }
    public void TakeDamage(int damage)
    {
        var random = new System.Random();
        damage += random.Next((-damage / 5) - 1, (damage / 5) + 1);
        if (GlobalManager.isPlayerClairvoyant)
        {
            var upgrade = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.Resistance);
            int resistance = upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
            damage = maxHealth / resistance;
            damageMultiplier = 1;
        }
        else
        {
            damageMultiplier = 1;
        }

        Tween.Custom(startValue: currentHealth, endValue: currentHealth - (damage*damageMultiplier), duration: 1, ease: Ease.OutSine,
            onValueChange: newVal => healthSlider.value = newVal / maxHealth);

        currentHealth -= damage * damageMultiplier;
        GameObject savedText = Instantiate(dmgText, transform.position, Quaternion.identity);
        savedText.GetComponent<DamageTextScript>().value = damage * damageMultiplier;
    }
}