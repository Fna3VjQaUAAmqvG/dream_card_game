using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("地图配置表")]
    public MapConfigSO mapConfig;
    [Header("地图布局")]
    public MapLayoutSO mapLayout;
    [Header("预制体")]
    public Room roomPrefab;
    public LineRenderer linePrefab;
    private float screenHeight;
    private float screenWidth;
    private float columnWidth;
    private Vector3 generatePoint;
    public float border; //距离屏幕边缘的隔断
    private List<Room> rooms = new();
    private List<LineRenderer> lines = new();
    public List<RoomDataSO> roomDataList = new();
    private Dictionary<RoomType, RoomDataSO> roomDataDict = new();



    private void Awake()
    {
        screenHeight = Camera.main.orthographicSize * 2;
        screenWidth = screenHeight * Camera.main.aspect;
        columnWidth = screenWidth / (mapConfig.roomBluePrints.Count + 1);

        foreach (var roomData in roomDataList)
        {
            roomDataDict.Add(roomData.roomType, roomData);
        }
    }

    private void OnEnable()
    {
        if (mapLayout.mapRoomDataList.Count > 0) //有存储数据
            LoadMap();
        else
            CreateMap();
    }

    public void CreateMap()
    {
        List<Room> previousColumnRooms = new();
        for (int column = 0; column < mapConfig.roomBluePrints.Count; column++)
        {
            var blueprint = mapConfig.roomBluePrints[column];
            var amount = Random.Range(blueprint.min, blueprint.max); //该列房间数
            var startHeight = screenHeight / 2 - screenHeight / (amount + 1); //每一列第一个房间从上往下的y坐标

            //每列从上往下第一个房间坐标
            generatePoint = new Vector3(-screenWidth / 2 + border + columnWidth * column, startHeight, z: 0);

            var newPosition = generatePoint;
            List<Room> currentColumnRooms = new();

            var roomGapY = screenHeight / (amount + 1);
            //循环生成列
            for (int i = 0; i < amount; i++)
            {
                if (column == mapConfig.roomBluePrints.Count - 1)
                {
                    newPosition.x = screenWidth / 2 - border * 2;
                }
                else if (column != 0)
                {
                    newPosition.x = generatePoint.x + Random.Range(-border / 2, border / 2);
                }
                newPosition.y = startHeight - roomGapY * i;
                //生成房间
                var room = Instantiate(roomPrefab, newPosition, quaternion.identity, transform);
                RoomType newType = GetRandomRoomType(mapConfig.roomBluePrints[column].roomType); //获得当前列的roomType

                //设置只有第一列可以进入
                if (column == 0)
                {
                    room.roomState = RoomState.Attainable;
                }
                else
                {
                    room.roomState = RoomState.Locked;
                }


                room.SetupRoom(column, i, GetRoomData(newType));
                rooms.Add(room);
                currentColumnRooms.Add(room);
            }

            //双指针连线，
            if (previousColumnRooms.Count > 0)
            {
                CreateConnections(previousColumnRooms, currentColumnRooms);
            }
            previousColumnRooms = currentColumnRooms;
        }
        SaveMap();
    }

    //连线
    private void CreateConnections(List<Room> previousColumnRooms, List<Room> currentColumnRooms)
    {
        HashSet<Room> connetedColumn2Rooms = new();
        foreach (var room in previousColumnRooms) //正向检测连线
        {
            var targetRoom = ConnectToRandomRoom(room, currentColumnRooms, false);
            connetedColumn2Rooms.Add((Room)targetRoom);
        }
        foreach (var room in currentColumnRooms) //反向检测连线
        {
            if (!connetedColumn2Rooms.Contains(room))
            {
                ConnectToRandomRoom(room, previousColumnRooms, true);
            }
        }
    }

    private object ConnectToRandomRoom(Room room, List<Room> currentColumnRooms, bool check)
    {
        Room targetRoom;
        targetRoom = currentColumnRooms[Random.Range(minInclusive: 0, currentColumnRooms.Count)];
        if (check)
        { 
            targetRoom.linkTo.Add(new(room.column, room.line));
        }
        else
        {
            room.linkTo.Add(new(targetRoom.column, targetRoom.line));
        }

        //开始连线
        var line = Instantiate(linePrefab, transform);
        line.SetPosition(index: 0, room.transform.position);
        line.SetPosition(index: 1, targetRoom.transform.position);

        lines.Add(line);
        return targetRoom;
    }

    //重新生成地图
    [ContextMenu(itemName: "ReGenerateRoom")]
    public void ReGenerateRoom()
    {
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }
        foreach (var item in lines)
        {
            Destroy(item.gameObject);
        }
        rooms.Clear();
        lines.Clear();
        CreateMap();
    }

    private RoomDataSO GetRoomData(RoomType roomType) //查找
    {
        return roomDataDict[roomType];
    }

    private RoomType GetRandomRoomType(RoomType flags) //获得随即类型
    {
        string[] option = flags.ToString().Split(separator: ',');
        string randomOption = option[Random.Range(minInclusive: 0, option.Length)];
        RoomType roomType = (RoomType)Enum.Parse(typeof(RoomType), randomOption);
        return roomType;
    }
    private void SaveMap()
    {
        mapLayout.mapRoomDataList = new();
        for (int i = 0; i < rooms.Count; i++)
        {
            var room = new MapRoomData()
            {
                posX = rooms[i].transform.position.x,
                posY = rooms[i].transform.position.y,
                column = rooms[i].column,
                line = rooms[i].line,
                roomData = rooms[i].roomData,
                roomState = rooms[i].roomState,
                linkTo = rooms[i].linkTo
            };
            mapLayout.mapRoomDataList.Add(room);
        }

        mapLayout.linePositionList = new();
        for (int i = 0; i < lines.Count; i++)
        {
            var line = new LinePosition()
            {
                startPos = new SerializeVector3(lines[i].GetPosition(index: 0)),
                endPos = new SerializeVector3(lines[i].GetPosition(index: 1)),
            };
            mapLayout.linePositionList.Add(line);
        }
    }
    private void LoadMap()
    {
        //读取房间
        for (int i = 0; i < mapLayout.mapRoomDataList.Count; i++)
        {
            var newPos = new Vector3(mapLayout.mapRoomDataList[i].posX, mapLayout.mapRoomDataList[i].posY, z: 0);
            var newRoom = Instantiate(roomPrefab, newPos, quaternion.identity, transform);
            newRoom.roomState = mapLayout.mapRoomDataList[i].roomState;
            newRoom.SetupRoom(mapLayout.mapRoomDataList[i].column, mapLayout.mapRoomDataList[i].line, mapLayout.mapRoomDataList[i].roomData);
            newRoom.linkTo = mapLayout.mapRoomDataList[i].linkTo;
            rooms.Add(newRoom);
        }

        //读取连线
        for (int i = 0; i < mapLayout.linePositionList.Count; i++)
        {
            var line = Instantiate(linePrefab, transform);
            line.SetPosition(index: 0, mapLayout.linePositionList[i].startPos.ToVector3());
            line.SetPosition(index: 1, mapLayout.linePositionList[i].endPos.ToVector3());
            lines.Add(line);
        }
    }
}
