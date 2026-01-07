using System.Collections.Generic;
using UnityEngine;
using ClueGame.Data;

namespace ClueGame.Player
{
    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public CharacterCard characterCard;
        public List<Card> hand = new List<Card>();
        public bool isEliminated = false;
        public bool isAI = false;

        // 보드 위치 추가
        public Vector2Int currentPosition;
        public RoomCard? currentRoom; // null이면 복도, 값이 있으면 방 안

        public PlayerData(string name, CharacterCard character, bool ai = false)
        {
            playerName = name;
            characterCard = character;
            isAI = ai;
            currentPosition = Vector2Int.zero;
            currentRoom = null;
        }

        public void AddCard(Card card)
        {
            hand.Add(card);
        }

        public bool HasCard(Card card)
        {
            return hand.Exists(c => c.cardId == card.cardId);
        }

        public Card GetMatchingCard(Card character, Card weapon, Card room)
        {
            if (HasCard(character)) return character;
            if (HasCard(weapon)) return weapon;
            if (HasCard(room)) return room;
            return null;
        }

        // 여러 매칭 카드 반환 (플레이어 선택용)
        public List<Card> GetMatchingCards(Card character, Card weapon, Card room)
        {
            List<Card> matchingCards = new List<Card>();

            if (HasCard(character)) matchingCards.Add(character);
            if (HasCard(weapon)) matchingCards.Add(weapon);
            if (HasCard(room)) matchingCards.Add(room);

            return matchingCards;
        }

        public void PrintHand()
        {
            Debug.Log($"{playerName}의 카드:");
            foreach (var card in hand)
            {
                Debug.Log($"  - {card}");
            }
        }

        // 방에 입장
        public void EnterRoom(RoomCard room)
        {
            currentRoom = room;
            Debug.Log($"{playerName}이(가) {room}에 입장했습니다.");
        }

        // 방에서 나가기
        public void ExitRoom()
        {
            if (currentRoom.HasValue)
            {
                Debug.Log($"{playerName}이(가) {currentRoom}에서 나갔습니다.");
                currentRoom = null;
            }
        }

        // 방 안에 있는지 확인
        public bool IsInRoom()
        {
            return currentRoom.HasValue;
        }
    }
}