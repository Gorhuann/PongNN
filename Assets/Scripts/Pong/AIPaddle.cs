using System;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Pong
{
    public class AIPaddle : Paddle
    {
        public static NeuralNetwork Brain { get; private set; }
        private Vector2 direction;
        private Ball trackedBall;
        private string savePath;
        private string trainingCountPath;
        public static int TrainingCount { get; private set; }

        [SerializeField] private TextMeshProUGUI aiScoreText;
        [SerializeField] private Button resetButton;
        [SerializeField] private BoxCollider collider;

        private int aiScore = 0;
        public int ballHit = 0;

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
        
        private void OnTriggerEnter2D(Collider2D obj)
        {
            if (obj.name == "Ball")
            {
                ballHit++;
                aiScoreText.text = $"Hit réussi : {ballHit}";
            }
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

        public static void ResetAIBrain()
        {
            Brain = new NeuralNetwork(3, 6, 1);
        }
        public void ResetAI()
        {
            TrainingCount = 0;
            SaveTrainingCount();
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