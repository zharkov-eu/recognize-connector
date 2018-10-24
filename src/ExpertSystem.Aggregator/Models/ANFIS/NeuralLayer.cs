using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Aggregator.Models.FuzzyLogic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Common.Utils;

namespace ExpertSystem.Aggregator.Models.ANFIS
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
        public readonly List<Neuron> Neurons;
        public NeuralLayerOptions Options;

        protected NeuralLayer(List<Neuron> neurons, NeuralLayerOptions options)
        {
            Neurons = neurons;
            Options = options;
        }

        public abstract void Process();
    }

    public abstract class NeuralSocketLayer : NeuralLayer
    {
        protected CustomSocket Socket;

        protected NeuralSocketLayer(List<Neuron> neurons, NeuralLayerOptions options)
            : base(neurons, options)
        {
            Options.IsSocketLayer = true;
        }

        public void SetSocket(CustomSocket socket)
        {
            Socket = socket;
        }
    }

    public class RuleNeuralLayer : NeuralSocketLayer
    {
        private readonly Dictionary<Neuron, InputDegreeParam> _params;
        private readonly Dictionary<FuzzyRule, Neuron> _domainNeurons;

        public RuleNeuralLayer(List<KeyValuePair<FuzzyRule, Neuron>> domainNeurons)
            : base(domainNeurons.Select(p => p.Value).ToList(), new NeuralLayerOptions {IsFixed = false})
        {
            _params = new Dictionary<Neuron, InputDegreeParam>();
            _domainNeurons = new Dictionary<FuzzyRule, Neuron>();

            foreach (var domainNeuron in domainNeurons)
            {
                var neuron = domainNeuron.Value;
                _domainNeurons[domainNeuron.Key] = neuron;
                _params[neuron] = new InputDegreeParam
                {
                    a = RandUtil.RandDoubleRange(1, 2),
                    b = RandUtil.RandDoubleRange(1, 2),
                    c = RandUtil.RandDoubleRange(1, 2)
                };
            }
        }

        public override void Process()
        {
            foreach (var domainNeuron in _domainNeurons)
            {
                var neuron = domainNeuron.Value;
                var param = _params[neuron];
                var domain = domainNeuron.Key.Domain.Domain;
                var inputValue = Convert.ToDouble(
                    CustomSocketExtension.SocketType.GetProperty(domain.ToString()).GetValue(Socket)
                );
                neuron.Value = 1 / (1 + Math.Pow(Math.Abs((inputValue - param.c) / param.a), 2 * param.b));
            }
        }
    }

    public class StatementNeuralLayer : NeuralLayer
    {
        public StatementNeuralLayer(List<Neuron> neurons)
            : base(neurons, new NeuralLayerOptions {IsFixed = true})
        {
        }

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
            : base(neurons, new NeuralLayerOptions {IsFixed = true})
        {
        }

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

            foreach (var neuron in Neurons)
                _params.Add(neuron, new ConclusionDegreeParam
                {
                    p = RandUtil.RandDoubleRange(1, 2),
                    q = RandUtil.RandDoubleRange(1, 2),
                    r = RandUtil.RandDoubleRange(1, 2),
                    s = RandUtil.RandDoubleRange(1, 2),
                });
        }

        public override void Process()
        {
            foreach (var neuron in Neurons)
            {
                var param = _params[neuron];
                var weight = neuron.Dendrites[0].Weight;
                var value = param.p * Socket.NumberOfContacts;
                value += param.q * Socket.SizeLength;
                value += param.r * Socket.SizeWidth;
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