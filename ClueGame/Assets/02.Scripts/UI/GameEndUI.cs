using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace ClueGame.UI
{
    public class GameEndUI : MonoBehaviour
    {
        public static GameEndUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject endPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI answerText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

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
            if (endPanel != null)
            {
                endPanel.SetActive(false);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        // 승리 화면
        public void ShowWin(string playerName, string character, string weapon, string room)
        {
            // 게임 일시정지
            Time.timeScale = 0f;

            if (resultText != null)
            {
                resultText.text = "승리!";
                resultText.color = new Color(0.2f, 0.8f, 0.2f);
            }

            if (messageText != null)
            {
                messageText.text = $"{playerName}이(가) 정답을 맞추었습니다!";
            }

            if (answerText != null)
            {
                answerText.text = $"범인: {character}\n무기: {weapon}\n장소: {room}";
            }

            if (endPanel != null)
            {
                endPanel.SetActive(true);
            }


        }

        // 패배 화면
        public void ShowLose(string character, string weapon, string room)
        {
            // 게임 일시정지
            Time.timeScale = 0f;

            if (resultText != null)
            {
                resultText.text = "패배...";
                resultText.color = new Color(0.8f, 0.2f, 0.2f);
            }

            if (messageText != null)
            {
                messageText.text = "모든 플레이어가 탈락했습니다.";
            }

            if (answerText != null)
            {
                answerText.text = $"정답:\n범인: {character}\n무기: {weapon}\n장소: {room}";
            }

            if (endPanel != null)
            {
                endPanel.SetActive(true);
            }


        }

        // 재시작
        private void OnRestartClicked()
        {
  

            Time.timeScale = 1f;

            // 현재 씬 이름으로 로드
            string currentSceneName = SceneManager.GetActiveScene().name;


            SceneManager.LoadScene(currentSceneName);
        }

        // 종료
        private void OnQuitClicked()
        {
           
            Time.timeScale = 1f; // 게임 속도 복원

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

    }
}