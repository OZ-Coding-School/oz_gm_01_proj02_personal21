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
        [SerializeField] private Image cardBorder;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI cardTypeText;
        [SerializeField] private Image cardIcon;

        [Header("Sprite References")]
        [SerializeField] private Sprite characterBackground;
        [SerializeField] private Sprite weaponBackground;
        [SerializeField] private Sprite roomBackground;

        // 캐릭터 스프라이트
        [SerializeField] private Sprite[] characterSprites = new Sprite[6];
        // 무기 스프라이트
        [SerializeField] private Sprite[] weaponSprites = new Sprite[6];
        // 방 스프라이트
        [SerializeField] private Sprite[] roomSprites = new Sprite[9];

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

            // 배경 설정
            if (cardBackground != null)
            {
                cardBackground.sprite = card.cardType switch
                {
                    CardType.Character => characterBackground,
                    CardType.Weapon => weaponBackground,
                    CardType.Room => roomBackground,
                    _ => null
                };
            }

            // 아이콘 설정
            if (cardIcon != null)
            {
                Sprite iconSprite = GetCardIcon(card);
                if (iconSprite != null)
                {
                    cardIcon.sprite = iconSprite;
                    cardIcon.enabled = true;
                }
                else
                {
                    cardIcon.enabled = false;
                }
            }

            // 테두리 색상
            if (cardBorder != null)
            {
                cardBorder.color = GetCardBorderColor(card.cardType);
            }
        }

        private Sprite GetCardIcon(Card card)
        {
            switch (card.cardType)
            {
                case CardType.Character:
                    int charIndex = (int)System.Enum.Parse(typeof(CharacterCard), card.cardName);
                    if (charIndex >= 0 && charIndex < characterSprites.Length)
                        return characterSprites[charIndex];
                    break;

                case CardType.Weapon:
                    // 무기 이름으로 인덱스 찾기
                    int weaponIndex = GetWeaponIndex(card.cardName);
                    if (weaponIndex >= 0 && weaponIndex < weaponSprites.Length)
                        return weaponSprites[weaponIndex];
                    break;

                case CardType.Room:
                    // 방 이름으로 인덱스 찾기
                    int roomIndex = GetRoomIndex(card.cardName);
                    if (roomIndex >= 0 && roomIndex < roomSprites.Length)
                        return roomSprites[roomIndex];
                    break;
            }

            return null;
        }

        private int GetWeaponIndex(string weaponName)
        {
            return weaponName switch
            {
                "촛대" => 0,
                "칼" => 1,
                "납파이프" => 2,
                "권총" => 3,
                "밧줄" => 4,
                "렌치" => 5,
                _ => -1
            };
        }

        private int GetRoomIndex(string roomName)
        {
            return roomName switch
            {
                "부엌" => 0,
                "무도회장" => 1,
                "온실" => 2,
                "식당" => 3,
                "당구장" => 4,
                "도서관" => 5,
                "라운지" => 6,
                "홀" => 7,
                "서재" => 8,
                _ => -1
            };
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

        private Color GetCardBorderColor(CardType type)
        {
            return type switch
            {
                CardType.Character => new Color(0.8f, 0.3f, 0.3f),
                CardType.Weapon => new Color(0.3f, 0.3f, 0.8f),
                CardType.Room => new Color(0.3f, 0.7f, 0.4f),
                _ => Color.black
            };
        }

        public Card GetCardData()
        {
            return cardData;
        }
    }
}