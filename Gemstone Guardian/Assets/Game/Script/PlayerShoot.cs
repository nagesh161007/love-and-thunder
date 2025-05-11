using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for RawImage

public class PlayerShoot : MonoBehaviour
{
    public Transform ShootingPoint;
    public GameObject LightningStrikeOrb;
    private Character playerCharacter;

    public RawImage LightningStrikeImage; // RawImage for Lightning Strike cooldown

    private float lastStrikeTime = 0f;
    private const float strikeCooldown = 0.2f; // 0.2 second cooldown

    private const float cooldownOpacity = 0.5f; // Adjust for desired opacity during cooldown

    private void Awake()
    {
        playerCharacter = GetComponent<Character>(); // Get the PlayerCharacter component
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastStrikeTime + strikeCooldown)
        {
            LightningStrike();
            lastStrikeTime = Time.time; // Update the last strike time
            StartCoroutine(ResetLightningStrikeImage());
        }

        // Adjust the opacity of the Lightning Strike image based on cooldown
        SetImageOpacity(LightningStrikeImage, Time.time >= lastStrikeTime + strikeCooldown ? 1f : cooldownOpacity);
    }

    private void LightningStrike()
    {
        Debug.Log("Instantiated Lightning Strike");
        GameObject orbInstance = Instantiate(LightningStrikeOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));

        // Set the orb's Rage Spell property
        LightningStikeOrb orbScript = orbInstance.GetComponent<LightningStikeOrb>();
        if (orbScript != null)
        {
            orbScript.isLaunchedDuringRageSpell = playerCharacter.isRageSpell;
        }
    }

    IEnumerator ResetLightningStrikeImage()
    {
        yield return new WaitForSeconds(strikeCooldown);
        SetImageOpacity(LightningStrikeImage, 1f); // Restore full opacity
    }

    private void SetImageOpacity(RawImage image, float opacity)
    {
        Color color = image.color;
        color.a = opacity;
        image.color = color;
    }
}
