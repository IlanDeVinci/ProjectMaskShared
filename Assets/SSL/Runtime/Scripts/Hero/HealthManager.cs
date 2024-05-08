using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] private int maxHealth;
    [SerializeField] private int totalLives;
    [SerializeField] private HeroEntity hero;
    [SerializeField] private int damageMultiplier = 1;

    public int currentHealth;
    public int currentLives;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentLives = totalLives;
        healthSlider = GameObject.FindGameObjectWithTag("HPSlider").GetComponent<Slider>();
        healthSlider.value = currentHealth / maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }

        if (currentHealth <= 0)
        { 
            //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    public void TakeDamage(int damage)
    {
        if (GlobalManager.isPlayerClairvoyant)
        {
            damage = maxHealth / 2;
        }
        else
        {
            damageMultiplier = 1;
        }

        Tween.Custom(startValue: currentHealth, endValue: currentHealth - (damage*damageMultiplier), duration: 1, ease: Ease.OutSine,
            onValueChange: newVal => healthSlider.value = newVal / maxHealth);

        currentHealth -= damage * damageMultiplier;
    }
}