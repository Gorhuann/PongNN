using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Pong
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Ball ball;
        [SerializeField] private PlayerPaddle playerPaddle;
        [SerializeField] private AIPaddle computerPaddle;
        [SerializeField] private TextMeshProUGUI playerScoreText;
        [SerializeField] private TextMeshProUGUI aiScoreText;
        [SerializeField] private TextMeshProUGUI trainingCountText;
        [SerializeField] private Button resetButton;
        [SerializeField] private int nbAiPaddles;

        [SerializeField] private List<AIPaddle> AIPaddles;

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
            NewRound();
        }

        public void NewRound()
        {
            playerPaddle.ResetPosition();
            computerPaddle.GameObject().SetActive(true);
            Debug.Log("ouais la télé");
            for (int i = 0; i < nbAiPaddles; i++)
            {
                AIPaddle aiPaddle = Instantiate(computerPaddle);
                AIPaddles.Add(aiPaddle);
                aiPaddle.ResetPosition();
                aiPaddle.ballHit = 0;
            }
            computerPaddle.GameObject().SetActive(false);
            ball.ResetPosition();
            aiScoreText.text = $"Hit réussi : 0";
            roundStartTime = Time.time;
            Invoke(nameof(StartRound), 1f);
        }

        private void StartRound()
        {
            ball.AddStartingForce();
        }

        public void OnPlayerScored()
        {
            Debug.Log(playerPaddle.ballHit);
            foreach (var paddle in AIPaddles)
            {
                if (AIPaddles.Count == 1)
                {
                    float survivalTime = Time.time - roundStartTime;
                    Debug.Log($"Reward : {(-0.1f + survivalTime * 0.01f) * AIPaddles[0].ballHit}");
                    AIPaddle.Brain.AdjustWeights((-0.1f + survivalTime * 0.01f) * AIPaddles[0].ballHit);
                    AIPaddles[0].IncrementTrainingCount();
                    UpdateTrainingCountUI();
                }
                if (paddle.ballHit < playerPaddle.ballHit)
                {
                    Destroy(paddle.GameObject());
                }
            }

            if (AIPaddles.Count == 0)
            {
                NewRound();
            }
        }

        /*public void OnAIScored()
        {
            aiPaddle.Brain.AdjustWeights(0.1f);
            aiPaddle.IncrementTrainingCount();
            UpdateTrainingCountUI();
            NewRound();
        }*/

        private void SetPlayerScore(int score)
        {
            playerScore = score;
            if (playerScoreText != null)
                playerScoreText.text = "Player Score: " + score;
        }

        private void UpdateTrainingCountUI()
        {
            if (trainingCountText != null)
                trainingCountText.text = "Training Count: " + AIPaddle.TrainingCount;
        }

        private void ResetGame()
        {
            foreach (var paddle in AIPaddles)
            {
                Destroy(paddle.GameObject());
            }
            AIPaddle.ResetAIBrain();
            NewGame();
            UpdateTrainingCountUI();
        }
    }
}