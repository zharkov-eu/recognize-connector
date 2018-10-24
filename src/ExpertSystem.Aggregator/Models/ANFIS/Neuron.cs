using System;
using System.Collections.Generic;
using ExpertSystem.Common.Utils;

namespace ExpertSystem.Aggregator.Models.ANFIS
{
    public class Dendrite
    {
        public readonly Neuron Neuron;
        public double Weight;

        public Dendrite(Neuron neuron, double weight = default(double))
        {
            Neuron = neuron;
            Weight = Math.Abs(weight - default(double)) < 0.0001 ? new CryptoRandom().RandomValue : weight;
        }
    }

    public class Neuron
    {
        public readonly List<Dendrite> Dendrites;
        public double Value;

        public Neuron()
        {
            Dendrites = new List<Dendrite>();
        }
    }

    public class ResultNeuron : Neuron
    {
        public readonly List<Dendrite> PassthroughDendrites;

        public ResultNeuron()
        {
            PassthroughDendrites = new List<Dendrite>();
        }
    }
}