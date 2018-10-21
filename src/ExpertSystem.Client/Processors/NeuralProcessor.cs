using ExpertSystem.Client.Models;
using ExpertSystem.Client.Models.ANFIS;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Client.Processors
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

            var socketType = typeof(CustomSocket);
            var socket = new CustomSocket();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(fact.Value, socket);

            return _network.Process(socket);
        }
    }
}