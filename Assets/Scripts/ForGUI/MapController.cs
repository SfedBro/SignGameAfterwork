using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public GameObject map;
    public GameObject mapPointer;
    public Vector2 mapPointerOffest = new(0, -50);
    public List<MapLevelInfo> mapLevelInfos;

    void Start() {
        map.SetActive(false);
        if (mapPointer)
        {
            mapPointer.SetActive(false);
        }
    }

    public void SetMapActive(bool active) {
        map.SetActive(active);
        if (active) SetMapPointerPosition();
        else HideMapPointer();
    }

    public void ChangeMapActive() {
        SetMapActive(!map.activeSelf);
    }
    
    void HideMapPointer() {
        mapPointer.SetActive(false);
    }

    void SetMapPointerPosition() {
        if (!HasSceneInLevelInfos()) HideMapPointer();
        else {
            mapPointer.SetActive(true);
            mapPointer.transform.position = GetCurrentMapLevelInfo().mapTransform.position + (Vector3)mapPointerOffest;
        }
    }

    bool HasSceneInLevelInfos() {
        return mapLevelInfos.Any(mli => mli.sceneName == SceneManager.GetActiveScene().name);
    }

    MapLevelInfo GetCurrentMapLevelInfo() {
        return mapLevelInfos.First(mli => mli.sceneName == SceneManager.GetActiveScene().name);
    }
}

[Serializable]
public struct MapLevelInfo {
    public string sceneName;
    public Transform mapTransform;
}
