using ClueGame.Board;
using ClueGame.Data;
using ClueGame.Player;
using System.Collections.Generic;
using UnityEngine;

namespace ClueGame.Managers
{
    public class MovementManager : MonoBehaviour
    {
        public static MovementManager Instance { get; private set; }

        public delegate void PlayerMovedHandler(PlayerData player, Vector2Int newPosition);
        public event PlayerMovedHandler OnPlayerMoved;

        public delegate void PlayerEnteredRoomHandler(PlayerData player, RoomCard room);
        public event PlayerEnteredRoomHandler OnPlayerEnteredRoom;

        // 하이라이트 색상
        [Header("Highlight Settings")]
        [SerializeField] private Color moveHighlightColor = new Color(0.5f, 1f, 0.5f, 0.5f); // 연한 초록
        [SerializeField] private Color roomHighlightColor = new Color(1f, 0.5f, 0.5f, 0.5f); // 연한 빨강

        private HashSet<Vector2Int> currentHighlightedTiles = new HashSet<Vector2Int>();

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

        // 이동 가능한 위치 계산 및 하이라이트
        public List<Vector2Int> CalculateReachableTiles(Vector2Int startPosition, int moves)
        {
            List<Vector2Int> reachableTiles = new List<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<(Vector2Int position, int remainingMoves)> queue = new Queue<(Vector2Int, int)>();

            queue.Enqueue((startPosition, moves));
            visited.Add(startPosition);

            while (queue.Count > 0)
            {
                var (currentPos, remainingMoves) = queue.Dequeue();

                if (remainingMoves > 0)
                {
                    List<Vector2Int> neighbors = BoardManager.Instance.GetWalkableNeighbors(currentPos);

                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            reachableTiles.Add(neighbor);
                            queue.Enqueue((neighbor, remainingMoves - 1));
                        }
                    }
                }
            }

            // 하이라이트 표시
            HighlightTiles(reachableTiles);

            return reachableTiles;
        }

        // 타일 하이라이트
        private void HighlightTiles(List<Vector2Int> tiles)
        {
            // 기존 하이라이트 제거
            ClearHighlights();

            if (BoardVisualizer.Instance == null) return;

            foreach (var tile in tiles)
            {
                // 방인지 확인
                RoomCard? room = BoardManager.Instance.GetRoomAtPosition(tile);
                Color highlightColor = room.HasValue ? roomHighlightColor : moveHighlightColor;

                BoardVisualizer.Instance.HighlightTile(tile, highlightColor);
                currentHighlightedTiles.Add(tile);
            }
        }

        // 하이라이트 제거
        public void ClearHighlights()
        {
            if (BoardVisualizer.Instance == null) return;

            foreach (var tile in currentHighlightedTiles)
            {
                // 원래 색으로 복원
                BoardTile boardTile = BoardManager.Instance.GetTile(tile);
                if (boardTile != null)
                {
                    Color originalColor = boardTile.tileType switch
                    {
                        TileType.Hallway => new Color(0.9f, 0.9f, 0.9f),
                        TileType.Room => GetRoomColor(boardTile.roomType),
                        TileType.StartPoint => new Color(1f, 0.7f, 0.7f),
                        _ => Color.white
                    };

                    BoardVisualizer.Instance.HighlightTile(tile, originalColor);
                }
            }

            currentHighlightedTiles.Clear();
        }

        private Color GetRoomColor(RoomCard? roomType)
        {
            if (!roomType.HasValue) return new Color(0.7f, 0.8f, 1f);

            return roomType.Value switch
            {
                RoomCard.서재 => new Color(0.8f, 0.6f, 0.6f),
                RoomCard.도서관 => new Color(0.6f, 0.8f, 0.6f),
                RoomCard.당구장 => new Color(0.6f, 0.6f, 0.8f),
                RoomCard.온실 => new Color(0.6f, 0.9f, 0.6f),
                RoomCard.무도회장 => new Color(0.9f, 0.9f, 0.6f),
                RoomCard.홀 => new Color(0.9f, 0.7f, 0.5f),
                RoomCard.라운지 => new Color(0.8f, 0.5f, 0.7f),
                RoomCard.식당 => new Color(0.7f, 0.8f, 0.9f),
                RoomCard.부엌 => new Color(0.9f, 0.8f, 0.7f),
                _ => new Color(0.7f, 0.8f, 1f)
            };
        }

        // 기존 메서드들...
        public bool MovePlayer(PlayerData player, Vector2Int targetPosition)
        {
            if (!BoardManager.Instance.CanMoveTo(targetPosition))
            {
                return false;
            }

            BoardTile oldTile = BoardManager.Instance.GetTile(player.currentPosition);
            if (oldTile != null)
            {
                oldTile.SetOccupied(false);
            }

            player.currentPosition = targetPosition;

            BoardTile newTile = BoardManager.Instance.GetTile(targetPosition);
            if (newTile != null)
            {
                newTile.SetOccupied(true);
            }

            // 방 확인
            RoomCard? room = BoardManager.Instance.GetRoomAtPosition(targetPosition);
            if (room.HasValue)
            {
                Vector2Int roomCenter = BoardManager.Instance.GetRoomCenter(room.Value);
                player.currentPosition = roomCenter;
                player.EnterRoom(room.Value);

                OnPlayerEnteredRoom?.Invoke(player, room.Value);
            }
            else
            {
                if (player.IsInRoom())
                {
                    player.ExitRoom();
                }
            }

            OnPlayerMoved?.Invoke(player, targetPosition);

            // 이동 후 하이라이트 제거
            ClearHighlights();

            return true;
        }

        public bool MovePlayerOneStep(PlayerData player, Vector2Int direction)
        {
            Vector2Int newPosition = player.currentPosition + direction;
            return MovePlayer(player, newPosition);
        }

        public void MovePlayerRandomly(PlayerData player, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                List<Vector2Int> neighbors = BoardManager.Instance.GetWalkableNeighbors(player.currentPosition);

                if (neighbors.Count == 0) break;

                Vector2Int randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
                MovePlayer(player, randomNeighbor);

                RoomCard? room = BoardManager.Instance.GetRoomAtPosition(player.currentPosition);
                if (room.HasValue)
                {
                    break;
                }
            }
        }

        public bool UseSecretPassage(PlayerData player)
        {
            if (!player.IsInRoom())
            {
                Debug.LogWarning("방 안에 있지 않아 비밀 통로를 사용할 수 없습니다.");
                return false;
            }

            RoomCard currentRoom = player.currentRoom.Value;
            RoomCard? destination = BoardManager.Instance.GetSecretPassageDestination(currentRoom);

            if (!destination.HasValue)
            {
                Debug.LogWarning($"{currentRoom}에는 비밀 통로가 없습니다.");
                return false;
            }

            player.ExitRoom();
            Vector2Int destPosition = BoardManager.Instance.GetRoomCenter(destination.Value);
            player.currentPosition = destPosition;
            player.EnterRoom(destination.Value);

            Debug.Log($"{player.playerName}이(가) 비밀 통로를 통해 {currentRoom}에서 {destination.Value}로 이동했습니다!");
            OnPlayerEnteredRoom?.Invoke(player, destination.Value);

            return true;
        }
    }
}