using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class PickCardPanel : MonoBehaviour
{
    public CardManager cardManager;
    public VisualTreeAsset cardTemplate;
    private VisualElement rootElement;
    private VisualElement cardContainer;
    private CardDataSO currentCardData;
    private List<Button> cardsToPick = new();
    private Button confirmButton;
    [Header("广播事件")]
    public ObjectEventSO finishPickCardEvent;
    private void Awake()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        cardContainer = rootElement.Q<VisualElement>("Container");
        confirmButton = rootElement.Q<Button>("ConfirmButton");
        confirmButton.clicked += OnConfirmButtonClicked;

        for (int i = 0; i < 3; i++)
        {
            var card = cardTemplate.Instantiate();
            var data = cardManager.GetNewCardData();
            InitCard(card, data);
            var cardOnDesk = card.Q<Button>("Card");
            cardContainer.Add(card);
            cardsToPick.Add(cardOnDesk);
            cardOnDesk.clicked += () => OnCardClicked(cardOnDesk, data);
        }
    }
    private void OnDisable() {
        confirmButton.clicked -= OnConfirmButtonClicked;
        
    }

    private void OnConfirmButtonClicked()
    {
        cardManager.UnlockCard(currentCardData);
        finishPickCardEvent.RaiseEvent(this,this);
    }
    private void OnCardClicked(Button cardButton, CardDataSO data)
    {
        currentCardData = data;
        for (int i = 0; i < cardButton.childCount; i++)
        {
            if (cardsToPick[i] == cardButton)
                cardsToPick[i].SetEnabled(true);
            else
                cardsToPick[i].SetEnabled(false);
        }

    }

    public void InitCard(VisualElement card, CardDataSO cardData)
    {

        card.dataSource = cardData;
        var cardSpriteElement = card.Q<VisualElement>("CardSprite");
        var cardCost = card.Q<Label>("EnergyCost");
        var cardDescription = card.Q<Label>("CardDescription");
        var cardType = card.Q<Label>("CardType");
        var cardName = card.Q<Label>("CardName");

        cardSpriteElement.style.backgroundImage = new StyleBackground(cardData.cardImage);
        cardName.text = cardData.cardName;
        cardCost.text = cardData.cost.ToString();
        cardDescription.text = cardData.description;
        cardType.text = cardData.cardType switch
        {
            CardType.Attack => "攻击",
            CardType.Defense => "防御",
            CardType.Ability => "能力",
            _ => throw new System.NotImplementedException()
        };
    }
}
