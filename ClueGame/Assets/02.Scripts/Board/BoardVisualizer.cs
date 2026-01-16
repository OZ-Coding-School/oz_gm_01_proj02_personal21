using ClueGame.Board;
using ClueGame.Data;
using ClueGame.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace ClueGame.Board
{
    public class BoardVisualizer : MonoBehaviour
    {
        public static BoardVisualizer Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform boardContainer;

        [Header("Colors")]
        [SerializeField] private Color hallwayColor = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color roomColor = new Color(0.7f, 0.8f, 1f);
        [SerializeField] private Color startPointColor = new Color(1f, 0.7f, 0.7f);

        [Header("Room Labels")]
        [SerializeField] private GameObject roomLabelPrefab;
        private List<RoomLabel> roomLabels = new List<RoomLabel>();

        private GameObject[,] tileObjects;

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

            // 방 라벨 
            CreateRoomLabels();

        }

        private void CreateTileVisual(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);
            BoardTile tile = BoardManager.Instance.GetTile(pos);

            if (tile == null) return;

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

            tileObj.transform.position = new Vector3(x * tileSize, y * tileSize, 0);
            tileObj.transform.localScale = new Vector3(tileSize * 0.95f, tileSize * 0.95f, 1f);
            tileObj.name = $"Tile_{x}_{y}";

            Renderer renderer = tileObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 새로운 Material 생성 (Transparent)
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = new Color(1f, 1f, 1f, 0f); // 완전 투명
                renderer.material = mat;
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

        private void CreateRoomLabels()
        {

            return;
            // 각 방의 중심 위치에 라벨 생성
            var rooms = System.Enum.GetValues(typeof(RoomCard));

            foreach (RoomCard room in rooms)
            {
                Vector2Int center = BoardManager.Instance.GetRoomCenter(room);
                Vector3 worldPos = new Vector3(center.x * tileSize, center.y * tileSize, -2);

                GameObject labelObj = new GameObject($"Label_{room}");
                labelObj.transform.SetParent(boardContainer);

                RoomLabel label = labelObj.AddComponent<RoomLabel>();
                label.Initialize(room, worldPos);

                roomLabels.Add(label);
            }
        }
    }
}