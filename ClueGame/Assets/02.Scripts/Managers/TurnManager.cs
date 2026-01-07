using UnityEngine;
using ClueGame.Player;
using System.Collections.Generic;

namespace ClueGame.Managers
{
    public enum GamePhase
    {
        GameStart,      // 게임 시작
        TurnStart,      // 턴 시작
        RollingDice,    // 주사위 굴리기
        Moving,         // 이동 중
        InRoom,         // 방 안에서
        Suggesting,     // 제안 중
        Accusing,       // 고발 중
        TurnEnd,        // 턴 종료
        GameEnd         // 게임 종료
    }

    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [Header("Turn Settings")]
        private GamePhase currentPhase = GamePhase.GameStart;
        private int currentPlayerIndex = 0;
        private List<PlayerData> players;

        private int remainingMoves = 0;
        public int RemainingMoves => remainingMoves;

        // 이벤트
        public System.Action<PlayerData> OnTurnStart;
        public System.Action<PlayerData> OnTurnEnd;
        public System.Action<GamePhase> OnPhaseChanged;

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

        // 플레이어 리스트 설정
        public void SetPlayers(List<PlayerData> playerList)
        {
            players = playerList;
            currentPlayerIndex = 0;
        }

        // 게임 시작
        public void StartGame()
        {
            ChangePhase(GamePhase.TurnStart);
            StartTurn();
        }

        // 턴 시작
        public void StartTurn()
        {
            PlayerData currentPlayer = GetCurrentPlayer();

            if (currentPlayer.isEliminated)
            {
                NextTurn();
                return;
            }

            Debug.Log($"=== {currentPlayer.playerName}의 턴 시작 ===");
            OnTurnStart?.Invoke(currentPlayer);

            ChangePhase(GamePhase.RollingDice);
        }

        // 주사위 굴리기
        public void RollDice()
        {
            if (currentPhase != GamePhase.RollingDice)
            {
                Debug.LogWarning("주사위를 굴릴 수 있는 페이즈가 아닙니다.");
                return;
            }

            int diceResult = DiceManager.Instance.RollDice();
            remainingMoves = diceResult;

            // 이동 가능한 위치 계산
            PlayerData currentPlayer = GetCurrentPlayer();
            MovementManager.Instance.CalculateAvailableMoves(currentPlayer.currentPosition, diceResult);

            ChangePhase(GamePhase.Moving);
        }

        // 이동 완료
        public void OnMoveComplete()
        {
            remainingMoves--;

            if (remainingMoves <= 0)
            {
                ChangePhase(GamePhase.InRoom);
            }
        }

        // 제안하기
        public void MakeSuggestion()
        {
            ChangePhase(GamePhase.Suggesting);
        }

        // 제안 완료
        public void OnSuggestionComplete()
        {
            ChangePhase(GamePhase.InRoom);
        }

        // 고발하기
        public void MakeAccusation()
        {
            ChangePhase(GamePhase.Accusing);
        }

        // 고발 완료
        public void OnAccusationComplete(bool isCorrect)
        {
            if (isCorrect)
            {
                ChangePhase(GamePhase.GameEnd);
            }
            else
            {
                EndTurn();
            }
        }

        // 턴 종료
        public void EndTurn()
        {
            PlayerData currentPlayer = GetCurrentPlayer();
            Debug.Log($"=== {currentPlayer.playerName}의 턴 종료 ===");

            OnTurnEnd?.Invoke(currentPlayer);
            ChangePhase(GamePhase.TurnEnd);

            NextTurn();
        }

        // 다음 플레이어로
        private void NextTurn()
        {
            do
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            }
            while (players[currentPlayerIndex].isEliminated);

            // 모든 플레이어가 탈락했는지 확인
            if (CheckAllPlayersEliminated())
            {
                ChangePhase(GamePhase.GameEnd);
                Debug.Log("게임 종료! 모든 플레이어가 탈락했습니다.");
                return;
            }

            ChangePhase(GamePhase.TurnStart);
            StartTurn();
        }

        // 페이즈 변경
        private void ChangePhase(GamePhase newPhase)
        {
            currentPhase = newPhase;
            Debug.Log($"Phase Changed: {newPhase}");
            OnPhaseChanged?.Invoke(newPhase);
        }

        // 현재 플레이어 가져오기
        public PlayerData GetCurrentPlayer()
        {
            return players[currentPlayerIndex];
        }

        // 현재 페이즈 가져오기
        public GamePhase GetCurrentPhase()
        {
            return currentPhase;
        }

        // 모든 플레이어 탈락 확인
        private bool CheckAllPlayersEliminated()
        {
            foreach (var player in players)
            {
                if (!player.isEliminated)
                    return false;
            }
            return true;
        }

        // 턴 스킵 (방에 있지 않을 때)
        public void SkipToNextTurn()
        {
            EndTurn();
        }
    }
}