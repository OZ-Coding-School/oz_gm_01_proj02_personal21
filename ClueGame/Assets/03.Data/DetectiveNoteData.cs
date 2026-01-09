using ClueGame.Data;
using ClueGame.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace ClueGame.Data
{
    public enum CardStatus
    {
        Unknown,    // 모름
        Have,       // 내가 가지고 있음
        Shown,      // 누군가 보여줌
        NotAnswer   // 정답이 아님
    }

    [System.Serializable]
    public class CardNote
    {
        public Card card;
        public CardStatus status;

        public CardNote(Card card)
        {
            this.card = card;
            this.status = CardStatus.Unknown;
        }
    }

    public class DetectiveNoteData : MonoBehaviour
    {
        public static DetectiveNoteData Instance { get; private set; }

        private Dictionary<int, CardNote> cardNotes = new Dictionary<int, CardNote>();

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
            InitializeNotes();
        }

        private void InitializeNotes()
        {
            var allCards = CardManager.Instance.GetAllCards();

            foreach (var card in allCards)
            {
                cardNotes[card.cardId] = new CardNote(card);
            }

            // 내 카드는 자동으로 Have 상태로
            var players = GameManager.Instance.GetPlayers();
            if (players != null && players.Count > 0)
            {
                var myHand = players[0].hand;
                foreach (var card in myHand)
                {
                    SetCardStatus(card.cardId, CardStatus.Have);
                }
            }

            Debug.Log($"추리 노트 초기화: 총 {cardNotes.Count}개 카드");
        }

        public void SetCardStatus(int cardId, CardStatus status)
        {
            if (cardNotes.ContainsKey(cardId))
            {
                cardNotes[cardId].status = status;
            }
        }

        public CardStatus GetCardStatus(int cardId)
        {
            if (cardNotes.ContainsKey(cardId))
            {
                return cardNotes[cardId].status;
            }
            return CardStatus.Unknown;
        }

        public List<CardNote> GetAllNotes()
        {
            return new List<CardNote>(cardNotes.Values);
        }

        public List<CardNote> GetNotesByType(CardType type)
        {
            List<CardNote> result = new List<CardNote>();

            foreach (var note in cardNotes.Values)
            {
                if (note.card.cardType == type)
                {
                    result.Add(note);
                }
            }

            return result;
        }
    }
}