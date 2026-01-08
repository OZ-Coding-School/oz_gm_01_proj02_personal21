using UnityEngine;
using UnityEngine.UI;
using ClueGame.Managers;

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
            Debug.Log("제안하기 버튼 클릭 - 제안 UI 열기 예정");
            // TODO: 제안 UI 열기 (내일 구현)
        }

        private void OnAccuseClicked()
        {
            Debug.Log("고발하기 버튼 클릭 - 고발 UI 열기 예정");
            // TODO: 고발 UI 열기 (내일 구현)
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