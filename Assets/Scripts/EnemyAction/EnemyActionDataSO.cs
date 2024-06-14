using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyActionDataSO", menuName = "Enemy/EnemyActionDataSO", order = 0)]
public class EnemyActionDataSO : ScriptableObject
{
    public List<EnemyAction> actions;
}
[System.Serializable]
public struct EnemyAction{
    public Sprite intendSprite;
    public Effect effect;
}