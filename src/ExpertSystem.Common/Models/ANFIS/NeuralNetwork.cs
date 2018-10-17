using System.Collections.Generic;
using ExpertSystem.Models.FuzzyLogic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models.ANFIS
{
    public class NeuralNetwork
    {
        private readonly LinkedList<NeuralLayer> _layers;

        public NeuralNetwork()
        {
            _layers = new LinkedList<NeuralLayer>();
        }

        public void Initialize(List<FuzzyStatement> statements)
        {
            var resultNeuron = new ResultNeuron();
            var conclusionNeurons = new List<Neuron>();
            var activationNeurons = new List<Neuron>();
            var ruleNeurons = new List<Neuron>();
            var inputNeurons = new List<Neuron>();

            // Строим 5, 4, 3 уровни нейросети
            foreach (var statement in statements)
            {
                var conclusionNeuron = new Neuron();
                conclusionNeurons.Add(conclusionNeuron);
                resultNeuron.Dendrites.Add(new Dendrite(conclusionNeuron, 1d));

                var activationNeuron = new Neuron();
                activationNeurons.Add(activationNeuron);
                conclusionNeuron.Dendrites.Add(new Dendrite(activationNeuron, 1d));
            }
            _layers.AddFirst(new ResultNeuralLayer(resultNeuron));
            _layers.AddFirst(new ConclusionNeuralLayer(conclusionNeurons));
            _layers.AddFirst(new ActivationNeuralLayer(activationNeurons));

            // Строим 2, 1 уровни нейросети
            var activationLayer = _layers.First.Value;
            foreach (var statement in statements)
            {
                var ruleNeuron = new Neuron();
                ruleNeurons.Add(ruleNeuron);
                foreach (var neuron in activationLayer.Neurons)
                    neuron.Dendrites.Add(new Dendrite(ruleNeuron, 1d));
            }
            _layers.AddFirst(new RuleNeuralLayer(ruleNeurons));
            _layers.AddFirst(new InputNeuralLayer(inputNeurons));
        }

        public double Process(FuzzyCustomSocket socket)
        {
            foreach (var layer in _layers)
            {
                if (layer.Options.IsSocketLayer)
                    ((NeuralSocketLayer) layer).SetSocket(socket);
                layer.Process();
            }
            return ((ResultNeuralLayer) _layers.Last.Value).GetResult;
        }
    }
}