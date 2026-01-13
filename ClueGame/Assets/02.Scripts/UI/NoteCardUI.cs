using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClueGame.Data;

namespace ClueGame.UI
{
    public class NoteCardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image cardBackgroundImage; // 배경 이미지 참조
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private Image statusIcon;
        [SerializeField] private Button cardButton;

        private CardNote cardNote;

        // 상태별 색상
        private Color unknownColor = new Color(0.8f, 0.8f, 0.8f); // 회색
        private Color haveColor = new Color(0.5f, 1f, 0.5f);      // 초록
        private Color shownColor = new Color(1f, 1f, 0.5f);       // 노랑
        private Color notAnswerColor = new Color(1f, 0.5f, 0.5f); // 빨강

        private void Start()
        {
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(OnCardClicked);
            }
        }

        // 배경 스프라이트를 설정하는 메서드 추가
        public void SetBackground(Sprite sectionSprite)
        {
            if (cardBackgroundImage != null && sectionSprite != null)
            {
                cardBackgroundImage.sprite = sectionSprite;
            }
        }

        public void SetupNoteCard(CardNote note)
        {
            cardNote = note;

            if (cardNameText != null)
            {
                cardNameText.text = note.card.cardName;
            }

            UpdateStatusDisplay();
        }

        private void OnCardClicked()
        {
            if (cardNote == null) return;

            // 상태 순환: Unknown → NotAnswer → Unknown
            // (Have와 Shown은 자동으로 설정됨)
            if (cardNote.status == CardStatus.Unknown)
            {
                cardNote.status = CardStatus.NotAnswer;
            }
            else if (cardNote.status == CardStatus.NotAnswer)
            {
                cardNote.status = CardStatus.Unknown;
            }

            DetectiveNoteData.Instance.SetCardStatus(cardNote.card.cardId, cardNote.status);
            UpdateStatusDisplay();
        }

        private void UpdateStatusDisplay()
        {
            if (statusIcon == null) return;

            Color color = cardNote.status switch
            {
                CardStatus.Unknown => unknownColor,
                CardStatus.Have => haveColor,
                CardStatus.Shown => shownColor,
                CardStatus.NotAnswer => notAnswerColor,
                _ => unknownColor
            };

            statusIcon.color = color;

            // Have와 Shown 상태면 버튼 비활성화
            if (cardButton != null)
            {
                cardButton.interactable = (cardNote.status != CardStatus.Have &&
                                          cardNote.status != CardStatus.Shown);
            }
        }

        public CardNote GetCardNote()
        {
            return cardNote;
        }
    }
}