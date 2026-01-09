using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ClueGame.Data;

namespace ClueGame.UI
{
    public class DetectiveNoteUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject notePanel;
        [SerializeField] private GameObject noteCardPrefab;

        [Header("Containers")]
        [SerializeField] private Transform characterContainer;
        [SerializeField] private Transform weaponContainer;
        [SerializeField] private Transform roomContainer;

        [Header("Buttons")]
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        private List<NoteCardUI> allNoteCards = new List<NoteCardUI>();
        private bool isInitialized = false;

        private void Start()
        {
            if (openButton != null)
                openButton.onClick.AddListener(OpenNote);

            if (closeButton != null)
                closeButton.onClick.AddListener(CloseNote);

            // 시작 시 숨김
            if (notePanel != null)
                notePanel.SetActive(false);

            // 초기화 지연
            Invoke(nameof(InitializeNote), 1f);
        }

        private void InitializeNote()
        {
            if (isInitialized) return;

            CreateNoteCards(CardType.Character, characterContainer);
            CreateNoteCards(CardType.Weapon, weaponContainer);
            CreateNoteCards(CardType.Room, roomContainer);

            isInitialized = true;
            Debug.Log("추리 노트 UI 초기화 완료");
        }

        private void CreateNoteCards(CardType type, Transform container)
        {
            var notes = DetectiveNoteData.Instance.GetNotesByType(type);

            foreach (var note in notes)
            {
                GameObject cardObj = Instantiate(noteCardPrefab, container);
                NoteCardUI cardUI = cardObj.GetComponent<NoteCardUI>();

                if (cardUI != null)
                {
                    cardUI.SetupNoteCard(note);
                    allNoteCards.Add(cardUI);
                }
            }
        }

        private void OpenNote()
        {
            if (notePanel != null)
            {
                notePanel.SetActive(true);

                // 열 때마다 상태 갱신
                RefreshAllCards();
            }
        }

        private void CloseNote()
        {
            if (notePanel != null)
            {
                notePanel.SetActive(false);
            }
        }

        private void RefreshAllCards()
        {
            foreach (var cardUI in allNoteCards)
            {
                // 카드 상태 다시 확인
                var note = cardUI.GetCardNote();
                if (note != null)
                {
                    var currentStatus = DetectiveNoteData.Instance.GetCardStatus(note.card.cardId);
                    note.status = currentStatus;
                }
            }
        }

        // 외부에서 카드 상태 업데이트 시 호출
        public void UpdateCardStatus(int cardId, CardStatus status)
        {
            DetectiveNoteData.Instance.SetCardStatus(cardId, status);
            RefreshAllCards();
        }
    }
}