using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClueGame.Managers;
using ClueGame.Player;

namespace ClueGame.UI
{
    public class TurnInfoUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI currentPlayerText;
        [SerializeField] private TextMeshProUGUI phaseText;
        [SerializeField] private TextMeshProUGUI remainingMovesText;

        private void Start()
        {
            // 이벤트 구독
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTurnStart += UpdateTurnInfo;
                TurnManager.Instance.OnPhaseChanged += UpdatePhase;
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTurnStart -= UpdateTurnInfo;
                TurnManager.Instance.OnPhaseChanged -= UpdatePhase;
            }
        }

        private void UpdateTurnInfo(PlayerData player)
        {
            if (currentPlayerText != null)
            {
                currentPlayerText.text = $"현재 플레이어: {player.playerName}";
            }
        }

        private void UpdatePhase(GamePhase phase)
        {
            if (phaseText != null)
            {
                string phaseKorean = phase switch
                {
                    GamePhase.GameStart => "게임 시작",
                    GamePhase.TurnStart => "턴 시작",
                    GamePhase.RollingDice => "주사위 굴리기",
                    GamePhase.Moving => "이동 중",
                    GamePhase.InRoom => "방 안",
                    GamePhase.Suggesting => "제안 중",
                    GamePhase.Accusing => "고발 중",
                    GamePhase.TurnEnd => "턴 종료",
                    GamePhase.GameEnd => "게임 종료",
                    _ => phase.ToString()
                };

                phaseText.text = $"페이즈: {phaseKorean}";
            }

            // 남은 이동 횟수 표시
            if (remainingMovesText != null && phase == GamePhase.Moving)
            {
                int moves = TurnManager.Instance.RemainingMoves;
                remainingMovesText.text = $"남은 이동: {moves}";
            }
            else if (remainingMovesText != null)
            {
                remainingMovesText.text = "";
            }
        }
    }
}