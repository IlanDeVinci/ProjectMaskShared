using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] private int maxHealth;
    [SerializeField] private int totalLives;
    [SerializeField] private int damageMultiplier = 1;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject dmgText;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private GameObject[] toDestroy;
    [SerializeField] private ParticleSystem particle;
    private bool hasDied = false;

    public int currentHealth;
    public int currentLives;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentLives = totalLives;
        healthSlider.value = currentHealth / maxHealth;
    }

    private IEnumerator Die()
    {
        hasDied = true;
        float alpha = 1.0f;
        Tween die = Tween.Custom(1, 0f, 1f, onValueChange: val => alpha = val);
        particle.Play();
        foreach(GameObject game in toDestroy)
        {
            Destroy(game);
        }

        while (die.isAlive)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                spriteRenderer.color = new Color(255, 255, 255, alpha);
            yield return new WaitForEndOfFrame();

        }

        yield return die.ToYieldInstruction();
        die.Stop();
        Destroy(entity,0.01f);

    }

    private void Update()
    {

        if (currentHealth <= 0)
        {
            if(!hasDied) StartCoroutine(Die());
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
        damageMultiplier = 1 * GlobalManager.isNextHitDoubled;
        if(damageMultiplier == 0)
        {
            damageMultiplier = 1;
        }
        GlobalManager.isNextHitDoubled = 1;

        Tween.Custom(startValue: currentHealth, endValue: currentHealth - (damage * damageMultiplier), duration: 1, ease: Ease.OutSine,
            onValueChange: newVal => healthSlider.value = newVal / maxHealth);

        currentHealth -= damage * damageMultiplier;
        GameObject savedText = Instantiate(dmgText, transform.position, Quaternion.identity);
        savedText.GetComponent<DamageTextScript>().value = damage * damageMultiplier;
    }
}