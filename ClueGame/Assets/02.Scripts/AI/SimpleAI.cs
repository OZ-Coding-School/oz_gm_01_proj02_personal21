using UnityEngine;
using ClueGame.Player;
using ClueGame.Managers;
using ClueGame.Data;
using System.Collections;
using System.Collections.Generic;

namespace ClueGame.AI
{
    public class SimpleAI : MonoBehaviour
    {
        public static SimpleAI Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float aiThinkDelay = 1f;

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
            // 턴 시작 이벤트 구독
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTurnStart += OnTurnStart;
            }
        }

        private void OnDestroy()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTurnStart -= OnTurnStart;
            }
        }

        private void OnTurnStart(PlayerData player)
        {
            // AI 플레이어의 턴이면 자동 플레이
            if (player.isAI)
            {
                StartCoroutine(AITurn());
            }
        }

        private IEnumerator AITurn()
        {
            yield return new WaitForSeconds(aiThinkDelay);

            // 1. 주사위 굴리기
            if (TurnManager.Instance.GetCurrentPhase() == GamePhase.RollingDice)
            {
                TurnManager.Instance.RollDice();
                yield return new WaitForSeconds(0.5f);
            }

            // 2. 이동
            if (TurnManager.Instance.GetCurrentPhase() == GamePhase.Moving)
            {
                PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();
                int moves = TurnManager.Instance.RemainingMoves;

                MovementManager.Instance.MovePlayerRandomly(currentPlayer, moves);
                TurnManager.Instance.OnMoveComplete();
                yield return new WaitForSeconds(0.5f);
            }

            // 3. 방에 있으면 제안하기
            if (TurnManager.Instance.GetCurrentPhase() == GamePhase.InRoom)
            {
                PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

                if (currentPlayer.IsInRoom())
                {
                    yield return new WaitForSeconds(aiThinkDelay);
                    MakeSuggestion(currentPlayer);
                }
            }

            // 4. 고발 여부 결정 (낮은 확률)
            if (Random.Range(0, 100) < 3) // 3% 확률 
            {
                yield return new WaitForSeconds(aiThinkDelay);
                MakeAccusation();
                yield break;
            }

            // 5. 턴 종료
            yield return new WaitForSeconds(aiThinkDelay);
            TurnManager.Instance.EndTurn();
        }

        private void MakeSuggestion(PlayerData player)
        {
            if (!player.currentRoom.HasValue) return;

            var allCards = CardManager.Instance.GetAllCards();

            // 랜덤 캐릭터 선택
            var characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            Card character = characterCards[Random.Range(0, characterCards.Count)];

            // 랜덤 무기 선택
            var weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            Card weapon = weaponCards[Random.Range(0, weaponCards.Count)];

            // 현재 방
            var roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
            Card room = roomCards.Find(c => c.cardName == player.currentRoom.ToString());

            if (room != null)
            {
                SuggestionManager.Instance.MakeSuggestion(character, weapon, room);
                TurnManager.Instance.OnSuggestionComplete();
            }
        }

        private void MakeAccusation()
        {
            var allCards = CardManager.Instance.GetAllCards();

            // 랜덤 고발 
            var characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            Card character = characterCards[Random.Range(0, characterCards.Count)];

            var weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            Card weapon = weaponCards[Random.Range(0, weaponCards.Count)]; 

            var roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
            Card room = roomCards[Random.Range(0, roomCards.Count)];

            bool result = AccusationManager.Instance.MakeAccusation(character, weapon, room);
            TurnManager.Instance.OnAccusationComplete(result);
        }
    }
}