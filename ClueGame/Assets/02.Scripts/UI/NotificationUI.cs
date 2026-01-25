using UnityEngine;
using TMPro;
using System.Collections;

namespace ClueGame.UI
{
    public class NotificationUI : MonoBehaviour
    {
        public static NotificationUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;

        [Header("Settings")]
        [SerializeField] private float displayDuration = 2f;

        private Coroutine currentNotification;

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
            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
        }

        // 알림 표시
        public void ShowNotification(string message)
        {
            if (currentNotification != null)
            {
                StopCoroutine(currentNotification);
            }

            currentNotification = StartCoroutine(ShowNotificationCoroutine(message));
        }

        private IEnumerator ShowNotificationCoroutine(string message)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
            }

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(true);
            }

            yield return new WaitForSeconds(displayDuration);

            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }

            currentNotification = null;
        }
    }
}