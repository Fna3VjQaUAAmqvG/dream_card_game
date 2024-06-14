using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct CardTransform
{
    public Vector3 pos;
    public quaternion rotation;
    public CardTransform(Vector3 vector3, quaternion quaternion)
    {
        pos = vector3;
        rotation =quaternion;
    }
}
