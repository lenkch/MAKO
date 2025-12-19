using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomData : MonoBehaviour
{
    public string roomName;
    public int entryHeight;
    public int exitHeight;
    public RoomDifficulty difficulty;
    public RoomType type;
}

public enum RoomDifficulty { Easy, Medium, Hard }
public enum RoomType { Combat, Platforming, Corridor, Safe }