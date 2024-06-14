using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class HealthBarController : MonoBehaviour
{
    private CharacterBase currentCharacter;
    [Header("Elements")]
    public Transform healthBarTransform;
    private UIDocument healthBarDocument;
    private ProgressBar healthBar;
    private VisualElement defenseElement;
    private Label defenseAmountLabel;
    private VisualElement buffElement;
    private Label buffRound;
    [Header("buff sprite")]
    public Sprite buffSprite;
    public Sprite debuffSprite;
    private Enemy enemy;
    private VisualElement intendSprite;
    private Label intendAmount;
    private void Awake()
    {
        currentCharacter = GetComponent<CharacterBase>();
        enemy=GetComponent<Enemy>();
        //InitHealthBar();
    }
    private void OnEnable()
    {
        InitHealthBar();
    }
    private void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 size)
    {
        Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, size, Camera.main);
        element.transform.position = rect.position;
    }
    [ContextMenu("Get UI Position")]
    public void InitHealthBar()
    {
        healthBarDocument = GetComponent<UIDocument>();
        healthBar = healthBarDocument.rootVisualElement.Q<ProgressBar>("HealthBar");
        healthBar.highValue = currentCharacter.maxHP;//进度条最大值
        MoveToWorldPosition(healthBar, healthBarTransform.position, Vector2.zero);
        
        defenseElement = healthBar.Q<VisualElement>("Defense");
        defenseAmountLabel = defenseElement.Q<Label>("DefenseAmount");
        defenseElement.style.display = DisplayStyle.None;

        buffElement = healthBar.Q<VisualElement>("Buff");
        buffRound = buffElement.Q<Label>("BuffRound");
        buffElement.style.display = DisplayStyle.None;

        intendSprite=healthBar.Q<VisualElement>("Intend");
        intendAmount=healthBar.Q<Label>("IntendAmount");
        intendSprite.style.display = DisplayStyle.None;
    }
    private void Update()
    {
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        if (currentCharacter.isDead)
        {
            healthBar.style.display = DisplayStyle.None;
            return;
        }
        if (healthBar != null)
        {
            healthBar.title = $"{currentCharacter.currentHP}/{currentCharacter.maxHP}";
            healthBar.value = currentCharacter.currentHP;
            healthBar.RemoveFromClassList("highHealth");
            healthBar.RemoveFromClassList("mediumHealth");
            healthBar.RemoveFromClassList("lowHealth");

            var percentage = (float)currentCharacter.currentHP / (float)currentCharacter.maxHP;
            if (percentage < 0.3f)
            {
                healthBar.AddToClassList("lowHealth");
            }
            else if (percentage < 0.6f)
            {
                healthBar.AddToClassList("mediumHealth");
            }
            else
            {
                healthBar.AddToClassList("highHealth");
            }

            //更新防御
            defenseElement.style.display = currentCharacter.defense.currentValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            defenseAmountLabel.text = currentCharacter.defense.currentValue.ToString();

            buffElement.style.display = currentCharacter.buffRound.currentValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            buffRound.text = currentCharacter.buffRound.currentValue.ToString();
            buffElement.style.backgroundImage = currentCharacter.baseStrength > 1 ? new StyleBackground(buffSprite) : new StyleBackground(debuffSprite);
        }
        else{
            Debug.LogError("null healthBar");
        }
    }

    //玩家回合开始
    public void SetIntendElement()
    {
        intendSprite.style.display = DisplayStyle.Flex;
        intendSprite.style.backgroundImage = new StyleBackground(enemy.currentAction.intendSprite);

        var value = enemy.currentAction.effect.value;
        if(enemy.currentAction.effect.GetType()==typeof(DamageEffect))
        {
            value=(int)math.round(enemy.currentAction.effect.value * enemy.baseStrength);
        }
        intendAmount.text=value.ToString();
    }
    //敌人回合结束之后
    public void HideIntendElement()
    {
        intendSprite.style.display = DisplayStyle.None;
    }
}
