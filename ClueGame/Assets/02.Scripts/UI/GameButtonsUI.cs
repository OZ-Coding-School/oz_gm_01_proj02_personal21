using ClueGame.Managers;
using ClueGame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ClueGame.UI
{
    public class GameButtonsUI : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button suggestButton;
        [SerializeField] private Button accuseButton;
        [SerializeField] private Button nextTurnButton;

        private void Start()
        {
            // 버튼 클릭 이벤트 등록
            if (suggestButton != null)
                suggestButton.onClick.AddListener(OnSuggestClicked);

            if (accuseButton != null)
                accuseButton.onClick.AddListener(OnAccuseClicked);

            if (nextTurnButton != null)
                nextTurnButton.onClick.AddListener(OnNextTurnClicked);

            // 페이즈 변경 이벤트 구독
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnPhaseChanged += UpdateButtonStates;
            }

            UpdateButtonStates(GamePhase.TurnStart);
        }

        private void OnDestroy()
        {
            if (suggestButton != null)
                suggestButton.onClick.RemoveListener(OnSuggestClicked);

            if (accuseButton != null)
                accuseButton.onClick.RemoveListener(OnAccuseClicked);

            if (nextTurnButton != null)
                nextTurnButton.onClick.RemoveListener(OnNextTurnClicked);

            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnPhaseChanged -= UpdateButtonStates;
            }
        }

        private void OnSuggestClicked()
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            // 이미 제안했는지 확인 
            if (currentPlayer.hasSuggestedThisTurn)
            {
                if (NotificationUI.Instance != null)
                {
                    NotificationUI.Instance.ShowNotification("이미 이번 턴에 제안했습니다!");
                }
                return;
            }

            if (currentPlayer.IsInRoom())
            {
                if (SuggestionUI.Instance != null)
                {
                    SuggestionUI.Instance.ShowSuggestionPanel();
                }
            }
            else
            {
                if (NotificationUI.Instance != null)
                {
                    NotificationUI.Instance.ShowNotification("복도에서는 제안할수 없습니다!");
                }
            }
        }

        private void OnAccuseClicked()
        {
            PlayerData currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            // 이미 고발했는지 확인
            if (currentPlayer.hasAccusedThisTurn)
            {
                if (NotificationUI.Instance != null)
                {
                    NotificationUI.Instance.ShowNotification("이미 이번 턴에 고발했습니다!");
                }
                return;
            }

            if (SuggestionUI.Instance != null)
            {
                SuggestionUI.Instance.ShowAccusationPanel();
            }
        }
        private void OnNextTurnClicked()
        {
            TurnManager.Instance.EndTurn();
        }

        private void UpdateButtonStates(GamePhase phase)
        {
            if (suggestButton != null)
            {
                suggestButton.interactable = (phase == GamePhase.InRoom);
            }

            if (accuseButton != null)
            {
                accuseButton.interactable = (phase == GamePhase.InRoom || phase == GamePhase.Moving);
            }

            if (nextTurnButton != null)
            {
                nextTurnButton.interactable = (phase == GamePhase.InRoom);
            }
        }
    }
}