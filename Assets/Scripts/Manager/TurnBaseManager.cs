using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBaseManager : MonoBehaviour
{
    public GameObject playerObj;
    private bool isPlayerTurn = false;
    private bool isEnemyTurn = false;
    public bool battleEnd = true;
    private float timeCounter;
    public float enemyTurnDuration;
    public float playerTurnDuration;
    [Header("事件广播")]
    public ObjectEventSO playerTurnBegin;
    public ObjectEventSO enemyTurnBegin;
    public ObjectEventSO enemyTurnEnd;
    private void Update()
    {
        if (battleEnd)
        {
            return;
        }
        if (isEnemyTurn) //回合计时
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= enemyTurnDuration)
            {
                timeCounter = 0f;
                EnemyTurnEnd();
                isPlayerTurn = true;
            }
        }
        if (isPlayerTurn) //回合计时
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= playerTurnDuration)
            {
                timeCounter = 0f;
                PlayerTurnBegin();
                isPlayerTurn = false;
            }
        }
    }

    [ContextMenu("Game Start")]
    public void GameStart()
    {
        isPlayerTurn = true;
        isEnemyTurn = false;
        battleEnd = false;
        timeCounter = 0;
    }

    public void NewGame()
    {
        playerObj.GetComponent<Player>().NewGame();
    }

    public void PlayerTurnBegin() //调用
    {
        playerTurnBegin.RaiseEvent(null, this);
    }

    public void EnemyTurnBegin() //调用2
    {
        isEnemyTurn = true;
        enemyTurnBegin.RaiseEvent(null, this);
    }

    public void EnemyTurnEnd() //调用
    {
        isEnemyTurn = false;
        enemyTurnEnd.RaiseEvent(null, this);
    }

    public void OnRoomLoadEvent(object obj)
    {
        Room room = obj as Room;
        switch (room.roomData.roomType)
        {
            case RoomType.MinorEnemy:
                playerObj.SetActive(true);
                GameStart();
                break;
            case RoomType.EliteEnemy:
                playerObj.SetActive(true);
                GameStart();
                break;
            case RoomType.Boss:
                playerObj.SetActive(true);
                GameStart();
                break;
            case RoomType.Shop:
                battleEnd = true;
                playerObj.SetActive(false);
                break;
            case RoomType.Treasure:
                battleEnd = true;
                playerObj.SetActive(false);
                break;
            case RoomType.RestRoom:
                battleEnd = true;
                playerObj.SetActive(true);
                playerObj.GetComponent<PlayerAnimation>().SetSLeepAnimation();
                break;
        }
        //GameStart();
    }

    public void StopTurnBaseSystem(object obj)
    {
        battleEnd = true;
        playerObj.SetActive(false);
    }

}
