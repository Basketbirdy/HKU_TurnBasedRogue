using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Proximity proximity;
    [SerializeField] Animator playerAnim;
    [SerializeField] GameObject HUD;
    private Slider healthSlider;
    [Space]
    [SerializeField] ParticleSystem hitParticles;

    [Header("Combat data")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [Space]
    [SerializeField] float minDamage = 60f;
    [SerializeField] float maxDamage = 120f;
    [Space]
    [SerializeField] float damageMultiplier = 1f;

    // events
    public static event Action onPlayerDeath;

    // Start is called before the first frame update
    void Start()
    {
        GetReferences();
        currentHealth = maxHealth;
    }

    public void DealDamage(IDamagable damagable)
    {
        damagable.TakeDamage(Mathf.RoundToInt(UnityEngine.Random.Range(minDamage * damageMultiplier , maxDamage * damageMultiplier)));

        playerAnim.SetTrigger("Attack");

        Debug.Log("Combat; Dealt damage to: " + damagable); 

        TurnManager.instance.AdvanceTurn(1);
    }

    public void TakeDamage(float damage)
    {
        float newHealth = currentHealth - damage;

        if(newHealth <= 0) {currentHealth = newHealth; Die(); onPlayerDeath?.Invoke(); }
        else
        {
            currentHealth = newHealth;
            UpdateHealthBar();
        }


        ParticleUtils.TriggerSystem(hitParticles, true);
        Debug.Log("Combat; Player took damage");
    }

    private void Die()
    {
        UpdateHealthBar();
        StartCoroutine(GameOverDelay(1));
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    // wait for a given duration before ending the game
    IEnumerator GameOverDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.instance.GameOver();
    }

    private void UpdateHealthBar()
    {
        if (currentHealth <= 0) { healthSlider.value = 0; }
        else
        {
            healthSlider.value = currentHealth;
        }
    }

    private void GetReferences()
    {
        healthSlider = HUD.GetComponentInChildren<Slider>();
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }
}
