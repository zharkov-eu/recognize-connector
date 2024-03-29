using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExpertSystem.Aggregator.Models.FuzzyLogic;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator.Models.ANFIS
{
    public class NeuralNetwork
    {
        public bool IsLearned;
        private readonly LinkedList<NeuralLayer> _layers;

        public NeuralNetwork()
        {
            _layers = new LinkedList<NeuralLayer>();
        }

        public NeuralNetwork Initialize(IReadOnlyCollection<FuzzyStatement> statements)
        {
            var resultNeuron = new ResultNeuron();
            var conclusionNeurons = new List<Neuron>();
            var activationNeurons = new List<Neuron>();
            var statementNeurons = new List<Neuron>();
            var ruleDictionary = new Dictionary<FuzzyRule, Neuron>();
            var ruleNeurons = new List<KeyValuePair<FuzzyRule, Neuron>>();

            // Строим 5, 4, 3 уровни нейросети
            foreach (var statement in statements)
            {
                var conclusionNeuron = new Neuron();
                conclusionNeurons.Add(conclusionNeuron);
                resultNeuron.Dendrites.Add(new Dendrite(conclusionNeuron, 0.001d));

                var activationNeuron = new Neuron();
                activationNeurons.Add(activationNeuron);
                conclusionNeuron.Dendrites.Add(new Dendrite(activationNeuron, 0.005d));
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
                resultNeuron.PassthroughDendrites.Add(new Dendrite(statementNeuron, 0.0005d));
                foreach (var neuron in activationLayer.Neurons)
                    neuron.Dendrites.Add(new Dendrite(statementNeuron, 0.00001d));

                foreach (var rule in statement.Rules)
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

            return this;
        }

        public NeuralNetwork Learn(Dictionary<CustomSocket, double> samples)
        {
            IsLearned = true;
            foreach (var sample in samples)
            {
                var result = Process(sample.Key);
                var diff = result - sample.Value;
            }

            return this;
        }

        public double Process(CustomSocket socket)
        {
            foreach (var layer in _layers)
            {
                if (layer.Options.IsSocketLayer)
                    ((NeuralSocketLayer) layer).SetSocket(socket);
                layer.Process();
            }

            var result = ((ResultNeuralLayer) _layers.Last.Value).GetResult;
            return result % 110 > 45 ? result : result + 45 ; 
        }
    }
}