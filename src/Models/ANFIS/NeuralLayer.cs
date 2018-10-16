using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Utils;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models.ANFIS
{
    public struct NeuralLayerOptions
    {
        public bool IsFixed;
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
        protected FuzzyCustomSocket _socket;

        public NeuralSocketLayer(List<Neuron> neurons, NeuralLayerOptions options)
            : base(neurons, options) {}

        public void SetSocket(FuzzyCustomSocket socket)
        {
            _socket = socket;
        }
    }

    public class InputNeuralLayer : NeuralSocketLayer
    {
        private InputDegreeParam _param;

        public InputNeuralLayer(List<Neuron> neurons)
            : base(neurons, new NeuralLayerOptions { IsFixed = false })
        {
            var r = new CryptoRandom();
            _param = new InputDegreeParam { a = r.RandomValue, b = r.RandomValue, c = r.RandomValue };
        }

        public override void Process()
        {
            
        }
    }

    public class ConclusionNeuralLayer : NeuralSocketLayer
    {
        private Dictionary<Neuron, ConclusionDegreeParam> _params;

        public ConclusionNeuralLayer(List<Neuron> neurons)
            : base(neurons, new NeuralLayerOptions { IsFixed = false })
        {
            _params = new Dictionary<Neuron, ConclusionDegreeParam>();
            var r = new CryptoRandom();
            foreach (var neuron in Neurons)
                _params.Add(neuron, new ConclusionDegreeParam {
                    p = r.RandomValue, q = r.RandomValue, r = r.RandomValue, s = r.RandomValue
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
            : base(new List<Neuron> { neuron }, new NeuralLayerOptions { IsFixed = true })
        {
            _result = neuron;
        }

        public override void Process()
        {
            var numerator = _result.Dendrites.Select(p => p.Weight * p.Neuron.Value).Sum();
            var denumerator = _result.PassthroughDendrites.Select(p => p.Neuron.Value).Sum();
            _result.Value = numerator / denumerator;
        }

        public double GetResult => _result.Value;
    }
}
