using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpri smallRenderer;
    public PlayerSpri bigRenderer;

    private DeathAnimation deathAnimation;

    public bool big => bigRenderer.enabled;
    public bool small => smallRenderer.enabled;
    public bool dead => deathAnimation;

    private void Awake()
    {
        deathAnimation = GetComponent<DeathAnimation>(); 
    }

    public void Hit()
    {
        if (big)
        {
            Shrink();
        } else
        {
            Death();
        }
    }

    private void Shrink()
    {

    }

    private void Death()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;

        deathAnimation.enabled = true;

        GameManager.instance.ResetLevel(3f);
    }
}
