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

    public int currentHealth;
    public int currentLives;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentLives = totalLives;
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
        Tween.Custom(startValue: currentHealth, endValue: currentHealth - damage, duration: 1, ease: Ease.OutSine,
            onValueChange: newVal => healthSlider.value = newVal / maxHealth);

        currentHealth -= damage;
    }
}