using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameWinPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Button pickCardButton;
    private Button backToMapButton;
    [Header("事件广播")]
    public ObjectEventSO loadMapEvent;
    public ObjectEventSO pickCardEvent;

    private void Awake()
    {
        Debug.Log("GameWinPanel Awake");
        //Initialize();
    }

    private void OnEnable()
    {
        Debug.Log("GameWinPanel OnEnable");
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        pickCardButton = rootElement.Q<Button>("PickCardButton");
        backToMapButton = rootElement.Q<Button>("BackToMapButton");
        pickCardButton.clicked += OnPickCardButtonClicked;
        backToMapButton.clicked += OnBackToMapButtonClicked;
    }

    private void OnDisable()
    {
        Debug.Log("GameWinPanel OnDisable");
        pickCardButton.clicked -= OnPickCardButtonClicked;
        backToMapButton.clicked -= OnBackToMapButtonClicked;
    }

    // private void Initialize()
    // {
    //     Debug.Log("GameWinPanel Initialize");
    //     rootElement = GetComponent<UIDocument>().rootVisualElement;
    //     pickCardButton = rootElement.Q<Button>("PickCardButton");
    //     backToMapButton = rootElement.Q<Button>("BackToMapButton");
    // }

    private void OnPickCardButtonClicked()
    {
        Debug.Log("GameWinPanel选牌按钮");
        pickCardEvent.RaiseEvent(null, this);
    }

    private void OnBackToMapButtonClicked()
    {
        Debug.Log("GameWinPanel返回地图按钮");
        loadMapEvent.RaiseEvent(null, this);
    }

    public void OnFinishPickCardEvent()
    {
        Debug.Log("GameWinPanel奖励牌按钮");
        pickCardButton.style.display = DisplayStyle.None;
    }
}



// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UIElements;

// public class GameWinPanel : MonoBehaviour
// {
//     private VisualElement rootElement;
//     private Button pickCardButton;
//     private Button backToMapButton;
//     [Header("事件广播")]
//     public ObjectEventSO loadMapEvent;
//     public ObjectEventSO pickCardEvent;
//     private void Awake()
//     {
//         Debug.Log("GameWinPanel Awake");
//         rootElement = GetComponent<UIDocument>().rootVisualElement;
//         pickCardButton = rootElement.Q<Button>("PickCardButton");
//         backToMapButton = rootElement.Q<Button>("BackToMapButton");
//     }
//     private void OnEnable()
//     {
//         Debug.Log("GameWinPanel OnEnable");
//         pickCardButton.clicked += OnPickCardButtonClicked;
//         backToMapButton.clicked += OnBackToMapButtonClicked;
//     }
//     private void OnDisable()
//     {
//         Debug.Log("GameWinPanel OnDisable");
//         pickCardButton.clicked -= OnPickCardButtonClicked;
//         backToMapButton.clicked -= OnBackToMapButtonClicked;
//     }
//     private void OnPickCardButtonClicked()
//     {
//         Debug.Log("GameWinPanel选牌按钮");
//         pickCardEvent.RaiseEvent(this, this);
//     }

//     private void OnBackToMapButtonClicked()
//     {
//         Debug.Log("GameWinPanel返回地图按钮");
//         loadMapEvent.RaiseEvent(this, this);
//     }
//     public void OnFinishPickCardEvent()
//     {
//         Debug.Log("GameWinPanel奖励牌按钮");
//         pickCardButton.style.display = DisplayStyle.None;
//     }
// }
