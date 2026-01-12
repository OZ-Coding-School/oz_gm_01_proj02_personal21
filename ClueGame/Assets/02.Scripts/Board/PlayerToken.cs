using UnityEngine;
using ClueGame.Player;
using ClueGame.Data;

namespace ClueGame.Board
{
    public class PlayerToken : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private PlayerData playerData;

        public void Initialize(PlayerData player)
        {
            playerData = player;

            // 색상 설정
            if (spriteRenderer != null)
            {
                spriteRenderer.color = GetCharacterColor(player.characterCard);
            }

            // 이름 표시
            gameObject.name = $"Token_{player.playerName}";

            // 시작 위치로 이동
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            if (playerData != null)
            {
                Vector3 worldPos = new Vector3(
                    playerData.currentPosition.x * tileSize,
                    playerData.currentPosition.y * tileSize,
                    -1f // 보드보다 앞에
                );
                transform.position = worldPos;
            }
        }

        private Color GetCharacterColor(CharacterCard character)
        {
            return character switch
            {
                CharacterCard.MissScarlet => new Color(0.8f, 0.2f, 0.2f),    // 빨강
                CharacterCard.ColonelMustard => new Color(0.9f, 0.9f, 0.2f), // 노랑
                CharacterCard.MrsWhite => new Color(0.9f, 0.9f, 0.9f),       // 흰색
                CharacterCard.MrGreen => new Color(0.2f, 0.8f, 0.2f),        // 초록
                CharacterCard.MrsPeacock => new Color(0.2f, 0.4f, 0.8f),     // 파랑
                CharacterCard.ProfessorPlum => new Color(0.6f, 0.2f, 0.6f),  // 보라
                _ => Color.white
            };
        }

        public PlayerData GetPlayerData()
        {
            return playerData;
        }
    }
}