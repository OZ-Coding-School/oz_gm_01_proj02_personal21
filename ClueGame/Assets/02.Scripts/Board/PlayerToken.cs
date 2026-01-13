using UnityEngine;
using TMPro;
using ClueGame.Player;
using ClueGame.Data;
using System.Collections;

namespace ClueGame.Board
{
    public class PlayerToken : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshPro nameText;

        [Header("Animation")]
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private PlayerData playerData;
        private Coroutine moveCoroutine;

        public void Initialize(PlayerData player)
        {
            playerData = player;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = GetCharacterColor(player.characterCard);
            }

            if (nameText == null)
            {
                GameObject textObj = new GameObject("NameText");
                textObj.transform.SetParent(transform);
                textObj.transform.localPosition = new Vector3(0, -0.3f, 0);

                nameText = textObj.AddComponent<TextMeshPro>();
                nameText.fontSize = 2;
                nameText.alignment = TextAlignmentOptions.Center;
                nameText.color = Color.white;
            }

            string shortName = player.isAI ? $"AI{player.playerName.Replace("AI ", "")}" : "P";
            nameText.text = shortName;

            gameObject.name = $"Token_{player.playerName}";
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            if (playerData != null)
            {
                Vector3 targetPos = new Vector3(
                    playerData.currentPosition.x * tileSize,
                    playerData.currentPosition.y * tileSize,
                    -2f
                );

                // 애니메이션으로 이동
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }
                moveCoroutine = StartCoroutine(MoveToPosition(targetPos));
            }
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveDuration;
                float curveValue = moveCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

                yield return null;
            }

            transform.position = targetPosition;
        }

        private Color GetCharacterColor(CharacterCard character)
        {
            return character switch
            {
                CharacterCard.MissScarlet => new Color(0.9f, 0.2f, 0.2f),
                CharacterCard.ColonelMustard => new Color(0.95f, 0.95f, 0.2f),
                CharacterCard.MrsWhite => new Color(0.95f, 0.95f, 0.95f),
                CharacterCard.MrGreen => new Color(0.2f, 0.9f, 0.2f),
                CharacterCard.MrsPeacock => new Color(0.2f, 0.5f, 0.9f),
                CharacterCard.ProfessorPlum => new Color(0.7f, 0.2f, 0.7f),
                _ => Color.white
            };
        }

        public PlayerData GetPlayerData()
        {
            return playerData;
        }
    }
}