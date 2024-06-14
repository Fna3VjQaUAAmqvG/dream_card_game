using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventSO<T> : ScriptableObject
{
    [TextArea]
    public string description; //添加事件描述
    public UnityAction<T> OnEventRaised;
    public string lastSender;
    public void RaiseEvent(T value, object sender) //前为参数，后为发起者注释
    {
        OnEventRaised?.Invoke(value);
        lastSender = sender.ToString();
    }
}
