using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ClueGame.Data;

namespace ClueGame.UI
{
    public class CardRevealUI : MonoBehaviour
    {
        public static CardRevealUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject revealPanel;
        [SerializeField] private Image cardContainer;      // 배경
        [SerializeField] private Image cardIcon;           // 아이콘
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI revealerNameText;
        [SerializeField] private Button closeButton;

        [Header("Animation Settings")]
        [SerializeField] private float flipDuration = 0.5f;
        [SerializeField] private float displayDuration = 2f;

        [Header("Card Backgrounds")]
        [SerializeField] private Sprite characterBackground;
        [SerializeField] private Sprite weaponBackground;
        [SerializeField] private Sprite roomBackground;

        [Header("Character Icons")]
        [SerializeField] private Sprite[] characterSprites = new Sprite[6];

        [Header("Weapon Icons")]
        [SerializeField] private Sprite[] weaponSprites = new Sprite[6];

        [Header("Room Icons")]
        [SerializeField] private Sprite[] roomSprites = new Sprite[9];

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
            if (revealPanel != null)
            {
                revealPanel.SetActive(false);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseReveal);
            }
        }

        public void ShowCardReveal(string revealerName, Card card)
        {
            StartCoroutine(CardRevealAnimation(revealerName, card));
        }

        private IEnumerator CardRevealAnimation(string revealerName, Card card)
        {
            if (revealPanel != null)
            {
                revealPanel.SetActive(true);
            }

            if (revealerNameText != null)
            {
                revealerNameText.text = $"{revealerName}이(가) 카드를 공개합니다!";
            }

            // 1. CardContainer에 배경 설정
            if (cardContainer != null)
            {
                Sprite background = card.cardType switch
                {
                    CardType.Character => characterBackground,
                    CardType.Weapon => weaponBackground,
                    CardType.Room => roomBackground,
                    _ => null
                };

                if (background != null)
                {
                    cardContainer.sprite = background;
                }
            }

            // 2. CardIcon에 아이콘 설정
            if (cardIcon != null)
            {
                Sprite icon = GetCardIcon(card);
                if (icon != null)
                {
                    cardIcon.sprite = icon;
                    cardIcon.enabled = true;
                }
                else
                {
                    cardIcon.enabled = false;
                }
            }

            // 3. CardContainer 전체를 뒤집기
            if (cardContainer != null)
            {
                float elapsed = 0f;
                Vector3 originalScale = cardContainer.transform.localScale;

                // 1단계: 카드 축소 (뒤집는 효과)
                while (elapsed < flipDuration / 2)
                {
                    float scaleX = Mathf.Lerp(1f, 0f, elapsed / (flipDuration / 2));
                    cardContainer.transform.localScale = new Vector3(scaleX, 1f, 1f);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // 카드 이름 표시 (중간에)
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName;
                }

                // 2단계: 카드 확대 (뒤집힌 상태)
                elapsed = 0f;
                while (elapsed < flipDuration / 2)
                {
                    float scaleX = Mathf.Lerp(0f, 1f, elapsed / (flipDuration / 2));
                    cardContainer.transform.localScale = new Vector3(scaleX, 1f, 1f);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                cardContainer.transform.localScale = originalScale;
            }

            // 잠시 표시
            yield return new WaitForSeconds(displayDuration);

            // 자동으로 닫기
            CloseReveal();
        }
        private void CloseReveal()
        {
            if (revealPanel != null)
            {
                revealPanel.SetActive(false);
            }
        }

        private Sprite GetCardIcon(Card card)
        {
            switch (card.cardType)
            {
                case CardType.Character:
                    int charIndex = GetCharacterIndex(card.cardName);
                    if (charIndex >= 0 && charIndex < characterSprites.Length)
                        return characterSprites[charIndex];
                    break;

                case CardType.Weapon:
                    int weaponIndex = GetWeaponIndex(card.cardName);
                    if (weaponIndex >= 0 && weaponIndex < weaponSprites.Length)
                        return weaponSprites[weaponIndex];
                    break;

                case CardType.Room:
                    int roomIndex = GetRoomIndex(card.cardName);
                    if (roomIndex >= 0 && roomIndex < roomSprites.Length)
                        return roomSprites[roomIndex];
                    break;
            }
            return null;
        }

        private int GetCharacterIndex(string name)
        {
            return name switch
            {
                "MissScarlet" => 0,
                "ColonelMustard" => 1,
                "MrsWhite" => 2,
                "MrGreen" => 3,
                "MrsPeacock" => 4,
                "ProfessorPlum" => 5,
                _ => -1
            };
        }

        private int GetWeaponIndex(string name)
        {
            return name switch
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

        private int GetRoomIndex(string name)
        {
            return name switch
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
    }
}