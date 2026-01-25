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

        // 보드 위치
        public Vector2Int currentPosition;
        public RoomCard? currentRoom;

        // 턴당 제한 
        public bool hasSuggestedThisTurn = false;
        public bool hasAccusedThisTurn = false;

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
   
            foreach (var card in hand)
            {
        
            }
        }

        public void EnterRoom(RoomCard room)
        {
            currentRoom = room;
        
        }

        public void ExitRoom()
        {
            if (currentRoom.HasValue)
            {
           
                currentRoom = null;
            }
        }

        public bool IsInRoom()
        {
            return currentRoom.HasValue;
        }

        public void EliminatePlayer()
        {
            isEliminated = true;
    
        }

        // 턴 시작 시 초기화 
        public void ResetTurnActions()
        {
            hasSuggestedThisTurn = false;
            hasAccusedThisTurn = false;
        }
    }
}