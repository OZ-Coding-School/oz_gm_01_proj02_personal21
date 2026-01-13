using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ClueGame.Data;

namespace ClueGame.UI
{
    public class CardRevealUI : MonoBehaviour
    {
        public static CardRevealUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject revealPanel;
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI revealerNameText;
        [SerializeField] private Button closeButton;

        [Header("Animation Settings")]
        [SerializeField] private float flipDuration = 0.5f;
        [SerializeField] private float displayDuration = 2f;

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
            if (revealPanel != null)
            {
                revealPanel.SetActive(false);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseReveal);
            }
        }

        public void ShowCardReveal(string revealerName, Card card)
        {
            StartCoroutine(CardRevealAnimation(revealerName, card));
        }

        private IEnumerator CardRevealAnimation(string revealerName, Card card)
        {
            if (revealPanel != null)
            {
                revealPanel.SetActive(true);
            }

            if (revealerNameText != null)
            {
                revealerNameText.text = $"{revealerName}이(가) 카드를 공개합니다!";
            }

            // 카드 뒤집기 애니메이션
            if (cardImage != null)
            {
                // 1. 카드 축소 (뒤집는 효과)
                float elapsed = 0f;
                Vector3 originalScale = cardImage.transform.localScale;

                while (elapsed < flipDuration / 2)
                {
                    float scaleX = Mathf.Lerp(1f, 0f, elapsed / (flipDuration / 2));
                    cardImage.transform.localScale = new Vector3(scaleX, 1f, 1f);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                // 카드 정보 표시
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName;
                }

                // 2. 카드 확대 (뒤집힌 상태)
                elapsed = 0f;
                while (elapsed < flipDuration / 2)
                {
                    float scaleX = Mathf.Lerp(0f, 1f, elapsed / (flipDuration / 2));
                    cardImage.transform.localScale = new Vector3(scaleX, 1f, 1f);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                cardImage.transform.localScale = originalScale;
            }

            // 잠시 표시
            yield return new WaitForSeconds(displayDuration);

            // 자동으로 닫기
            CloseReveal();
        }

        private void CloseReveal()
        {
            if (revealPanel != null)
            {
                revealPanel.SetActive(false);
            }
        }
    }
}