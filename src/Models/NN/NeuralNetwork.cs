using System;
using System.Collections.Generic;

namespace ExpertSystem.Models.NN
{
    public class NeuralNetwork
    {
        public List<NnLayer> Layers { get; set; }
        public double LearningRate { get; set; }
        public int LayerCount => Layers.Count;

        public NeuralNetwork(double learningRate, int[] layers)
        {
            if (layers.Length < 2) return;

            LearningRate = learningRate;
            Layers = new List<NnLayer>();

            for (var l = 0; l < layers.Length; l++)
            {
                var layer = new NnLayer(layers[l]);
                Layers.Add(layer);

                for (var n = 0; n < layers[l]; n++)
                    layer.Neurons.Add(new Neuron());

                layer.Neurons.ForEach(nn =>
                {
                    if (l == 0)
                        nn.Bias = 0;
                    else
                        for (var d = 0; d < layers[l - 1]; d++)
                            nn.Dendrites.Add(new Dendrite());
                });
            }
        }

        private static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public double[] Run(List<double> input)
        {
            if (input.Count != Layers[0].NeuronCount) return null;

            for (var l = 0; l < Layers.Count; l++)
            {
                var layer = Layers[l];
                for (var n = 0; n < layer.Neurons.Count; n++)
                {
                    var neuron = layer.Neurons[n];

                    if (l == 0)
                        neuron.Value = input[n];
                    else
                    {
                        neuron.Value = 0;
                        for (var np = 0; np < Layers[l - 1].Neurons.Count; np++)
                            neuron.Value = neuron.Value +
                                           Layers[l - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;

                        neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                    }
                }
            }

            var last = Layers[Layers.Count - 1];
            var numOutput = last.Neurons.Count;
            var output = new double[numOutput];
            for (var i = 0; i < last.Neurons.Count; i++)
                output[i] = last.Neurons[i].Value;

            return output;
        }

        public bool Train(List<double> input, List<double> output)
        {
            if (input.Count != Layers[0].Neurons.Count ||
                output.Count != Layers[Layers.Count - 1].Neurons.Count) return false;

            Run(input);

            for (var i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++)
            {
                var neuron = Layers[Layers.Count - 1].Neurons[i];
                neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[i] - neuron.Value);

                for (var j = Layers.Count - 2; j > 2; j--)
                {
                    for (var k = 0; k < Layers[j].Neurons.Count; k++)
                    {
                        var n = Layers[j].Neurons[k];

                        n.Delta = n.Value *
                                  (1 - n.Value) *
                                  Layers[j + 1].Neurons[i].Dendrites[k].Weight *
                                  Layers[j + 1].Neurons[i].Delta;
                    }
                }
            }

            for (var i = Layers.Count - 1; i > 1; i--)
            {
                for (var j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    var n = Layers[i].Neurons[j];
                    n.Bias = n.Bias + LearningRate * n.Delta;

                    for (var k = 0; k < n.Dendrites.Count; k++)
                        n.Dendrites[k].Weight = n.Dendrites[k].Weight +
                                                LearningRate * Layers[i - 1].Neurons[k].Value * n.Delta;
                }
            }

            return true;
        }
    }
}