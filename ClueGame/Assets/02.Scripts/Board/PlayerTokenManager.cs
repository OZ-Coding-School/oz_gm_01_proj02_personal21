using UnityEngine;
using System.Collections.Generic;
using ClueGame.Player;
using ClueGame.Board;

namespace ClueGame.Managers
{
    public class PlayerTokenManager : MonoBehaviour
    {
        public static PlayerTokenManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject tokenPrefab;
        [SerializeField] private Transform tokenContainer;

        private Dictionary<PlayerData, PlayerToken> playerTokens = new Dictionary<PlayerData, PlayerToken>();

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
            // 이동 이벤트 구독
            if (MovementManager.Instance != null)
            {
                MovementManager.Instance.OnPlayerMoved += OnPlayerMoved;
            }

            Invoke(nameof(CreatePlayerTokens), 0.5f);
        }

        private void OnDestroy()
        {
            if (MovementManager.Instance != null)
            {
                MovementManager.Instance.OnPlayerMoved -= OnPlayerMoved;
            }
        }

        private void CreatePlayerTokens()
        {
            var players = GameManager.Instance.GetPlayers();

            foreach (var player in players)
            {
                CreateToken(player);
            }

            Debug.Log($"{players.Count}개의 플레이어 토큰 생성 완료");
        }

        private void CreateToken(PlayerData player)
        {
            GameObject tokenObj;

            if (tokenPrefab != null)
            {
                tokenObj = Instantiate(tokenPrefab, tokenContainer);
            }
            else
            {
                // Prefab이 없으면 기본 원형 생성
                tokenObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                tokenObj.transform.SetParent(tokenContainer);
                tokenObj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // 0.5 → 0.8

                // SpriteRenderer 추가 (2D용)
                Destroy(tokenObj.GetComponent<MeshRenderer>());
                Destroy(tokenObj.GetComponent<MeshFilter>());
                Destroy(tokenObj.GetComponent<SphereCollider>());

                GameObject sprite = new GameObject("Sprite");
                sprite.transform.SetParent(tokenObj.transform);
                sprite.transform.localPosition = Vector3.zero;
                SpriteRenderer sr = sprite.AddComponent<SpriteRenderer>();

                // 원형 스프라이트 생성
                Texture2D tex = new Texture2D(32, 32);
                Color[] pixels = new Color[32 * 32];
                for (int i = 0; i < pixels.Length; i++)
                {
                    int x = i % 32 - 16;
                    int y = i / 32 - 16;
                    pixels[i] = (x * x + y * y < 256) ? Color.white : Color.clear;
                }
                tex.SetPixels(pixels);
                tex.Apply();

                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            }

            PlayerToken token = tokenObj.GetComponent<PlayerToken>();
            if (token == null)
            {
                token = tokenObj.AddComponent<PlayerToken>();
            }

            token.Initialize(player);
            playerTokens[player] = token;
        }

        private void OnPlayerMoved(PlayerData player, Vector2Int newPosition)
        {
            if (playerTokens.TryGetValue(player, out PlayerToken token))
            {
                token.UpdatePosition();
            }
        }

        public PlayerToken GetPlayerToken(PlayerData player)
        {
            playerTokens.TryGetValue(player, out PlayerToken token);
            return token;
        }
    }
}