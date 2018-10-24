using ExpertSystem.Aggregator.Models.ANFIS;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Processors
{
    public class NeuralProcessor : AbstractProcessor
    {
        private readonly NeuralRulesGenerator _generator;
        private readonly NeuralNetwork _network;

        public NeuralProcessor(NeuralNetwork network, ProcessorOptions options)
            : base(options)
        {
            _network = network;
            _generator = new NeuralRulesGenerator();
        }

        public double Process(FactSet factSet)
        {
            if (!_network.IsLearned)
                _generator.LearnNeuralNetwork(_network);

            var socket = new CustomSocket();
            foreach (var fact in factSet)
                CustomSocketExtension.SocketType.GetProperty(fact.Domain.ToString()).SetValue(fact.Value, socket);

            return _network.Process(socket);
        }
    }
}