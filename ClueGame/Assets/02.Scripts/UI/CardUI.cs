using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClueGame.Data;

namespace ClueGame.UI
{
    public class CardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image cardBackground;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI cardTypeText;

        private Card cardData;

        public void SetupCard(Card card)
        {
            cardData = card;

            if (cardNameText != null)
            {
                cardNameText.text = card.cardName;
            }

            if (cardTypeText != null)
            {
                cardTypeText.text = GetCardTypeKorean(card.cardType);
            }

            if (cardBackground != null)
            {
                cardBackground.color = GetCardColor(card.cardType);
            }
        }

        private string GetCardTypeKorean(CardType type)
        {
            return type switch
            {
                CardType.Character => "캐릭터",
                CardType.Weapon => "무기",
                CardType.Room => "방",
                _ => ""
            };
        }

        private Color GetCardColor(CardType type)
        {
            return type switch
            {
                CardType.Character => new Color(1f, 0.7f, 0.7f), // 연한 빨강
                CardType.Weapon => new Color(0.7f, 0.7f, 1f),    // 연한 파랑
                CardType.Room => new Color(0.7f, 1f, 0.7f),      // 연한 초록
                _ => Color.white
            };
        }

        public Card GetCardData()
        {
            return cardData;
        }
    }
}