using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStikeOrb : MonoBehaviour
{
    public float Speed = 20f;
    public int Damage = 10;
    public ParticleSystem HitVFX;
    private Rigidbody _rb;
    public bool isLaunchedDuringRageSpell = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is a Lightning Orb
        if (other.gameObject.CompareTag("LightningOrb"))
        {
            return;
        }


        Character cc = other.gameObject.GetComponent<Character>();
        if (cc != null && !cc.isPlayer || other.gameObject.CompareTag("DamageOrb"))
        {
            int finalDamage = Damage;
            if (isLaunchedDuringRageSpell)
            {
                finalDamage *= 2; // Apply double damage if launched during Rage Spell
            }

            cc.ApplyDamage(finalDamage, transform.position);
            Debug.Log("Lightning Strike Hit");
            Instantiate(HitVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
