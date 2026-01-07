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

        private PlayerData currentMovingPlayer;
        private List<Vector2Int> availableMoves = new List<Vector2Int>();

        // 이동 이벤트
        public System.Action<PlayerData, Vector2Int> OnPlayerMoved;
        public System.Action<PlayerData, RoomCard> OnPlayerEnteredRoom;

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

        // 이동 가능한 위치 계산 (BFS 알고리즘)
        public List<Vector2Int> CalculateAvailableMoves(Vector2Int startPos, int moveRange)
        {
            availableMoves.Clear();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<(Vector2Int pos, int distance)> queue = new Queue<(Vector2Int, int)>();

            queue.Enqueue((startPos, 0));
            visited.Add(startPos);

            while (queue.Count > 0)
            {
                var (currentPos, distance) = queue.Dequeue();

                if (distance < moveRange)
                {
                    List<Vector2Int> neighbors = BoardManager.Instance.GetWalkableNeighbors(currentPos);

                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue((neighbor, distance + 1));
                            availableMoves.Add(neighbor);
                        }
                    }
                }
            }

            Debug.Log($"이동 가능한 위치: {availableMoves.Count}개");
            return availableMoves;
        }

        // 플레이어 이동
        public bool MovePlayer(PlayerData player, Vector2Int targetPosition)
        {
            // 이동 가능한 위치인지 확인
            if (!availableMoves.Contains(targetPosition))
            {
                Debug.LogWarning("이동할 수 없는 위치입니다!");
                return false;
            }

            // 이전 위치 타일 비우기
            BoardTile oldTile = BoardManager.Instance.GetTile(player.currentPosition);
            if (oldTile != null)
            {
                oldTile.isOccupied = false;
            }

            // 새 위치로 이동
            player.currentPosition = targetPosition;

            // 새 위치 타일 점유
            BoardTile newTile = BoardManager.Instance.GetTile(targetPosition);
            if (newTile != null)
            {
                newTile.isOccupied = true;
            }

            Debug.Log($"{player.playerName}이(가) {targetPosition}로 이동했습니다.");
            OnPlayerMoved?.Invoke(player, targetPosition);

            // 방에 입장했는지 확인
            RoomCard? room = BoardManager.Instance.GetRoomAtPosition(targetPosition);
            if (room.HasValue)
            {
                player.EnterRoom(room.Value);
                OnPlayerEnteredRoom?.Invoke(player, room.Value);
                return true;
            }

            return false;
        }

        // 한 칸 이동 (간단한 버전)
        public bool MovePlayerOneStep(PlayerData player, Vector2Int direction)
        {
            Vector2Int newPosition = player.currentPosition + direction;

            if (BoardManager.Instance.CanMoveTo(newPosition))
            {
                return MovePlayer(player, newPosition);
            }

            return false;
        }

        // 랜덤 이동 (AI용)
        public void MovePlayerRandomly(PlayerData player, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                List<Vector2Int> neighbors = BoardManager.Instance.GetWalkableNeighbors(player.currentPosition);

                if (neighbors.Count > 0)
                {
                    Vector2Int randomPos = neighbors[Random.Range(0, neighbors.Count)];

                    // 이전 위치 비우기
                    BoardTile oldTile = BoardManager.Instance.GetTile(player.currentPosition);
                    if (oldTile != null) oldTile.isOccupied = false;

                    // 이동
                    player.currentPosition = randomPos;

                    // 새 위치 점유
                    BoardTile newTile = BoardManager.Instance.GetTile(randomPos);
                    if (newTile != null) newTile.isOccupied = true;

                    // 방에 도착하면 중단
                    RoomCard? room = BoardManager.Instance.GetRoomAtPosition(randomPos);
                    if (room.HasValue)
                    {
                        player.EnterRoom(room.Value);
                        Debug.Log($"{player.playerName}이(가) {room.Value}에 도착했습니다!");
                        OnPlayerEnteredRoom?.Invoke(player, room.Value);
                        break;
                    }
                }
            }

            Debug.Log($"{player.playerName} 최종 위치: {player.currentPosition}");
        }

        // 이동 가능 위치 목록 반환
        public List<Vector2Int> GetAvailableMoves()
        {
            return new List<Vector2Int>(availableMoves);
        }

        // 이동 가능 위치 초기화
        public void ClearAvailableMoves()
        {
            availableMoves.Clear();
        }

        // ===== 비밀 통로 사용 =====
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

            // 비밀 통로로 이동
            player.ExitRoom();
            Vector2Int destPosition = BoardManager.Instance.GetRoomCenter(destination.Value);
            player.currentPosition = destPosition;
            player.EnterRoom(destination.Value);

            Debug.Log($"{player.playerName}이(가) 비밀 통로를 통해 {currentRoom}에서 {destination.Value}로 이동했습니다!");
            OnPlayerEnteredRoom?.Invoke(player, destination.Value);

            return true;
        }
        // =========================
    }
}