using System;
using System.Collections.Generic;
using ExpertSystem.Utils;

namespace ExpertSystem.Models.NN
{
    public class Dendrite
    {
        public double Weight { get; set; }

        public Dendrite()
        {
            var n = new CryptoRandom();
            Weight = n.RandomValue;
        }
    }

    public class Neuron
    {
        public List<Dendrite> Dendrites { get; set; }
        public double Bias { get; set; }
        public double Delta { get; set; }
        public double Value { get; set; }

        public int DendriteCount => Dendrites.Count;

        public Neuron()
        {
            var n = new Random(Environment.TickCount);
            Bias = n.NextDouble();
            Dendrites = new List<Dendrite>();
        }
    }
}
