using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int hubExitHeight = 1;
    [SerializeField] private int roomsPerLevel = 3;
    [SerializeField] private Grid grid;


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
        Vector3 nextPosition = Vector3.zero;

        // baselineWorldY is the world/local Y coordinate where the current room's entry row
        // should be placed. Initialize from `hubExitHeight` (in tiles) using the project's
        // Grid cell size when available so the first room starts at the expected row.
        float baselineWorldY = 0f;
        if (grid != null)
            baselineWorldY = hubExitHeight * grid.cellSize.y;

        foreach (RoomData room in selectedRooms)
        {
            GameObject roomGO = Instantiate(room.gameObject, transform);
            Tilemap tilemap = roomGO.GetComponentInChildren<Tilemap>();
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;

            // cell size (world units per tile)
            Vector3 cellSize = tilemap.layoutGrid.cellSize;

            // Place X so the leftmost tile of the tilemap aligns with nextPosition.x
            float placeX = nextPosition.x - bounds.xMin * cellSize.x;

            // Place Y so the entry row (measured from bottom) matches baselineWorldY.
            // Account for the tilemap's cellBounds.yMin because the tilemap origin
            // inside the prefab may not start at 0.
            float entryCellIndex = bounds.yMin + room.entryHeight;
            float exitCellIndex = bounds.yMin + room.exitHeight;
            float placeY = baselineWorldY - entryCellIndex * cellSize.y;

            roomGO.transform.localPosition = new Vector3(placeX, placeY, 0f);

            // Update baselineWorldY to the world Y of the exit row for the next room
            baselineWorldY = placeY + exitCellIndex * cellSize.y;

            // Advance nextPosition by the room width in world units
            nextPosition.x += bounds.size.x * cellSize.x;
        }
        
    }
}