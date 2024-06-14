using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("面板")]
    public GameObject gameplayPanel;
    public GameObject gameWinPanel;
    public GameObject gameOverPanel;
    public GameObject pickCardPanel;
    public GameObject restRoomPanel;

    // public void 
    public void OnLoadRoomEvent(object data)
    {
        Room currentRoom = (Room)data;
        switch(currentRoom.roomData.roomType)
        {
            case RoomType.MinorEnemy:
                gameplayPanel.SetActive(true);
                break;
            case RoomType.EliteEnemy:
                gameplayPanel.SetActive(true);
                break;
            case RoomType.Boss:
                gameplayPanel.SetActive(true);
                break;
            case RoomType.Shop:
                gameplayPanel.SetActive(false);
                break;
            case RoomType.Treasure:
                gameplayPanel.SetActive(false);
                break;
            case RoomType.RestRoom:
                restRoomPanel.SetActive(false);
                break;

        }
    }
    public void HideAllPanels()
    {
        gameplayPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pickCardPanel.SetActive(false);
        restRoomPanel.SetActive(false);
    }

    public void OnGameWinEvent()
    {
        gameplayPanel.SetActive(false);
        gameWinPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pickCardPanel.SetActive(false);
        restRoomPanel.SetActive(false);
    }
    public void OnGameOverEvent()
    {
        gameplayPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        pickCardPanel.SetActive(false);
        restRoomPanel.SetActive(false);
    }
    public void OnPickCardEvent()
    {
        gameplayPanel.SetActive(false);
        gameWinPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pickCardPanel.SetActive(true);
        restRoomPanel.SetActive(false);
    }
    public void OnFinishPickCardEvent()
    {
        gameplayPanel.SetActive(false);
        gameWinPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pickCardPanel.SetActive(false);
        restRoomPanel.SetActive(false);
    }
}
