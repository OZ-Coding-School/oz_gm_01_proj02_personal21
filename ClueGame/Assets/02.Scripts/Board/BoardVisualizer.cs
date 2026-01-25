using ClueGame.Board;
using ClueGame.Data;
using ClueGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClueGame.Board
{
    public class BoardVisualizer : MonoBehaviour
    {
        public static BoardVisualizer Instance { get; private set; }

        [Header("UI Settings")]
        [SerializeField] private RectTransform boardContainer;

        [Header("Tile Colors")]
        [SerializeField] private Color hallwayColor = new Color(1f, 1f, 0.8f, 0.3f);
        [SerializeField] private Color roomColor = new Color(0.8f, 0.9f, 1f, 0.3f);
        [SerializeField] private Color startPointColor = new Color(1f, 0.7f, 0.7f, 0.5f);

        [Header("Highlight Colors")]
        [SerializeField] private Color moveHighlightColor = new Color(0f, 1f, 0f, 0.6f);
        [SerializeField] private Color roomHighlightColor = new Color(1f, 0f, 0f, 0.6f);

        [Header("Grid Settings")]
        [SerializeField] private bool showGridLines = true;
        [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

        [Header("Room Colors")]
        [SerializeField] private bool useRoomSpecificColors = true;

        private Dictionary<Vector2Int, Image> visualTiles = new Dictionary<Vector2Int, Image>();
        private List<Vector2Int> currentHighlightedTiles = new List<Vector2Int>();

        private int boardWidth = 24;
        private int boardHeight = 24;
        private bool isBoardCreated = false;

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

        private void OnEnable()
        {
            if (boardContainer == null)
            {
                StartCoroutine(FindBoardContainerDelayed());
            }
        }

        private IEnumerator FindBoardContainerDelayed()
        {
            yield return null;

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform container = canvas.transform.Find("BoardContainer");
                if (container != null)
                {
                    boardContainer = container.GetComponent<RectTransform>();

                    if (!isBoardCreated)
                    {
                        CreateBoard();
                    }
                }
            }
        }

        private void Start()
        {
        }

        private void CreateBoard()
        {
            if (boardContainer == null) return;
            if (isBoardCreated) return;

            float containerWidth = boardContainer.rect.width;
            float containerHeight = boardContainer.rect.height;

            float tileWidth = containerWidth / boardWidth;
            float tileHeight = containerHeight / boardHeight;

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    BoardTile tile = BoardManager.Instance.GetTile(new Vector2Int(x, y));
                    if (tile == null) continue;

                    GameObject tileObj = new GameObject($"Tile_{x}_{y}");
                    tileObj.transform.SetParent(boardContainer, false);

                    Image tileImage = tileObj.AddComponent<Image>();

                    Color tileColor;
                    if (useRoomSpecificColors && tile.tileType == TileType.Room && tile.roomType.HasValue)
                    {
                        tileColor = GetRoomColor(tile.roomType.Value);
                    }
                    else
                    {
                        tileColor = GetTileColor(tile.tileType);
                    }

                    tileImage.color = tileColor;

                    RectTransform rectTransform = tileObj.GetComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 0);
                    rectTransform.pivot = new Vector2(0, 0);
                    rectTransform.anchoredPosition = new Vector2(x * tileWidth, y * tileHeight);
                    rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);

                    if (tile.tileType != TileType.Empty)
                    {
                        Outline outline = tileObj.AddComponent<Outline>();
                        outline.effectColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                        outline.effectDistance = new Vector2(1, 1);
                    }

                    visualTiles[new Vector2Int(x, y)] = tileImage;
                }
            }

            if (showGridLines)
            {
                CreateGridLines(containerWidth, containerHeight, tileWidth, tileHeight);
            }

            isBoardCreated = true;
        }

        private void CreateGridLines(float boardWidth, float boardHeight, float tileWidth, float tileHeight)
        {
            for (int x = 0; x <= this.boardWidth; x++)
            {
                GameObject line = new GameObject($"VLine_{x}");
                line.transform.SetParent(boardContainer, false);

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = gridLineColor;
                lineImage.raycastTarget = false;

                RectTransform rect = line.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(x * tileWidth, 0);
                rect.sizeDelta = new Vector2(1, 0);
            }

            for (int y = 0; y <= this.boardHeight; y++)
            {
                GameObject line = new GameObject($"HLine_{y}");
                line.transform.SetParent(boardContainer, false);

                Image lineImage = line.AddComponent<Image>();
                lineImage.color = gridLineColor;
                lineImage.raycastTarget = false;

                RectTransform rect = line.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(0, y * tileHeight);
                rect.sizeDelta = new Vector2(0, 1);
            }
        }

        private Color GetTileColor(TileType type)
        {
            return type switch
            {
                TileType.Hallway => hallwayColor,
                TileType.Room => roomColor,
                TileType.StartPoint => startPointColor,
                TileType.Empty => Color.clear,
                _ => Color.clear
            };
        }

        private Color GetRoomColor(RoomCard room)
        {
            float alpha = 0.4f;

            return room switch
            {
                RoomCard.서재 => new Color(0.8f, 0.6f, 0.6f, alpha),
                RoomCard.도서관 => new Color(0.6f, 0.8f, 0.6f, alpha),
                RoomCard.당구장 => new Color(0.6f, 0.6f, 0.8f, alpha),
                RoomCard.온실 => new Color(0.6f, 0.9f, 0.6f, alpha),
                RoomCard.무도회장 => new Color(0.9f, 0.9f, 0.6f, alpha),
                RoomCard.홀 => new Color(0.9f, 0.7f, 0.5f, alpha),
                RoomCard.라운지 => new Color(0.8f, 0.5f, 0.7f, alpha),
                RoomCard.식당 => new Color(0.7f, 0.8f, 0.9f, alpha),
                RoomCard.부엌 => new Color(0.9f, 0.8f, 0.7f, alpha),
                _ => roomColor
            };
        }

        public void HighlightTile(Vector2Int position, Color color)
        {
            if (visualTiles.TryGetValue(position, out Image tileImage))
            {
                color.a = 0.7f;
                tileImage.color = color;

                RectTransform rect = tileImage.GetComponent<RectTransform>();
                rect.localScale = new Vector3(1.05f, 1.05f, 1f);

                if (!currentHighlightedTiles.Contains(position))
                {
                    currentHighlightedTiles.Add(position);
                }
            }
        }

        public void ClearHighlights()
        {
            foreach (var pos in currentHighlightedTiles)
            {
                if (visualTiles.TryGetValue(pos, out Image tileImage))
                {
                    BoardTile tile = BoardManager.Instance.GetTile(pos);
                    if (tile != null)
                    {
                        Color originalColor;
                        if (useRoomSpecificColors && tile.tileType == TileType.Room && tile.roomType.HasValue)
                        {
                            originalColor = GetRoomColor(tile.roomType.Value);
                        }
                        else
                        {
                            originalColor = GetTileColor(tile.tileType);
                        }

                        tileImage.color = originalColor;

                        RectTransform rect = tileImage.GetComponent<RectTransform>();
                        rect.localScale = Vector3.one;
                    }
                }
            }
            currentHighlightedTiles.Clear();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}