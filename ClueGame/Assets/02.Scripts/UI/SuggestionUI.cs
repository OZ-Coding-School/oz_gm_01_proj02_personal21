using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ClueGame.Data;
using ClueGame.Managers;
using ClueGame.Player;

namespace ClueGame.UI
{
    public class SuggestionUI : MonoBehaviour
    {
        public static SuggestionUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject suggestionPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TMP_Dropdown characterDropdown;
        [SerializeField] private TMP_Dropdown weaponDropdown;
        [SerializeField] private TMP_Dropdown roomDropdown;  
        [SerializeField] private TextMeshProUGUI roomText;   // 제안용
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private bool isAccusation = false;
        private List<Card> characterCards;
        private List<Card> weaponCards;
        private List<Card> roomCards;

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
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
            }

            if (suggestionPanel != null)
            {
                suggestionPanel.SetActive(false);
            }

            LoadCards();
        }

        private void LoadCards()
        {
            var allCards = CardManager.Instance.GetAllCards();

            characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
        }

        // 제안 패널 열기
        public void ShowSuggestionPanel()
        {
            isAccusation = false;

            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            if (!currentPlayer.IsInRoom())
            {
                Debug.LogWarning("방 안에 있지 않아 제안할 수 없습니다.");
                return;
            }

            SetupDropdowns();

            // 제안: 방 텍스트만 보이기
            if (roomDropdown != null)
            {
                roomDropdown.gameObject.SetActive(false);
            }

            if (roomText != null)
            {
                roomText.gameObject.SetActive(true);
                roomText.text = $"{currentPlayer.currentRoom} (현재 위치)";
            }

            if (titleText != null)
            {
                titleText.text = "제안하기";
            }

            if (suggestionPanel != null)
            {
                suggestionPanel.SetActive(true);
            }
        }

        // 고발 패널 열기
        public void ShowAccusationPanel()
        {
            isAccusation = true;

            SetupDropdowns();

            // 고발: 방 드롭다운 보이기
            if (roomDropdown != null)
            {
                roomDropdown.gameObject.SetActive(true);

                // 방 드롭다운 설정
                roomDropdown.ClearOptions();
                List<string> roomOptions = new List<string>();
                foreach (var card in roomCards)
                {
                    roomOptions.Add(card.cardName);
                }
                roomDropdown.AddOptions(roomOptions);
            }

            if (roomText != null)
            {
                roomText.gameObject.SetActive(false);
            }

            if (titleText != null)
            {
                titleText.text = "고발하기";
            }

            if (suggestionPanel != null)
            {
                suggestionPanel.SetActive(true);
            }
        }

        private void SetupDropdowns()
        {
            // 캐릭터 드롭다운
            if (characterDropdown != null)
            {
                characterDropdown.ClearOptions();
                List<string> characterOptions = new List<string>();
                foreach (var card in characterCards)
                {
                    characterOptions.Add(card.cardName);
                }
                characterDropdown.AddOptions(characterOptions);
            }

            // 무기 드롭다운
            if (weaponDropdown != null)
            {
                weaponDropdown.ClearOptions();
                List<string> weaponOptions = new List<string>();
                foreach (var card in weaponCards)
                {
                    weaponOptions.Add(card.cardName);
                }
                weaponDropdown.AddOptions(weaponOptions);
            }
        }

        private void OnConfirmClicked()
        {
            Card character = characterCards[characterDropdown.value];
            Card weapon = weaponCards[weaponDropdown.value];

            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            if (isAccusation)
            {
                // 고발: 방도 선택
                Card room = roomCards[roomDropdown.value];

                Debug.Log($"고발: {character.cardName} + {weapon.cardName} + {room.cardName}");
                bool result = AccusationManager.Instance.MakeAccusation(character, weapon, room);

                if (!result)
                {
                    currentPlayer.EliminatePlayer();
                    Debug.Log($"{currentPlayer.playerName} 고발 실패! 탈락!");
                }
            }
            else
            {
                // 제안: 현재 방 사용
                if (currentPlayer.IsInRoom())
                {
                    Card room = roomCards.Find(c => c.cardName == currentPlayer.currentRoom.ToString());
                    if (room != null)
                    {
                        Debug.Log($"제안: {character.cardName} + {weapon.cardName} + {room.cardName}");
                        SuggestionManager.Instance.MakeSuggestion(character, weapon, room);
                    }
                }
            }

            ClosePanel();
        }

        private void OnCancelClicked()
        {
            ClosePanel();
        }

        private void ClosePanel()
        {
            if (suggestionPanel != null)
            {
                suggestionPanel.SetActive(false);
            }
        }
    }
}