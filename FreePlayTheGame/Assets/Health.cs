using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] Slider slider;
    float curHealth;


    void Start(){
        curHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.minValue = 0f;
        slider.value = curHealth;
    }

    void InitSlider(){
        slider.maxValue = maxHealth;
        slider.minValue = 0f;
        slider.value = curHealth;
        UpdateHealth();
    }

    void UpdateHealth(){
        slider.value = curHealth;
    }

    public void DealDamage(float dmg){
        dmg = Mathf.Clamp(dmg, 0f, curHealth);
        curHealth -= dmg;
        UpdateHealth();
        if(curHealth <= 0){
            Die();
        }
        
    }

    public void Heal(float amount){
        amount = Mathf.Clamp(amount, 0f, maxHealth-curHealth);

        curHealth += amount;
        UpdateHealth();
    }

    void Die(){
        Debug.Log("Died");
    }

    public float GetCurHealth(){
        return curHealth;
    }
}
