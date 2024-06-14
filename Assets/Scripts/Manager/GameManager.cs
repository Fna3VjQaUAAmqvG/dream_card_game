using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("地图布局")]
    public MapLayoutSO mapLayout;
    public List<Enemy> aliveEnemyList = new();
    [Header("事件广播")]
    public ObjectEventSO gameWinEvent;
    public ObjectEventSO gameOverEvent;

    //更新房间的事件监听函数
    public void UpdateMapLayOutData(object value)
    {
        var roomVector = (Vector2Int)value; //vector2Int
        if(mapLayout.mapRoomDataList.Count==0)
        {
            return;
        }
        //找到当前房间
        var currentRoom = mapLayout.mapRoomDataList.Find(r => r.column == roomVector.x && r.line == roomVector.y);
        currentRoom.roomState = RoomState.Visited;
        //更新相邻房间的数据
        var sameColumnRoom = mapLayout.mapRoomDataList.FindAll(r => r.column == currentRoom.column);

        foreach (var room in sameColumnRoom)
        {

            if (room.line != roomVector.y) //锁住同一列中不同行房间
            {
                room.roomState = RoomState.Locked;
            }
        }
        foreach (var link in currentRoom.linkTo) //解锁linkTo的所有房间
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(r => r.column == link.x && r.line == link.y);
            linkedRoom.roomState = RoomState.Attainable;
        }
        aliveEnemyList.Clear();
    }
    public void OnRoomLoadedEvent(object obj)
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            aliveEnemyList.Add(enemy);
        }
    }
    public void OnCharacterDeadEvent(object character)
    {
        if (character is Player)
        {
            StartCoroutine(EventDelayAction(gameOverEvent));
        }
        else if (character is Boss)
        {
            StartCoroutine(EventDelayAction(gameOverEvent));
        }
        else if (character is Enemy)
        {
            aliveEnemyList.Remove(character as Enemy);
            if (aliveEnemyList.Count == 0)
            {
                //抽卡
                StartCoroutine(EventDelayAction(gameWinEvent));
            }
        }
    }
    IEnumerator EventDelayAction(ObjectEventSO eventSO)
    {
        yield return new WaitForSeconds(1.5f);
        eventSO.RaiseEvent(null, this);
    }
    public void OnNewGameEvent()
    {
        mapLayout.mapRoomDataList.Clear();
        mapLayout.linePositionList.Clear();
    }
}
