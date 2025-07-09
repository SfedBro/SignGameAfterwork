using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int roomsX = 4, roomsY = 4;
    public float roomOffsetX, roomOffsetY;

    RoomInfoInstance[,] rooms;

    void Awake() {
        rooms = new RoomInfoInstance[roomsX, roomsY];
        GenerateLevel();
    }

    void Start() {
        LoadLevel();
    }

    void GenerateLevel() {
        List<RoomInfo> roomInfos = Resources.LoadAll<RoomInfo>("RoomInfos").ToList();
        for (int x = 0; x < roomsX; ++x) {
            for (int y = 0; y < roomsY; ++y) {
                RoomDirections possibleRD = GetRoomDirectionsAvailable(x, y);
                List<RoomInfo> possibleRooms = roomInfos.Where(ri => possibleRD.CanFit(ri.rd)).ToList();
                if (possibleRooms.Count == 0) continue;
                RoomInfo ri = possibleRooms.OrderBy(_ => Random.value).First();
                // RoomInfo ri = roomInfos[Random.Range(0, roomInfos.Count - 1)];
                Transform t = new GameObject($"Root {x} {y} {ri.rd}").transform;
                t.position = new(x * roomOffsetX, -y * roomOffsetY);
                rooms[x, y] = new RoomInfoInstance{roomInfo = ri, root = t};
            }
        }
    }

    async void LoadLevel() {
        for (int x = 0; x < roomsX; ++x) {
            for (int y = 0; y < roomsY; ++y) {
                RoomInfoInstance rii = rooms[x, y];
                await LoadSceneObjects(rii.roomInfo.sceneName, rii.root);
            }
        }
    }

    async Task LoadSceneObjects(string sceneName, Transform t) {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        // for (int i = 1; i < SceneManager.loadedSceneCount; ++i) {
        //     scene = SceneManager.GetSceneAt(i);
        //     if (scene.name == sceneName && scene.GetRootGameObjects().Length == 0) return;
        // }
        scene.GetRootGameObjects().ToList().ForEach(go => go.transform.SetParent(t, false));
        // t.gameObject.SetActive(false);
        await SceneManager.UnloadSceneAsync(scene);
    }

    RoomDirections GetRoomDirectionsAvailable(int x, int y) {
        RoomDirections rd = new() {
            up = IsDirectionAvailable(x, y, x, y - 1),
            down = IsDirectionAvailable(x, y, x, y + 1),
            left = IsDirectionAvailable(x, y, x - 1, y),
            right = IsDirectionAvailable(x, y, x + 1, y)
        };
        return rd;
    }

    DirAvailability IsDirectionAvailable(int newX, int newY, int exX, int exY) {
        if (exX < 0 || exX >= roomsX || exY < 0 || exY >= roomsY) return DirAvailability.NotAvailable;
        if (rooms[exX, exY] == null) return DirAvailability.Any;
        if (exX > newX) return rooms[exX, exY].roomInfo.rd.left;
        if (exX < newX) return rooms[exX, exY].roomInfo.rd.right;
        if (exY > newY) return rooms[exX, exY].roomInfo.rd.up;
        if (exY < newY) return rooms[exX, exY].roomInfo.rd.down;
        return DirAvailability.Available;
    }

    public RoomInfoInstance GetRoom(int x, int y) {
        return rooms[x, y];
    }
}
