using ClueGame.Data;
using ClueGame.Player;
using ClueGame.UI;
using System.Collections.Generic;
using UnityEngine;

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

            if (currentPlayer.hasAccusedThisTurn)
            {
                Debug.LogWarning("이미 이번 턴에 고발했습니다!");
                return false;
            }

            Debug.Log($"=== {currentPlayer.playerName}의 고발 ===");
            Debug.Log($"{character.cardName} + {weapon.cardName} + {room.cardName}");

            Card answerCharacter = CardManager.Instance.GetAnswerCard(CardType.Character);
            Card answerWeapon = CardManager.Instance.GetAnswerCard(CardType.Weapon);
            Card answerRoom = CardManager.Instance.GetAnswerCard(CardType.Room);

            bool isCorrect = (character.cardId == answerCharacter.cardId &&
                             weapon.cardId == answerWeapon.cardId &&
                             room.cardId == answerRoom.cardId);

            currentPlayer.hasAccusedThisTurn = true;

            if (isCorrect)
            {
              

                // 승리 UI 표시
                if (GameEndUI.Instance != null)
                {
                    GameEndUI.Instance.ShowWin(
                        currentPlayer.playerName,
                        character.cardName,
                        weapon.cardName,
                        room.cardName
                    );
                }
            }
            else
            {
           
                currentPlayer.EliminatePlayer();

                // 모든 플레이어 탈락 확인
                CheckAllPlayersEliminated();

                TurnManager.Instance.EndTurn();
            }

            return isCorrect;
        }

        private void CheckAllPlayersEliminated()
        {
            List<PlayerData> players = GameManager.Instance.GetPlayers();

            // 사람 플레이어가 탈락했는지 확인
            bool humanPlayerEliminated = false;
            bool anyPlayerAlive = false;

            foreach (var player in players)
            {
                if (!player.isEliminated)
                {
                    anyPlayerAlive = true;
                }

                if (!player.isAI && player.isEliminated)
                {
                    humanPlayerEliminated = true;
                }
            }

            // 사람 플레이어가 탈락했으면 즉시 게임 종료
            if (humanPlayerEliminated)
            {
                Debug.Log("플레이어가 탈락했습니다! 게임 종료!");

                Card answerCharacter = CardManager.Instance.GetAnswerCard(CardType.Character);
                Card answerWeapon = CardManager.Instance.GetAnswerCard(CardType.Weapon);
                Card answerRoom = CardManager.Instance.GetAnswerCard(CardType.Room);

                if (GameEndUI.Instance != null)
                {
                    GameEndUI.Instance.ShowLose(
                        answerCharacter.cardName,
                        answerWeapon.cardName,
                        answerRoom.cardName
                    );
                }
                return;
            }

            // 모든 플레이어가 탈락했으면 게임 종료
            if (!anyPlayerAlive)
            {
                Debug.Log("모든 플레이어가 탈락했습니다! 게임 종료!");

                Card answerCharacter = CardManager.Instance.GetAnswerCard(CardType.Character);
                Card answerWeapon = CardManager.Instance.GetAnswerCard(CardType.Weapon);
                Card answerRoom = CardManager.Instance.GetAnswerCard(CardType.Room);

                if (GameEndUI.Instance != null)
                {
                    GameEndUI.Instance.ShowLose(
                        answerCharacter.cardName,
                        answerWeapon.cardName,
                        answerRoom.cardName
                    );
                }
            }
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