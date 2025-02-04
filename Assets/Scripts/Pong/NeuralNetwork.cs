using UnityEngine;

namespace Pong
{
    public class NeuralNetwork
    {
        private float[][] weights;
        private System.Random random = new System.Random();

        public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
        {
            weights = new float[2][];
            weights[0] = new float[inputSize * hiddenSize];
            weights[1] = new float[hiddenSize * outputSize];
            InitializeWeights();
        }

        private void InitializeWeights()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] = (float)(random.NextDouble() * 2 - 1);
                }
            }
        }

        public float[] FeedForward(float[] inputs)
        {
            float[] outputs = new float[1];
            outputs[0] = inputs[0] * weights[0][0] + inputs[1] * weights[0][1] + inputs[2] * weights[0][2];
            return outputs;
        }

        public void AdjustWeights(float reward)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] += reward * 0.01f;
                }
            }
        }

        public float[][] GetWeights()
        {
            return weights;
        }

        public void SetWeights(float[][] newWeights)
        {
            weights = newWeights;
        }
    }
}