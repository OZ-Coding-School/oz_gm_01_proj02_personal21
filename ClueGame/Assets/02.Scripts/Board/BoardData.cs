using UnityEngine;
using ClueGame.Data;

namespace ClueGame.Board
{
    public enum TileType
    {
        Empty,
        Hallway,
        Room,
        StartPoint
    }

    public class BoardTile
    {
        public TileType tileType;
        public Vector2Int position;
        public RoomCard? roomType;
        public bool isOccupied;

        public BoardTile(TileType type, Vector2Int pos, RoomCard? room = null)
        {
            tileType = type;
            position = pos;
            roomType = room;
            isOccupied = false;
        }

        public bool IsWalkable()
        {
            return (tileType == TileType.Hallway ||
                    tileType == TileType.Room ||
                    tileType == TileType.StartPoint) &&
                   !isOccupied;
        }

        // SetOccupied 메서드 추가
        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
        }
    }
}