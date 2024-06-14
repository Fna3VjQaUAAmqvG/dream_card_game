using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System.Diagnostics.Tracing;
using System.Collections.ObjectModel;
public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;
    public CardlayoutManager layoutManager;
    public Vector3 deckPosition;

    [SerializeField]
    private List<CardDataSO> drawDeck = new(); //draw牌堆
    private List<CardDataSO> discardDeck = new(); //弃牌堆
    private List<Card> handCardObjectList = new(); //每回合
    [Header("事件广播")]
    public IntEventSO drawCountEvent;
    public IntEventSO discardCountEvent;
    private void Start()
    {
        InitializeDeck();
    }
    public void InitializeDeck() //理应在新开始游戏时调用
    {
        drawDeck.Clear();
        foreach (var entry in cardManager.currentCardLibrary.cardLibraryList)
        {
            for (int i = 0; i < entry.amount; i++) //多张同类型卡
            {
                drawDeck.Add(entry.cardData);
            }
        }
        ShuffleDeck();
    }
    [ContextMenu("测试抽2牌")]
    public void TestDrawCard() //Test
    {
        DrawCard(2);
    }
    public void NewTurnDrawCard()
    {
        DrawCard(4);
    }
    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawDeck.Count == 0) //draw牌堆为空
            {
                Debug.Log("无牌可抽");
                if(discardDeck.Count==0) //牌库和弃牌堆都没牌，牌全在手上
                {
                    Debug.Log("牌全在手上");
                    return;
                }

                //洗牌
                foreach (var item in discardDeck)
                {
                    drawDeck.Add(item);
                }
                ShuffleDeck();
            }
            var currentCardData = drawDeck[0]; //从抽牌堆顶抽一张
            drawDeck.RemoveAt(0);

            //更新UI数字
            drawCountEvent.RaiseEvent(drawDeck.Count, this); //drawCountEvent?.Invoke(drawDeck.Count)

            //初始化
            var card = cardManager.GetCardObject().GetComponent<Card>(); //对象池取一个卡
            card.Init(currentCardData); //给卡赋值
            handCardObjectList.Add(card); //入List
            card.transform.position = deckPosition; //卡放画面中间
            var delay = i * 0.15f;
            SetCardLayout(delay); //卡放手牌里

        }
    }
    private void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];
            CardTransform cardTransform = layoutManager.GetCardTransform(i, handCardObjectList.Count);

            currentCard.UpdateCardState();
            currentCard.isAnimating = true;

            currentCard.transform.DOScale(Vector3.one, 0.5f).SetDelay(delay).onComplete = () => //DOScale变换，111比例，0.5事件，再设置delay延迟，延迟complete后调用下面
            {
                currentCard.transform.DOMove(cardTransform.pos, 0.2f).onComplete = () =>
                currentCard.isAnimating = false;
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);

            };

            currentCard.GetComponent<SortingGroup>().sortingOrder = i; //牌layer
            currentCard.UpdatePositionRotaion(cardTransform.pos, cardTransform.rotation);
        }
    }

    public void ShuffleDeck() //洗牌，随机数生成后，使每张牌与第随机数张牌进行交换
    {
        discardDeck.Clear();
        //TODO:更新UI显示数量
        drawCountEvent.RaiseEvent(drawDeck.Count,this);
        discardCountEvent.RaiseEvent(discardDeck.Count,this);
        for (int i = 0; i < drawDeck.Count; i++)
        {
            CardDataSO temp = drawDeck[i];
            int randomIndex = Random.Range(i, drawDeck.Count);
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }
    }

    public void DiscardCard(object obj) //弃牌，事件函数
    {
        Card card = obj as Card;
        discardDeck.Add(card.cardData);
        handCardObjectList.Remove(card);
        cardManager.DiscardCard(card.gameObject);
        //弃牌
        discardCountEvent.RaiseEvent(discardDeck.Count,this);
        SetCardLayout(0f);
    }
    public void OnPlayerTurnEnd()
    {
        for(int i=0;i<handCardObjectList.Count;i++)
        {
            discardDeck.Add(handCardObjectList[i].cardData);
            cardManager.DiscardCard(handCardObjectList[i].gameObject);
        }
        handCardObjectList.Clear();
        discardCountEvent.RaiseEvent(discardDeck.Count,this);
    }
    public void ReleaseAllCards(object obj)
    {
        foreach(var card in handCardObjectList)
        {
            cardManager.DiscardCard(card.gameObject);
        }
        handCardObjectList.Clear();
        InitializeDeck();
    }
}
