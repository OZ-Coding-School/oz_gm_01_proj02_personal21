using UnityEngine;
using ClueGame.Data;
using ClueGame.Player;

namespace ClueGame.Managers
{
    public class AccusationManager : MonoBehaviour
    {
        public static AccusationManager Instance { get; private set; }

        // 고발 결과 이벤트
        public System.Action<PlayerData> OnAccusationCorrect; // 정답!
        public System.Action<PlayerData> OnAccusationWrong;   // 오답!

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

        // 고발하기
        public bool MakeAccusation(Card character, Card weapon, Card room)
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            Debug.Log($"=== {currentPlayer.playerName}의 고발 ===");
            Debug.Log($"{character.cardName} + {weapon.cardName} + {room.cardName}");

            // 정답 확인
            bool isCorrect = CardManager.Instance.CheckSolution(character, weapon, room);

            if (isCorrect)
            {
                Debug.Log($"정답입니다! {currentPlayer.playerName}이(가) 승리했습니다!");
                OnAccusationCorrect?.Invoke(currentPlayer);
            }
            else
            {
                Debug.Log($" 틀렸습니다! {currentPlayer.playerName}이(가) 탈락합니다");
                currentPlayer.isEliminated = true;
                OnAccusationWrong?.Invoke(currentPlayer);
            }

            return isCorrect;
        }

        // AI가 고발할지 결정 (간단한 로직)
        public bool ShouldAIMakeAccusation(PlayerData aiPlayer)
        {
            // AI가 18장 이상의 카드 정보를 알고 있으면 고발 (단순 로직)
            // 실제로는 더 복잡한 추론 시스템이 필요
            return Random.Range(0, 100) < 5; // 5% 확률로 고발 시도
        }

        // AI가 랜덤 고발 (단순 버전)
        public (Card character, Card weapon, Card room) GetRandomAccusation()
        {
            var allCards = CardManager.Instance.GetAllCards();

            var characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            Card character = characterCards[Random.Range(0, characterCards.Count)];

            var weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            Card weapon = weaponCards[Random.Range(0, weaponCards.Count)];

            var roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
            Card room = roomCards[Random.Range(0, roomCards.Count)];

            return (character, weapon, room);
        }
    }
}