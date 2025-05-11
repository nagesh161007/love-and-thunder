using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    // Input Variables
    public float HorizontalInput;
    public float VerticalInput;

    public bool MouseButtonDown;
    public bool SpaceKeyDown;

    public bool PunchKeyDown;
    public bool KickKeyDown;

    public bool HealKeyDown;
    public bool AttackKeyDown;

    public bool FireSpellKeyDown;
    public bool FreezeSpellKeyDown;

    public bool RageSpellKeyDown;
    public bool BlockSpellKeyDown;

    // UI Images for spells and actions
    public RawImage FireSpellImage;
    public RawImage FreezeSpellImage;
    public RawImage RageSpellImage;
    public RawImage BlockSpellImage;
    public RawImage HealImage; // UI image for Heal

    private Dictionary<string, float> actionLastUsedTime;
    private const float spellCooldown = 10f; // 10 seconds cooldown for spells
    private const float cooldownOpacity = 0.5f; // Adjust for desired opacity during cooldown

    void Start()
    {
        actionLastUsedTime = new Dictionary<string, float>
        {
            {"FireSpell", 0f},
            {"FreezeSpell", 0f},
            {"RageSpell", 0f},
            {"BlockSpell", 0f},
            {"Heal", 0f}
        };
    }

    void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        AttackKeyDown = CheckKey(KeyCode.J, AttackKeyDown);
        PunchKeyDown = CheckKey(KeyCode.L, PunchKeyDown);
        KickKeyDown = CheckKey(KeyCode.K, KickKeyDown);

        HealKeyDown = CheckSpellKey(KeyCode.H, "Heal", HealKeyDown, HealImage);
        FireSpellKeyDown = CheckSpellKey(KeyCode.O, "FireSpell", FireSpellKeyDown, FireSpellImage);
        FreezeSpellKeyDown = CheckSpellKey(KeyCode.U, "FreezeSpell", FreezeSpellKeyDown, FreezeSpellImage);
        RageSpellKeyDown = CheckSpellKey(KeyCode.N, "RageSpell", RageSpellKeyDown, RageSpellImage);
        BlockSpellKeyDown = CheckSpellKey(KeyCode.B, "BlockSpell", BlockSpellKeyDown, BlockSpellImage);

        AdjustSpellImageOpacity(HealImage, "Heal");
        AdjustSpellImageOpacity(FireSpellImage, "FireSpell");
        AdjustSpellImageOpacity(FreezeSpellImage, "FreezeSpell");
        AdjustSpellImageOpacity(RageSpellImage, "RageSpell");
        AdjustSpellImageOpacity(BlockSpellImage, "BlockSpell");
    }

    private bool CheckKey(KeyCode key, bool keyDown)
    {
        if (!keyDown && Time.timeScale != 0)
        {
            return Input.GetKeyDown(key);
        }
        return keyDown;
    }

    private bool CheckSpellKey(KeyCode key, string actionName, bool actionKeyDown, RawImage spellImage)
    {
        if (!actionKeyDown && Time.timeScale != 0 && Time.time > actionLastUsedTime[actionName] + spellCooldown)
        {
            if (Input.GetKeyDown(key))
            {
                actionLastUsedTime[actionName] = Time.time;
                return true;
            }
        }
        return actionKeyDown;
    }

    private void AdjustSpellImageOpacity(RawImage image, string actionName)
    {
        float opacity = Time.time > actionLastUsedTime[actionName] + spellCooldown ? 1f : cooldownOpacity;
        SetImageOpacity(image, opacity);
    }

    private void SetImageOpacity(RawImage image, float opacity)
    {
        Color color = image.color;
        color.a = opacity;
        image.color = color;
    }

    private void OnDisable()
    {
        ClearCache();
    }

    public void ClearCache()
    {
        AttackKeyDown = false;
        PunchKeyDown = false;
        KickKeyDown = false;
        //HealKeyDown = false;
        //FireSpellKeyDown = false;
        //FreezeSpellKeyDown = false;
        //RageSpellKeyDown = false;
        //BlockSpellKeyDown = false;
    }
}
