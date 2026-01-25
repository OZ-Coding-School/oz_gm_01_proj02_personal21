using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClueGame.Managers;
using System.Collections;

namespace ClueGame.UI
{
    public class DiceButtonUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button diceButton;
        [SerializeField] private TextMeshProUGUI diceResultText;
        [SerializeField] private Image diceImage;

        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;

        private void Start()
        {
            if (diceButton != null)
            {
                diceButton.onClick.AddListener(OnDiceButtonClicked);
            }

            if (DiceManager.Instance != null)
            {
                DiceManager.Instance.OnDiceRolled += ShowDiceResult;
            }

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
                StartCoroutine(AnimateDiceRoll());
            }
        }

        private IEnumerator AnimateDiceRoll()
        {
            // 굴리는 애니메이션
            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                if (diceResultText != null)
                {
                    diceResultText.text = $"주사위: {Random.Range(1, 7)}";
                }

                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            // 실제 주사위 굴리기
            TurnManager.Instance.RollDice();
        }

        private void ShowDiceResult(int result)
        {
            if (diceResultText != null)
            {
                diceResultText.text = $"주사위: {result}";

                // 텍스트 펄스 애니메이션
                StartCoroutine(PulseText());
            }
        }

        private IEnumerator PulseText()
        {
            if (diceResultText == null) yield break;

            Vector3 originalScale = diceResultText.transform.localScale;
            Vector3 targetScale = originalScale * 1.3f;

            float elapsed = 0f;
            while (elapsed < 0.2f)
            {
                diceResultText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / 0.2f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < 0.2f)
            {
                diceResultText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / 0.2f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            diceResultText.transform.localScale = originalScale;
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