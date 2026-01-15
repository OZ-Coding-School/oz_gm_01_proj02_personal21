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
                Debug.Log("SimpleAI: OnTurnStart 이벤트 구독 완료");
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
            Debug.Log($"OnTurnStart 호출: {player.playerName}, isAI: {player.isAI}");

            // AI 플레이어의 턴이면 자동 플레이
            if (player.isAI && !player.isEliminated)
            {
                Debug.Log($"AI {player.playerName} 턴 시작!");
                StartCoroutine(AITurn(player));
            }
        }

        private IEnumerator AITurn(PlayerData player)
        {
            // 플레이어가 모두 탈락했는지 확인
            bool allHumanPlayersEliminated = CheckAllHumanPlayersEliminated();

            // 관전 모드: AI 딜레이 단축
            float delay = allHumanPlayersEliminated ? 0.1f : aiThinkDelay;

            Debug.Log($"AI {player.playerName}: AITurn 코루틴 시작");
            yield return new WaitForSeconds(delay);

            // 1. 주사위 굴리기
            GamePhase currentPhase = TurnManager.Instance.GetCurrentPhase();
            Debug.Log($"AI {player.playerName}: 현재 페이즈 = {currentPhase}");

            if (currentPhase == GamePhase.RollingDice)
            {
                Debug.Log($"AI {player.playerName}: 주사위 굴리기 시도");
                TurnManager.Instance.RollDice();
                yield return new WaitForSeconds(delay);
            }

            // 2. 이동
            while (TurnManager.Instance.GetCurrentPhase() == GamePhase.Moving &&
                   TurnManager.Instance.RemainingMoves > 0)
            {
                int moves = TurnManager.Instance.RemainingMoves;
                Debug.Log($"AI {player.playerName}: {moves}칸 이동");

                MovementManager.Instance.MovePlayerRandomly(player, moves);
                yield return new WaitForSeconds(delay);

                if (player.IsInRoom())
                {
                    Debug.Log($"AI {player.playerName}: 방 입장, 이동 종료");
                    break;
                }
            }

            yield return new WaitForSeconds(delay);

            // 3. 방에 있으면 제안하기
            if (player.IsInRoom())
            {
                Debug.Log($"AI {player.playerName}: 제안 시작");
                MakeSuggestion(player);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                // 4. 방 밖이면 고발 여부 결정 (낮은 확률)
                if (Random.Range(0, 100) < 3)
                {
                    Debug.Log($"AI {player.playerName}: 고발 시도");
                    MakeAccusation(player);
                    yield return new WaitForSeconds(delay);
                    yield break;
                }
            }

            // 5. 턴 종료
            yield return new WaitForSeconds(delay);
            Debug.Log($"AI {player.playerName}: 턴 종료");
            TurnManager.Instance.EndTurn();
        }

        // 모든 사람 플레이어가 탈락했는지 확인 (추가!)
        private bool CheckAllHumanPlayersEliminated()
        {
            List<PlayerData> players = GameManager.Instance.GetPlayers();

            foreach (var player in players)
            {
                if (!player.isAI && !player.isEliminated)
                {
                    return false; // 사람 플레이어가 남아있음
                }
            }

            return true; // 모든 사람 플레이어가 탈락
        }
        private void MakeSuggestion(PlayerData player)
        {
            if (!player.currentRoom.HasValue)
            {
                Debug.LogWarning("AI: 방에 없어서 제안 불가");
                return;
            }

            var allCards = CardManager.Instance.GetAllCards();

            // 랜덤 캐릭터 선택
            var characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            Card character = characterCards[Random.Range(0, characterCards.Count)];

            // 랜덤 무기 선택
            var weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            Card weapon = weaponCards[Random.Range(0, weaponCards.Count)];

            // 현재 방 카드 찾기
            var roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
            Card room = roomCards.Find(c => c.cardName == player.currentRoom.ToString());

            if (room != null)
            {
                Debug.Log($"AI 제안: {character.cardName} + {weapon.cardName} + {room.cardName}");
                SuggestionManager.Instance.MakeSuggestion(character, weapon, room);
            }
            else
            {
                Debug.LogWarning($"AI: 방 카드를 찾을 수 없음 - {player.currentRoom}");
            }
        }

        private void MakeAccusation(PlayerData player)
        {
            var allCards = CardManager.Instance.GetAllCards();

            // 랜덤 고발 
            var characterCards = allCards.FindAll(c => c.cardType == CardType.Character);
            Card character = characterCards[Random.Range(0, characterCards.Count)];

            var weaponCards = allCards.FindAll(c => c.cardType == CardType.Weapon);
            Card weapon = weaponCards[Random.Range(0, weaponCards.Count)];

            var roomCards = allCards.FindAll(c => c.cardType == CardType.Room);
            Card room = roomCards[Random.Range(0, roomCards.Count)];

            Debug.Log($"AI 고발: {character.cardName} + {weapon.cardName} + {room.cardName}");
            bool result = AccusationManager.Instance.MakeAccusation(character, weapon, room);

            if (!result)
            {
                player.EliminatePlayer();
                Debug.Log($"AI {player.playerName} 고발 실패! 탈락!");
            }
        }
    }
}