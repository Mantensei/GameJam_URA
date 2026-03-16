using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GameJam_URA.Prototype
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] GameObject clearPanel;
        [SerializeField] GameObject gameOverPanel;
        [SerializeField] Button retryButton;

        void Start()
        {
            clearPanel.SetActive(false);
            gameOverPanel.SetActive(false);

            GameManager.Instance.OnStateChanged += OnStateChanged;

            if (retryButton != null)
                retryButton.onClick.AddListener(Retry);
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= OnStateChanged;
        }

        void OnStateChanged(GameState state)
        {
            clearPanel.SetActive(state == GameState.Clear);
            gameOverPanel.SetActive(state == GameState.GameOver);
        }

        void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
