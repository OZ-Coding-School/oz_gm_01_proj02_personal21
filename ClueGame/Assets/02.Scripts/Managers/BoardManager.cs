using UnityEngine;
using ClueGame.Board;
using ClueGame.Data;
using System.Collections.Generic;

namespace ClueGame.Managers
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance { get; private set; }

        [Header("Board Settings")]
        [SerializeField] private int boardWidth = 24;
        [SerializeField] private int boardHeight = 24;

        private BoardTile[,] board;

        // ===== 비밀 통로 추가 =====
        private Dictionary<RoomCard, RoomCard> secretPassages = new Dictionary<RoomCard, RoomCard>
        {
            { RoomCard.서재, RoomCard.부엌 },
            { RoomCard.부엌, RoomCard.서재 },
            { RoomCard.온실, RoomCard.라운지 },
            { RoomCard.라운지, RoomCard.온실 }
        };

        // 방의 중심 위치 저장
        private Dictionary<RoomCard, Vector2Int> roomCenters = new Dictionary<RoomCard, Vector2Int>();
        // ========================

        private List<Vector2Int> startPositions = new List<Vector2Int>
        {
            new Vector2Int(12, 12),  // 중앙
            new Vector2Int(12, 12),  // 중앙
            new Vector2Int(12, 12),  // 중앙
            new Vector2Int(12, 12),  // 중앙
            new Vector2Int(12, 12),  // 중앙
            new Vector2Int(12, 12)   // 중앙
        };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            board = new BoardTile[boardWidth, boardHeight];

            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    board[x, y] = new BoardTile(TileType.Hallway, new Vector2Int(x, y));
                }
            }

            for (int i = 0; i < startPositions.Count; i++)
            {
                Vector2Int pos = startPositions[i];
                board[pos.x, pos.y] = new BoardTile(TileType.StartPoint, pos);
            }

            CreateRoom(RoomCard.서재, 1, 1, 5, 6);
            CreateRoom(RoomCard.도서관, 1, 8, 6, 6);
            CreateRoom(RoomCard.당구장, 1, 18, 6, 5);
            CreateRoom(RoomCard.온실, 9, 19, 6, 4);
            CreateRoom(RoomCard.무도회장, 8, 9, 7, 8);
            CreateRoom(RoomCard.홀, 9, 1, 6, 6);
            CreateRoom(RoomCard.라운지, 18, 1, 5, 6);
            CreateRoom(RoomCard.식당, 17, 9, 7, 7);
            CreateRoom(RoomCard.부엌, 18, 18, 5, 5);

            Debug.Log($"{boardWidth}x{boardHeight} 보드가 생성되었습니다.");
            Debug.Log($"비밀 통로: 서재↔부엌, 온실↔라운지");
        }

        private void CreateRoom(RoomCard roomType, int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight)
                    {
                        board[x, y] = new BoardTile(TileType.Room, new Vector2Int(x, y), roomType);
                    }
                }
            }

            // ===== 방 중심 위치 저장 =====
            Vector2Int center = new Vector2Int(startX + width / 2, startY + height / 2);
            roomCenters[roomType] = center;
            // ===========================
        }

        public Vector2Int GetStartPosition(int playerIndex)
        {
            if (playerIndex < startPositions.Count)
            {
                return startPositions[playerIndex];
            }
            return Vector2Int.zero;
        }

        public BoardTile GetTile(Vector2Int position)
        {
            if (position.x >= 0 && position.x < boardWidth &&
                position.y >= 0 && position.y < boardHeight)
            {
                return board[position.x, position.y];
            }
            return null;
        }

        public bool CanMoveTo(Vector2Int position)
        {
            BoardTile tile = GetTile(position);
            return tile != null && tile.IsWalkable();
        }

        public List<Vector2Int> GetWalkableNeighbors(Vector2Int position)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            Vector2Int[] directions = {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in directions)
            {
                Vector2Int newPos = position + dir;
                if (CanMoveTo(newPos))
                {
                    neighbors.Add(newPos);
                }
            }

            return neighbors;
        }

        public RoomCard? GetRoomAtPosition(Vector2Int position)
        {
            BoardTile tile = GetTile(position);
            if (tile != null && tile.tileType == TileType.Room)
            {
                return tile.roomType;
            }
            return null;
        }

        public Vector2Int GetBoardSize()
        {
            return new Vector2Int(boardWidth, boardHeight);
        }

        // ===== 비밀 통로 관련 메서드 추가 =====

        // 비밀 통로가 있는지 확인
        public bool HasSecretPassage(RoomCard fromRoom)
        {
            return secretPassages.ContainsKey(fromRoom);
        }

        // 비밀 통로 목적지 반환
        public RoomCard? GetSecretPassageDestination(RoomCard fromRoom)
        {
            if (secretPassages.TryGetValue(fromRoom, out RoomCard destination))
            {
                return destination;
            }
            return null;
        }

        // 방의 중심 위치 반환
        public Vector2Int GetRoomCenter(RoomCard room)
        {
            if (roomCenters.TryGetValue(room, out Vector2Int center))
            {
                return center;
            }
            return Vector2Int.zero;
        }

        // =====================================
    }
}