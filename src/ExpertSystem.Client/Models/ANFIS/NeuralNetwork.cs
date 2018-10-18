using System.Collections.Generic;
using ExpertSystem.Common.Models;
using ExpertSystem.Client.Models.FuzzyLogic;

namespace ExpertSystem.Client.Models.ANFIS
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
            var statementNeurons = new List<Neuron>();
            var ruleNeurons = new List<KeyValuePair<FuzzyRule, Neuron>>();

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
                var statementNeuron = new Neuron();
                statementNeurons.Add(statementNeuron);
                resultNeuron.PassthroughDendrites.Add(new Dendrite(statementNeuron, 1d));
                foreach (var neuron in activationLayer.Neurons)
                    neuron.Dendrites.Add(new Dendrite(statementNeuron, 1d));

                var ruleDictionary = new Dictionary<FuzzyRule, Neuron>();
                foreach(var rule in statement.Rules)
                {
                    if (!ruleDictionary.ContainsKey(rule))
                    {
                        var neuron = new Neuron();
                        ruleDictionary.Add(rule, neuron);
                        ruleNeurons.Add(new KeyValuePair<FuzzyRule, Neuron>(rule, neuron));
                    }
                    statementNeuron.Dendrites.Add(new Dendrite(ruleDictionary[rule], 1d));
                }
            }
            _layers.AddFirst(new StatementNeuralLayer(statementNeurons));
            _layers.AddFirst(new RuleNeuralLayer(ruleNeurons));
        }

        public void Learn(Dictionary<CustomSocket, double> samples)
        {
            foreach (var sample in samples)
            {
                var result = Process(sample.Key);
                var diff = result - sample.Value;
            }
        }

        public double Process(CustomSocket socket)
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