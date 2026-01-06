using UnityEngine;
using ClueGame.Data;
using ClueGame.Player;
using System.Collections.Generic;

namespace ClueGame.Managers
{
    public class SuggestionManager : MonoBehaviour
    {
        public static SuggestionManager Instance { get; private set; }

        // 제안 결과 이벤트
        public System.Action<PlayerData, Card> OnCardRevealed; // 누가, 어떤 카드를 공개했는지
        public System.Action OnNoCardRevealed; // 아무도 카드가 없을 때

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

        // 제안하기
        public Card MakeSuggestion(Card character, Card weapon, Card room)
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            Debug.Log($"=== {currentPlayer.playerName}의 제안 ===");
            Debug.Log($"{character.cardName} + {weapon.cardName} + {room.cardName}");

            // 현재 플레이어의 다음 플레이어부터 순서대로 확인
            List<PlayerData> players = GameManager.Instance.GetPlayers();
            int currentIndex = players.IndexOf(currentPlayer);

            for (int i = 1; i < players.Count; i++)
            {
                int playerIndex = (currentIndex + i) % players.Count;
                PlayerData player = players[playerIndex];

                if (player.isEliminated) continue;

                // 해당 플레이어가 제안된 카드 중 하나라도 가지고 있는지 확인
                Card matchingCard = player.GetMatchingCard(character, weapon, room);

                if (matchingCard != null)
                {
                    Debug.Log($"→ {player.playerName}이(가) [{matchingCard.cardName}] 카드를 공개했습니다.");
                    OnCardRevealed?.Invoke(player, matchingCard);
                    return matchingCard;
                }
            }

            Debug.Log("→ 아무도 카드를 가지고 있지 않습니다!");
            OnNoCardRevealed?.Invoke();
            return null;
        }

        // AI가 제안할 카드 선택 (랜덤)
        public (Card character, Card weapon, Card room) GetRandomSuggestion(RoomCard currentRoom)
        {
            List<Card> allCards = CardManager.Instance.GetAllCards();

            // 캐릭터 카드 중 랜덤
            var characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            Card character = characterCards[Random.Range(0, characterCards.Count)];

            // 무기 카드 중 랜덤
            var weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            Card weapon = weaponCards[Random.Range(0, weaponCards.Count)];

            // 현재 방
            var roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
            Card room = roomCards.Find(c => c.cardName == currentRoom.ToString());

            return (character, weapon, room);
        }
    }
}