using System;
using System.Collections.Generic;
using ExpertSystem.Utils;

namespace ExpertSystem.Models.ANFIS
{
    public class Dendrite
    {
        public Neuron Neuron;
        public double Weight;

        public Dendrite(Neuron neuron, double weight = default(double))
        {
            Neuron = neuron;
            if (weight == default(double))
                Weight = new CryptoRandom().RandomValue;
            else
                Weight = weight;
        }
    }

    public class Neuron
    {
        public List<Dendrite> Dendrites;
        public double Value;

        public Neuron()
        {
            Dendrites = new List<Dendrite>();
        }
    }

    public class ResultNeuron : Neuron
    {
        public List<Dendrite> PassthroughDendrites;

        public ResultNeuron()
        {
            Dendrites = new List<Dendrite>();
            PassthroughDendrites = new List<Dendrite>();
        }
    }
}
