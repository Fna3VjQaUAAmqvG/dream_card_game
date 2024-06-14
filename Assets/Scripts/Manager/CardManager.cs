using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;
    public List<CardDataSO> cardDataList;
    [Header(header: "卡牌库")]
    public CardLibrarySO newGameCardLibrary;
    public CardLibrarySO currentCardLibrary;
    private int previousIndex;
    private void Awake()
    {
        InitializeCardDataList();
        foreach (var item in newGameCardLibrary.cardLibraryList)
        {
            currentCardLibrary.cardLibraryList.Add(item);
        }
    }

    private void OnDisable() //清除手牌堆
    {
        currentCardLibrary.cardLibraryList.Clear();
    }
    private void InitializeCardDataList()
    {
        Addressables.LoadAssetsAsync<CardDataSO>("CardData", null).Completed += OncardDataLoaded; //加载完才将牌入库
    }

    private void OncardDataLoaded(AsyncOperationHandle<IList<CardDataSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded) //所有数据都找到
        {
            cardDataList = new List<CardDataSO>(handle.Result);
        }
        else //fail safe
        {
            Debug.LogError(message: "Fail safe: Load Card addressable assets failed");
        }
    }

    public GameObject GetCardObject() //从池中获取
    {
        return poolTool.GetObjectFromPool();
    }
    public void DiscardCard(GameObject cardObject) //放回池子
    {
        poolTool.ReturnObjectToPool(cardObject);
    }

    public CardDataSO GetNewCardData()
    {
        var randomIndex = 0;
        do //两张相邻一定不一样
        {
            randomIndex = UnityEngine.Random.Range(0, cardDataList.Count);
        } while (previousIndex == randomIndex);

        previousIndex = randomIndex;
        return cardDataList[randomIndex];
    }
    
    public void UnlockCard(CardDataSO newCardData)
    {
        var newCard = new CardLibraryEntry
        {
            cardData = newCardData,
            amount = 1,
        };
        if (currentCardLibrary.cardLibraryList.Contains(newCard)) //已有的牌则直接数量++
        {
            var target = currentCardLibrary.cardLibraryList.Find(t => t.cardData == newCardData);
            target.amount++;
        }
        else
        {
            currentCardLibrary.cardLibraryList.Add(newCard);
        }
    }
}
