using UnityEngine;
using ClueGame.Managers;
using ClueGame.Data;
using ClueGame.Board;

namespace ClueGame.Board
{
    public class BoardVisualizer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform boardContainer;

        [Header("Colors")]
        [SerializeField] private Color hallwayColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color roomColor = new Color(0.7f, 0.8f, 1f);
        [SerializeField] private Color startPointColor = new Color(1f, 0.7f, 0.7f);

        private GameObject[,] tileObjects;

        private void Start()
        {
            Invoke(nameof(CreateBoardVisual), 0.2f);
        }

        private void CreateBoardVisual()
        {
            Vector2Int boardSize = BoardManager.Instance.GetBoardSize();
            tileObjects = new GameObject[boardSize.x, boardSize.y];

            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    CreateTileVisual(x, y);
                }
            }

            Debug.Log("보드 비주얼 생성 완료");
        }

        private void CreateTileVisual(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);
            BoardTile tile = BoardManager.Instance.GetTile(pos);

            if (tile == null) return;

            // 타일 생성
            GameObject tileObj;

            if (tilePrefab != null)
            {
                tileObj = Instantiate(tilePrefab, boardContainer);
            }
            else
            {
                tileObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tileObj.transform.SetParent(boardContainer);
            }

            // 위치 설정
            tileObj.transform.position = new Vector3(x * tileSize, y * tileSize, 0);
            tileObj.transform.localScale = new Vector3(tileSize * 0.9f, tileSize * 0.9f, 1f);
            tileObj.name = $"Tile_{x}_{y}";

            // 색상 설정
            Renderer renderer = tileObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = tile.tileType switch
                {
                    TileType.Hallway => hallwayColor,
                    TileType.Room => GetRoomColor(tile.roomType),
                    TileType.StartPoint => startPointColor,
                    _ => hallwayColor
                };

                renderer.material.color = color;
            }

            tileObjects[x, y] = tileObj;
        }

        private Color GetRoomColor(RoomCard? roomType)
        {
            if (!roomType.HasValue) return roomColor;

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
                _ => roomColor
            };
        }

        // 특정 타일 하이라이트
        public void HighlightTile(Vector2Int pos, Color color)
        {
            if (pos.x >= 0 && pos.x < tileObjects.GetLength(0) &&
                pos.y >= 0 && pos.y < tileObjects.GetLength(1))
            {
                GameObject tile = tileObjects[pos.x, pos.y];
                if (tile != null)
                {
                    Renderer renderer = tile.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = color;
                    }
                }
            }
        }
    }
}