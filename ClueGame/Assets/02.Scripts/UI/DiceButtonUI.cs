using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClueGame.Managers;

namespace ClueGame.UI
{
    public class DiceButtonUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button diceButton;
        [SerializeField] private TextMeshProUGUI diceResultText;

        private void Start()
        {
            if (diceButton != null)
            {
                diceButton.onClick.AddListener(OnDiceButtonClicked);
            }

            // 주사위 결과 이벤트 구독
            if (DiceManager.Instance != null)
            {
                DiceManager.Instance.OnDiceRolled += ShowDiceResult;
            }

            // 페이즈 변경 이벤트 구독
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnPhaseChanged += UpdateButtonState;
            }

            UpdateButtonState(GamePhase.TurnStart);
        }

        private void OnDestroy()
        {
            if (diceButton != null)
            {
                diceButton.onClick.RemoveListener(OnDiceButtonClicked);
            }

            if (DiceManager.Instance != null)
            {
                DiceManager.Instance.OnDiceRolled -= ShowDiceResult;
            }

            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnPhaseChanged -= UpdateButtonState;
            }
        }

        private void OnDiceButtonClicked()
        {
            if (TurnManager.Instance.GetCurrentPhase() == GamePhase.RollingDice)
            {
                TurnManager.Instance.RollDice();
            }
        }

        private void ShowDiceResult(int result)
        {
            if (diceResultText != null)
            {
                diceResultText.text = $"주사위: {result}";
            }
        }

        private void UpdateButtonState(GamePhase phase)
        {
            if (diceButton != null)
            {
                diceButton.interactable = (phase == GamePhase.RollingDice);
            }
        }
    }
}