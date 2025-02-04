using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pong
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Ball ball;
        [SerializeField] private Paddle playerPaddle;
        [SerializeField] private AIPaddle aiPaddle;
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI aiScoreText;
        [SerializeField] private TextMeshProUGUI trainingCountText;
        [SerializeField] private Button resetButton;

        private int playerScore;
        private int aiScore;
        private float roundStartTime;

        private void Start()
        {
            NewGame();
            if (resetButton != null)
                resetButton.onClick.AddListener(ResetGame);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetGame();
            }
        }

        public void NewGame()
        {
            SetPlayerScore(0);
            SetAIScore(0);
            NewRound();
        }

        public void NewRound()
        {
            playerPaddle.ResetPosition();
            aiPaddle.ResetPosition();
            ball.ResetPosition();
            roundStartTime = Time.time;
            Invoke(nameof(StartRound), 1f);
        }

        private void StartRound()
        {
            ball.AddStartingForce();
        }

        public void OnPlayerScored()
        {
            float survivalTime = Time.time - roundStartTime;
            SetPlayerScore(playerScore + 1);
            aiPaddle.Brain.AdjustWeights(-0.1f + survivalTime * 0.01f);
            aiPaddle.IncrementTrainingCount();
            UpdateTrainingCountUI();
            NewRound();
        }

        public void OnAIScored()
        {
            SetAIScore(aiScore + 1);
            aiPaddle.Brain.AdjustWeights(0.1f);
            aiPaddle.IncrementTrainingCount();
            UpdateTrainingCountUI();
            NewRound();
        }

        private void SetPlayerScore(int score)
        {
            playerScore = score;
            if (playerScoreText != null)
                playerScoreText.text = "Player Score: " + score;
        }

        private void SetAIScore(int score)
        {
            aiScore = score;
            if (aiScoreText != null)
                aiScoreText.text = "AI Score: " + score;
        }

        private void UpdateTrainingCountUI()
        {
            if (trainingCountText != null)
                trainingCountText.text = "Training Count: " + aiPaddle.TrainingCount;
        }

        private void ResetGame()
        {
            aiPaddle.ResetAI();
            NewGame();
            UpdateTrainingCountUI();
        }
    }
}