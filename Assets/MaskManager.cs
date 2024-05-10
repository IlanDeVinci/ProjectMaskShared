using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskManager : MonoBehaviour
{

    [SerializeField] private Mask _currentMask = Mask.Invisibility;
    public Mask CurrentMask => _currentMask;
    public enum Mask
    {
        Invisibility,
        Clairvoyance
    }

    [SerializeField] private float invisibilityCooldown;
    private float currentInvisibilityTime= 0;
    [SerializeField ] private float invisibilityDuration = 3; 
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject hero;
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private Image maskImage;
    [SerializeField] private List<Sprite> spriteList;

    // Start is called before the first frame update
    private void Awake()
    {
        cooldownSlider = GameObject.FindGameObjectWithTag("MaskSlider").GetComponent<Slider>();
        maskImage = GameObject.FindGameObjectWithTag("MaskImage").GetComponent<Image>();
    }

    public void SetAlpha(float alpha)
    {
        SpriteRenderer[] children = visuals.GetComponentsInChildren<SpriteRenderer>();
        Color newColor;
        foreach (SpriteRenderer child in children)
        {
            newColor = child.color;
            newColor.a = alpha;
            child.color = newColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var upgrade = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.InvisibilityCooldown);
        invisibilityCooldown = upgrade.upgradesList[upgrade.upgradeLevel].upgradeValue;
        var upgrade1 = GlobalUpgrades.Instance.Upgrades.Find(x => x.upgradeType == GlobalUpgrades.UpgradeType.InvisibilityTime);
        invisibilityDuration = upgrade1.upgradesList[upgrade1.upgradeLevel].upgradeValue;

        cooldownSlider.value = currentInvisibilityTime/invisibilityCooldown;
        if (Input.GetKeyDown(KeyCode.H))
        {
            switch(_currentMask)
            {
                case Mask.Invisibility:
                    GlobalManager.isPlayerClairvoyant = true;
                    _currentMask = Mask.Clairvoyance;
                    maskImage.sprite = spriteList[0];
                    break;
                case Mask.Clairvoyance:
                    GlobalManager.isPlayerClairvoyant = false;
                    _currentMask = Mask.Invisibility;
                    maskImage.sprite = spriteList[1];
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GlobalManager.isPlayerClairvoyant = false;

            switch (_currentMask)
            {
                case Mask.Invisibility:
                    if(currentInvisibilityTime > invisibilityCooldown)
                    {
                        GlobalManager.isNextHitDoubled = true;
                        currentInvisibilityTime = 0;
                        Debug.Log("a");
                        SetAlpha(0.3f);
                        hero.tag = "Player";

                    }
                    break;
                case Mask.Clairvoyance:
                    GlobalManager.isPlayerClairvoyant = true;
                    break;
            }
        }
        currentInvisibilityTime += Time.deltaTime;
        if(currentInvisibilityTime > invisibilityDuration)
        {
            SetAlpha(1.0f);
            hero.tag = "PlayerTrigger";
            GlobalManager.isNextHitDoubled = false;

        }
    }
}
