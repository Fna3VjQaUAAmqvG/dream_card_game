using System;
[Flags]
public enum RoomType
{
    MinorEnemy=1,
    EliteEnemy=2,
    Shop=4,
    Treasure=8,
    RestRoom=16,
    Boss=32
}
public enum RoomState{
    Locked,
    Visited,
    Attainable
}

public enum CardType
{
    Attack,
    Defense,
    Ability
}
public enum EffectTargetType
{
    Self,
    Target,
    All
}