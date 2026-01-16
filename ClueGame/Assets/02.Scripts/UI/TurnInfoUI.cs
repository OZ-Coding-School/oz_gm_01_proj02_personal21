using ClueGame.Data;
using ClueGame.Managers;
using ClueGame.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                    GamePhase.InRoom => GetRoomPhaseText(), 
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

        // 방 이름 가져오기 
        private string GetRoomPhaseText()
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            if (currentPlayer != null && currentPlayer.IsInRoom())
            {
                string roomName = GetRoomNameInKorean(currentPlayer.currentRoom.Value);
                return $"방 안 ({roomName})";
            }
            else if (!currentPlayer.IsInRoom())
            {
                return $"복도";
            }

            return "방 안";
        }

        // 방 이름 한글
        private string GetRoomNameInKorean(RoomCard room)
        {
            return room switch
            {
                RoomCard.부엌 => "부엌",
                RoomCard.무도회장 => "무도회장",
                RoomCard.온실 => "온실",
                RoomCard.식당 => "식당",
                RoomCard.당구장 => "당구장",
                RoomCard.도서관 => "도서관",
                RoomCard.라운지 => "라운지",
                RoomCard.홀 => "홀",
                RoomCard.서재 => "서재",
                _ => room.ToString()
            };
        }
    }
}