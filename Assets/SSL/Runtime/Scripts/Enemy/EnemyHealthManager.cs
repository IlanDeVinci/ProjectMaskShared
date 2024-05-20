using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] public Slider healthSlider;
    [SerializeField] private int maxHealth;
    [SerializeField] private int totalLives;
    private int damageMultiplier = 1;
    [SerializeField] private GameObject entity;
    [SerializeField] private GameObject dmgText;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private CanvasGroup[] canvasGroups;

    [SerializeField] private GameObject[] toDestroy;
    [SerializeField] private ParticleSystem particle;
    private bool hasDied = false;
    private bool isVulnerable = true;
    public int currentHealth;
    public int currentLives;
    [SerializeField] bool isBoss = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentLives = totalLives;
        if (isBoss)
        {
            healthSlider = GameObject.FindGameObjectWithTag("BossHPBar").GetComponent<Slider>();
        }
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
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            }
            foreach (CanvasGroup canvas in canvasGroups)
            {
                canvas.alpha = alpha;
            }
            yield return new WaitForEndOfFrame();

        }

        yield return die.ToYieldInstruction();
        die.Stop();
        Destroy(gameObject,0.01f);

    }

    private void Update()
    {

        if (currentHealth <= 0)
        {
            if(currentLives > 1)
            {
                currentLives--;
                isVulnerable = false;
                Tween.Custom(0, 1, 3f,  ease:Ease.Linear, onValueChange: val => healthSlider.value = val);
                currentHealth = maxHealth;
            }
            else
            {
                if(isVulnerable)
                {
                    if (!hasDied) StartCoroutine(Die());
                }
            }
            //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
        if (healthSlider.value > 0.99) isVulnerable = true;

    }

    public int GetHP()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isVulnerable)
        {
            var random = new System.Random();
            damage += random.Next((-damage / 5) - 1, (damage / 5) + 1);
            damageMultiplier = 1 * GlobalManager.isNextHitDoubled;
            if (damageMultiplier == 0)
            {
                damageMultiplier = 1;
            }
            GlobalManager.isNextHitDoubled = 1;

            Tween.Custom(startValue: currentHealth, endValue: currentHealth - (damage * damageMultiplier), duration: 1, ease: Ease.OutSine,
                onValueChange: newVal => healthSlider.value = newVal / maxHealth);

            currentHealth -= damage * damageMultiplier;
            GameObject savedText = Instantiate(dmgText, transform.position, Quaternion.identity);
            savedText.GetComponent<DamageTextScript>().value = damage * damageMultiplier;
            savedText.GetComponent<DamageTextScript>().color = Color.yellow;

        }

    }
}