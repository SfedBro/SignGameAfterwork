using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int roomsX = 4, roomsY = 4;
    public float roomOffsetX, roomOffsetY;

    RoomInfoInstance[,] rooms;

    void Start() {
        rooms = new RoomInfoInstance[roomsX, roomsY];
        GenerateMap();
    }

    void GenerateMap() {
        List<RoomInfo> roomInfos = Resources.LoadAll<RoomInfo>("RoomInfos").ToList();
        for (int x = 0; x < roomsX; ++x) {
            for (int y = 0; y < roomsY; ++y) {
                RoomInfo ri = roomInfos[Random.Range(0, roomInfos.Count - 1)];
                Transform t = new GameObject($"Root {x} {y}").transform;
                t.position = new(x * roomOffsetX, -y * roomOffsetY);
                rooms[x, y] = new RoomInfoInstance{roomInfo = ri, root = t};
                LoadSceneObjects(ri.sceneName, t);
            }
        }
    }

    async void LoadSceneObjects(string sceneName, Transform t) {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        scene.GetRootGameObjects().ToList().ForEach(go => go.transform.SetParent(t, false));
        t.gameObject.SetActive(false);
        await SceneManager.UnloadSceneAsync(scene);
    }
}
