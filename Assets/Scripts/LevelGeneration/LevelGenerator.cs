using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int hubExitHeight = 1;
    [SerializeField] private int roomsPerLevel = 3;

    [SerializeField] private List<RoomData> rooms = new List<RoomData>();

    private List<RoomData> selectedRooms = new List<RoomData>();

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        selectedRooms.Clear();
        int currentHeight = hubExitHeight;

        for (int i = 0; i < roomsPerLevel; i++)
    {
        List<RoomData> matchingRooms = rooms.FindAll(room => room.entryHeight == currentHeight);
        if (matchingRooms.Count > 0)
            {
                RoomData pickedRoom = matchingRooms[Random.Range(0, matchingRooms.Count)];
                selectedRooms.Add(pickedRoom);
                currentHeight = pickedRoom.exitHeight;
            }
        // to resolve if matchingRooms is empty
    }
        Vector3 currentPosition = Vector3.zero;

        foreach (RoomData room in selectedRooms)
        {
            
            Instantiate(room.gameObject, currentPosition, Quaternion.identity, transform);
            Tilemap tilemap = room.GetComponentInChildren<Tilemap>();
            Debug.Log($"Room {room.roomName}: Tilemap = {tilemap}, tile count = {tilemap.GetUsedTilesCount()}");
            BoundsInt bounds = tilemap.cellBounds;
            Debug.Log($"Bounds: {bounds}");
            currentPosition.x += bounds.size.x;

        }
        
    }
}