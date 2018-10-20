using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Generated;
using ExpertSystem.Client.Models.FuzzyLogic;
using ExpertSystem.Common.Utils;

namespace ExpertSystem.Client.Models.ANFIS
{
    public struct NeuralLayerOptions
    {
        public bool IsFixed;
        public bool IsSocketLayer;
    }

    public struct InputDegreeParam
    {
        public double a;
        public double b;
        public double c;
    }

    public struct ConclusionDegreeParam
    {
        public double p;
        public double q;
        public double r;
        public double s;
    }

    public abstract class NeuralLayer
    {
        public List<Neuron> Neurons;
        public NeuralLayerOptions Options;

        public NeuralLayer(List<Neuron> neurons, NeuralLayerOptions options)
        {
            Neurons = neurons;
            Options = options;
        }

        public abstract void Process();
    }

    public abstract class NeuralSocketLayer : NeuralLayer
    {
        protected CustomSocket _socket;

        public NeuralSocketLayer(List<Neuron> neurons, NeuralLayerOptions options)
            : base(neurons, options)
        {
            Options.IsSocketLayer = true;
        }

        public void SetSocket(CustomSocket socket)
        {
            _socket = socket;
        }
    }

    public class RuleNeuralLayer : NeuralSocketLayer
    {
        private InputDegreeParam _param;
        private Dictionary<FuzzyRule, List<Neuron>> _domainNeurons;

        public RuleNeuralLayer(List<KeyValuePair<FuzzyRule, Neuron>> domainNeurons)
            : base(domainNeurons.Select(p => p.Value).ToList(), new NeuralLayerOptions { IsFixed = false })
        {
            var r = new CryptoRandom();
            _param = new InputDegreeParam { a = r.RandomValue, b = r.RandomValue, c = r.RandomValue };
            _domainNeurons = new Dictionary<FuzzyRule, List<Neuron>>();

            foreach (var domainNeuron in domainNeurons)
            {
                if (_domainNeurons.ContainsKey(domainNeuron.Key) == false)
                    _domainNeurons[domainNeuron.Key] = new List<Neuron>();
                _domainNeurons[domainNeuron.Key].Add(domainNeuron.Value);
            }
        }

        public override void Process()
        {
        }
    }

    public class StatementNeuralLayer : NeuralLayer
    {
        public StatementNeuralLayer(List<Neuron> neurons)
            : base(neurons, new NeuralLayerOptions { IsFixed = true }) {}

        public override void Process()
        {
            foreach (var neuron in Neurons)
            {
                neuron.Value = neuron.Dendrites
                    .Select(p => p.Weight * p.Neuron.Value)
                    .Aggregate((acc, p) => acc * p);
            }
        }
    }

    public class ActivationNeuralLayer : NeuralLayer
    {
        public ActivationNeuralLayer(List<Neuron> neurons)
            : base(neurons, new NeuralLayerOptions { IsFixed = true }) {}
        
        public override void Process()
        {
            var dendritesSum = 0d;
            var dendritesValue = new Dictionary<Neuron, double>();
            foreach (var neuron in Neurons)
            {
                var value = neuron.Dendrites
                    .Select(p => p.Weight * p.Neuron.Value)
                    .Aggregate((acc, p) => acc + p);
                dendritesValue.Add(neuron, value);
                dendritesSum += value;
            }
            
            foreach (var neuron in Neurons)
                neuron.Value = dendritesValue[neuron] / dendritesSum;
        }
    }

    public class ConclusionNeuralLayer : NeuralSocketLayer
    {
        private readonly Dictionary<Neuron, ConclusionDegreeParam> _params;

        public ConclusionNeuralLayer(List<Neuron> neurons)
            : base(neurons, new NeuralLayerOptions {IsFixed = false})
        {
            _params = new Dictionary<Neuron, ConclusionDegreeParam>();

            var r = new CryptoRandom();
            foreach (var neuron in Neurons)
                _params.Add(neuron, new ConclusionDegreeParam
                {
                    p = r.RandomValue,
                    q = r.RandomValue,
                    r = r.RandomValue,
                    s = r.RandomValue
                });
        }

        public override void Process()
        {
            foreach (var neuron in Neurons)
            {
                var param = _params[neuron];
                var weight = neuron.Dendrites[0].Weight;
                var value = param.p * _socket.NumberOfContacts;
                value += param.q * _socket.SizeLength;
                value += param.r * _socket.SizeWidth;
                value += param.s;

                neuron.Value = weight * value;
            }
        }
    }

    public class ResultNeuralLayer : NeuralLayer
    {
        private readonly ResultNeuron _result;

        public ResultNeuralLayer(ResultNeuron neuron)
            : base(new List<Neuron> {neuron}, new NeuralLayerOptions {IsFixed = true})
        {
            _result = neuron;
        }

        public double GetResult => _result.Value;

        public override void Process()
        {
            var numerator = _result.Dendrites.Select(p => p.Weight * p.Neuron.Value).Sum();
            var denumerator = _result.PassthroughDendrites.Select(p => p.Neuron.Value).Sum();
            _result.Value = numerator / denumerator;
        }
    }
}