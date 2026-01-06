using System.Collections.Generic;
using UnityEngine;
using ClueGame.Data;

namespace ClueGame.Player
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public CharacterCard characterCard; // 플레이어가 조종하는 캐릭터
        public List<Card> hand = new List<Card>(); // 보유한 카드들
        public bool isEliminated = false; // 탈락 여부
        public bool isAI = false; // AI 플레이어인지

        public PlayerData(string name, CharacterCard character, bool ai = false)
        {
            playerName = name;
            characterCard = character;
            isAI = ai;
        }

        // 카드 추가
        public void AddCard(Card card)
        {
            hand.Add(card);
        }

        // 특정 카드를 가지고 있는지 확인
        public bool HasCard(Card card)
        {
            return hand.Exists(c => c.cardId == card.cardId);
        }

        // 제안된 카드 중 하나라도 가지고 있으면 반환
        public Card GetMatchingCard(Card character, Card weapon, Card room)
        {
            if (HasCard(character)) return character;
            if (HasCard(weapon)) return weapon;
            if (HasCard(room)) return room;
            return null;
        }

        // 보유 카드 출력
        public void PrintHand()
        {
            Debug.Log($"{playerName}의 카드:");
            foreach (var card in hand)
            {
                Debug.Log($"  - {card}");
            }
        }
    }
}