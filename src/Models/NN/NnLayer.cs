using System.Collections.Generic;

namespace ExpertSystem.Models.NN
{
    public class NnLayer
    {
        public List<Neuron> Neurons { get; set; }
        public int NeuronCount => Neurons.Count;

        public NnLayer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
        }
    }
}
