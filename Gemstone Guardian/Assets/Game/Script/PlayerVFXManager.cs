using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect footStep;

    public ParticleSystem blade01;
    public ParticleSystem blade02;
    public ParticleSystem blade03;

    public ParticleSystem HealSpell;

    public VisualEffect Slash;
    public VisualEffect Heal;

    public ParticleSystem FreezeSpell;
    public ParticleSystem FireSpell;

    public ParticleSystem BlockSpell;
    public ParticleSystem RageSpell;

    public void update_FootStep(bool state)
    {
        if (state)
        {
            footStep.Play();
        } else
        {
            footStep.Stop();
        }
    }

    public void PlayBlade01()
    {
        blade01.Play();
    }


    public void PlayFireSpell()
    {
        FireSpell.Play();
    }


    public void StopFireSpell()
    {
        Debug.Log("Stoping Fire Spell");
        FireSpell.Stop();
    }

    public void PlayRageSpell()
    {
        RageSpell.Play();
    }


    public void StopRageSpell()
    {
        Debug.Log("Stoping Rage Spell");
        RageSpell.Stop();
    }


    public void PlayBlockSpell()
    {
        BlockSpell.Play();
    }


    public void StopBlockSpell()
    {
        Debug.Log("Stoping Block Spell");
        BlockSpell.Stop();
    }


    public void PlayFreezeSpell()
    {
        FreezeSpell.Play();
    }

    public void StopFreezeSpell()
    {
        Debug.Log("Stoping Freeze Spell");
        FreezeSpell.Stop();
    }


    public void PlayBlade02()
    {
        blade02.Play();
    }


    public void PlayBlade03()
    {
        blade03.Play();
    }


    public void PlayHealSpell()
    {
        HealSpell.Play();
    }


    public void StopHealSpell()
    {
        Debug.Log("Stoping Heal Spell");
        HealSpell.Stop();
    }


    public void PlaySlash(Vector3 pos)
    {
        Slash.transform.position = pos;
        Slash.Play();
    }

    public void PlayHealVFX()
    {
        Heal.Play();
    }

}
