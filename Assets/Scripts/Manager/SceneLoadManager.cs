using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public FadePanel fadePanel;
    private AssetReference currentScene;
    public AssetReference map;
    public AssetReference menu;
    private Vector2Int currentRoomVector;
    private Room currentRoom;
    [Header(header: "广播")]
    public ObjectEventSO afterRoomLoadedEvent;
    public ObjectEventSO updateRoomEvent;
    private void Awake()
    {
        currentRoomVector = Vector2Int.one * -1;
        LoadMenu();
    }

    public async void OnLoadRoomEvent(object data)
    {
        if (data is Room)
        {
            currentRoom = data as Room;
            var currentData = currentRoom.roomData;
            currentRoomVector = new(currentRoom.column, currentRoom.line);
            currentScene = currentData.sceneToLoad;
        }
        await UnloadSceneTask();
        await LoadSceneTask();
        afterRoomLoadedEvent.RaiseEvent(currentRoom, this);
    }

    private async Awaitable LoadSceneTask() //加载Scene
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        await s.Task;

        if (s.Status == AsyncOperationStatus.Succeeded) //异步加载成功
        {
            fadePanel.FadeOut(0.4f);
            SceneManager.SetActiveScene(s.Result.Scene);
        }
    }

    private async Awaitable UnloadSceneTask() //卸载场景
    {
        fadePanel.FadeIn(0.6f);
        await Awaitable.WaitForSecondsAsync(0.65f);
        if (SceneManager.GetActiveScene().buildIndex != 0) //不能卸载最后一个场景
        {
            await Awaitable.FromAsyncOperation(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()));
        }
    }

    public async void LoadMap()
    {
        currentScene = map;
        if (currentRoomVector != Vector2.one * -1)
        {
            updateRoomEvent.RaiseEvent(currentRoomVector, this);
        }
        await UnloadSceneTask();
        await LoadSceneTask();
    }

    public async void LoadMenu()
    {
        if (currentScene != null)
        {
            await UnloadSceneTask();
        }
        currentScene = menu;
        await LoadSceneTask();
    }
}
