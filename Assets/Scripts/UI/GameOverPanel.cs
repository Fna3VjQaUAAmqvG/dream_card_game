using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverPanel : MonoBehaviour
{
    private Button backToStartButton;
    public ObjectEventSO loadMenuEvent;
    private void OnEnable() {
        GetComponent<UIDocument>().rootVisualElement.Q<Button>("BackToStartButton").clicked += BackToStart;

    }

    private void BackToStart()
    {
        loadMenuEvent.RaiseEvent(null,this);
    }
}
