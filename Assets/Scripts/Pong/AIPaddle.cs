using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pong
{
    public class AIPaddle : Paddle
    {
        public NeuralNetwork Brain { get; private set; }
        private Vector2 direction;
        private Ball trackedBall;
        private string savePath;
        private string trainingCountPath;
        public int TrainingCount { get; private set; }

        [SerializeField] private Text aiScoreText;
        [SerializeField] private Button resetButton;

        private int aiScore = 0;

        private void Start()
        {
            Brain = new NeuralNetwork(3, 6, 1);
            trackedBall = FindObjectOfType<Ball>();
            savePath = Application.persistentDataPath + "/ai_weights.json";
            trainingCountPath = Application.persistentDataPath + "/training_count.json";
            LoadWeights();
            LoadTrainingCount();

            if (resetButton != null)
                resetButton.onClick.AddListener(ResetAI);
        }

        private void FixedUpdate()
        {
            if (trackedBall != null)
            {
                float[] inputs = new float[]
                {
                    trackedBall.transform.position.y,
                    trackedBall.rb.velocity.y,
                    transform.position.y
                };

                float output = Brain.FeedForward(inputs)[0];
                direction = output > 0.5f ? Vector2.up : Vector2.down;
                rb.AddForce(direction * speed);
            }
        }

        public void ResetAI()
        {
            Brain = new NeuralNetwork(3, 6, 1);
            TrainingCount = 0;
            SaveTrainingCount();
            if (aiScoreText != null)
                aiScoreText.text = "AI Score: 0";
        }

        public void IncrementTrainingCount()
        {
            TrainingCount++;
            SaveTrainingCount();
        }

        private void SaveTrainingCount()
        {
            File.WriteAllText(trainingCountPath, TrainingCount.ToString());
        }

        private void LoadTrainingCount()
        {
            if (File.Exists(trainingCountPath))
            {
                int count;
                if (int.TryParse(File.ReadAllText(trainingCountPath), out count))
                    TrainingCount = count;
            }
        }

        private void LoadWeights()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                Brain.SetWeights(JsonConvert.DeserializeObject<float[][]>(json));
            }
        }
    }
}