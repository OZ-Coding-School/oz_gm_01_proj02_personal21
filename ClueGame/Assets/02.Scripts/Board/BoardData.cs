using UnityEngine;
using ClueGame.Data;

namespace ClueGame.Board
{
    // 타일 타입
    public enum TileType
    {
        Empty,      // 빈 공간
        Hallway,    // 복도 (이동 가능)
        Room,       // 방
        StartPoint  // 시작 지점
    }

    // 타일 클래스
    [System.Serializable]
    public class BoardTile
    {
        public TileType tileType;
        public Vector2Int position;
        public RoomCard? roomType; // 방일 경우 어떤 방인지
        public bool isOccupied;    // 플레이어가 있는지

        public BoardTile(TileType type, Vector2Int pos, RoomCard? room = null)
        {
            tileType = type;
            position = pos;
            roomType = room;
            isOccupied = false;
        }

        public bool IsWalkable()
        {
            return (tileType == TileType.Hallway || tileType == TileType.Room || tileType == TileType.StartPoint)
                   && !isOccupied;
        }
    }
}