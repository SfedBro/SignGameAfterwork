using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomInfo", menuName = "Scriptable Objects/RoomInfo")]
public class RoomInfo : ScriptableObject
{
    public string sceneName;
    public bool up;
    public bool down;
    public bool left;
    public bool right;
}

public class RoomInfoInstance
{
    public RoomInfo roomInfo;
    public bool visited = false;
    public bool cleared = false;
    public Transform root;
}
