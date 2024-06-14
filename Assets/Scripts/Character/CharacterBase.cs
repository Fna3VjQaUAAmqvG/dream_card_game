using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public int maxHP;
    public IntVariable hp;
    public IntVariable defense;
    public IntVariable buffRound;
    public int currentHP { get => hp.currentValue; set => hp.SetValue(value); }
    public int MaxHP { get => hp.maxValue; }
    protected Animator animator; //subclass accessable
    public bool isDead;
    public GameObject buff;
    public GameObject debuff;
    public float baseStrength = 1f;
    private float strengthEffect = 0.5f;
    [Header("广播变量")]
    public ObjectEventSO characterDeadEvent;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    protected virtual void Start()
    {
        hp.maxValue = maxHP;
        currentHP = maxHP;
        buffRound.currentValue = buffRound.maxValue;
        ResetDefense();
    }

    protected virtual void Update()
    {
        animator.SetBool("isDead", isDead);
    }
    public virtual void TakeDamage(int damage)
    {
        var currentDamage = (damage - defense.currentValue) >= 0 ? (damage - defense.currentValue) : 0;
        var currentDefense = (damage - defense.currentValue) >= 0 ? 0 : (defense.currentValue - damage);
        defense.SetValue(currentDefense);
        if (currentHP > damage)
        {
            currentHP -= damage;
            animator.SetTrigger("hit");
        }
        else
        {
            currentHP = 0;
            isDead = true;
            characterDeadEvent.RaiseEvent(this,this);
        }
        //TODO:die
    }
    public void UpdateDefense(int amount)
    {
        var value = defense.currentValue + amount;
        defense.SetValue(value);
    }
    public void ResetDefense()
    {
        defense.SetValue(0);
    }
    public void HealHealth(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        buff.SetActive(true);
    }

    public void SetupStrength(int round, bool isPositive)
    {
        if (isPositive)
        {
            float newStrength = baseStrength + strengthEffect;
            baseStrength = Mathf.Min(newStrength, 1.5f);
            buff.SetActive(true);
        }
        else
        {
            debuff.SetActive(true);
            baseStrength = 1 - strengthEffect;
        }

        var currentRound = buffRound.currentValue + round;
        if (baseStrength == 1)
        {
            buffRound.SetValue(0);
        }
        else
        {
            buffRound.SetValue(currentRound);
        }
    }
    public void UpdateStrengthRound()
    {
        buffRound.SetValue(buffRound.currentValue - 1);
        if (buffRound.currentValue <= 0)
        {
            buffRound.SetValue(0);
            baseStrength = 1;
        }
    }
}
