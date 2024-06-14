using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header(header: "组件")]
    public SpriteRenderer cardSprite;
    public TextMeshPro costText, descriptionText, typeText;
    public CardDataSO cardData;
    public bool isAnimating;
    [Header("原始数据")]
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public int originalLayouOrder;
    public bool isAvailable;
    public Player player;
    [Header("广播事件")]
    public ObjectEventSO discardCardEvent;
    public IntEventSO costEvent;
    private void Start()
    {
        Init(cardData);
    }
    public void Init(CardDataSO data)
    {
        cardData = data;
        cardSprite.sprite = data.cardImage;
        costText.text = data.cost.ToString();
        descriptionText.text = data.description;
        typeText.text = data.cardType switch
        {
            CardType.Attack => "Attack",
            CardType.Defense => "Defense",
            CardType.Ability => "Ablility",
            _ => throw new System.NotImplementedException(),
        };
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
    public void UpdatePositionRotaion(Vector3 position, Quaternion rotation)
    {
        originalPosition = position;
        originalRotation = rotation;
        originalLayouOrder = GetComponent<SortingGroup>().sortingOrder;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isAnimating)
            return;
        transform.position = originalPosition + Vector3.up;
        transform.rotation = Quaternion.identity;
        GetComponent<SortingGroup>().sortingOrder = 20;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isAnimating) return;
        ResetCardTransform();
    }
    public void ResetCardTransform()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        GetComponent<SortingGroup>().sortingOrder = originalLayouOrder;
    }
    public void ExecuteCardEffects(CharacterBase from, CharacterBase target)
    {
        //TODO:减少能量，回收卡牌
        costEvent.RaiseEvent(cardData.cost, this);
        discardCardEvent.RaiseEvent(this,this);
        foreach (var effect in cardData.effects)
        {
            effect.Execute(from, target);
        }
    }
    public void UpdateCardState()
    {
        isAvailable = cardData.cost <=player.CurrentMana;
        costText.color = isAvailable ? Color.green:Color.red;
    }
}
