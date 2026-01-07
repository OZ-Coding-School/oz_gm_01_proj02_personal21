using System.Collections.Generic;
using UnityEngine;
using ClueGame.Data;
using ClueGame.Player;
using System.Linq;

namespace ClueGame.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int numberOfPlayers = 4;

        private List<PlayerData> players = new List<PlayerData>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Invoke(nameof(InitializeGame), 0.1f);
        }

        private void InitializeGame()
        {
            CreatePlayers();
            DistributeCards();

            // TurnManager에 플레이어 설정
            TurnManager.Instance.SetPlayers(players);

            Debug.Log("게임 시작!");
            PrintAllPlayersHands();

            // 게임 시작
            TurnManager.Instance.StartGame();
        }

        private void CreatePlayers()
        {
            players.Clear();

            var availableCharacters = new List<CharacterCard>
            {
             CharacterCard.MissScarlet,
             CharacterCard.ColonelMustard,
             CharacterCard.MrsWhite,
             CharacterCard.MrGreen,
             CharacterCard.MrsPeacock,
             CharacterCard.ProfessorPlum
            };

            for (int i = 0; i < numberOfPlayers; i++)
            {
                var character = availableCharacters[i];
                string playerName = i == 0 ? "Player" : $"AI {i}";
                bool isAI = i > 0;

                PlayerData player = new PlayerData(playerName, character, isAI);

                // 시작 위치 설정
                player.currentPosition = BoardManager.Instance.GetStartPosition(i);

                players.Add(player);
            }

            Debug.Log($"{numberOfPlayers}명의 플레이어가 생성되었습니다.");
        }

        private void DistributeCards()
        {
            List<Card> cardsToDistribute = CardManager.Instance.GetCardsForDistribution();

            int cardIndex = 0;
            while (cardIndex < cardsToDistribute.Count)
            {
                for (int i = 0; i < players.Count && cardIndex < cardsToDistribute.Count; i++)
                {
                    players[i].AddCard(cardsToDistribute[cardIndex]);
                    cardIndex++;
                }
            }

            Debug.Log("카드 분배 완료!");
        }

        private void PrintAllPlayersHands()
        {
            foreach (var player in players)
            {
                player.PrintHand();
            }
        }

        // 제안 처리 (개선됨)
        public Card ProcessSuggestion(Card character, Card weapon, Card room)
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            Debug.Log($"{currentPlayer.playerName}의 제안: {character.cardName} + {weapon.cardName} + {room.cardName}");

            // 현재 플레이어의 다음부터 순서대로 확인
            int currentIndex = players.IndexOf(currentPlayer);

            for (int i = 1; i < players.Count; i++)
            {
                int playerIndex = (currentIndex + i) % players.Count;
                var player = players[playerIndex];

                if (player.isEliminated) continue;

                Card matchingCard = player.GetMatchingCard(character, weapon, room);
                if (matchingCard != null)
                {
                    Debug.Log($"{player.playerName}이(가) {matchingCard.cardName} 카드를 공개했습니다.");
                    return matchingCard;
                }
            }

            Debug.Log("아무도 카드를 가지고 있지 않습니다!");
            return null;
        }

        // 고발 처리
        public bool ProcessAccusation(Card character, Card weapon, Card room)
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            Debug.Log($"{currentPlayer.playerName}의 고발: {character.cardName} + {weapon.cardName} + {room.cardName}");

            bool isCorrect = CardManager.Instance.CheckSolution(character, weapon, room);

            if (isCorrect)
            {
              // 승리
            }
            else
            {
             // 실패
                currentPlayer.isEliminated = true;
            }

            return isCorrect;
        }

        // 플레이어 리스트 반환
        public List<PlayerData> GetPlayers()
        {
            return players;
        }
    }
}