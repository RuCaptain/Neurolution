using System;
using System.Collections.Generic;
using System.Linq;
using RecurrentNetworkLibrary.ActivationFunctions;

namespace RecurrentNetworkLibrary
{
    public class RecurrentNetwork
    {
        public List<Layer> Layers = new List<Layer>();
        public float LearningRate = 0.2f;
        public int MemorySize = 10;
        private readonly Layer _inputLayer;
        private readonly Layer _outputLayer;
        private readonly Layer _recurrentLayer;
        private List<float> _lastOutputs;
        private List<float> _lastInputs; 
        private bool _isInitialized;
        private float _startRange;
        private float _endRange;
        private readonly List<float> _memory = new List<float>(); 

        public RecurrentNetwork(int inputNeuronsCount, int outputNeuronsCount, int hiddenLayersCount, int hiddenNeuronsCount, float threshold = 0.5f)
        {
            //Initizating input layer
            _inputLayer = new Layer(inputNeuronsCount, new LinearFunction(), null, null);
            Layers.Add(_inputLayer);

            //Initizating hidden layers
            var hiddenLayers = new List<Layer>();
            for (var i = 0; i < hiddenLayersCount; i++)
            {
                var hiddenLayer = new Layer(hiddenNeuronsCount, new SigmoidFunction(), Layers.Last(), null);
                Layers.Last().SetOutputLayer(hiddenLayer);
                hiddenLayers.Add(hiddenLayer);
            }
            Layers.AddRange(hiddenLayers);

            //Initizating output layer
            _outputLayer = new Layer(outputNeuronsCount, new ThresholdFunction(threshold), Layers.Last(), null);
            Layers.Last().SetOutputLayer(_outputLayer);
            Layers.Add(_outputLayer);

            //Initizating recurrent layer
            _recurrentLayer = new Layer(0, new LinearFunction(), null, hiddenLayers[0]);
            hiddenLayers[0].SetRecurrentLayer(_recurrentLayer);
            Layers.Add(_recurrentLayer);

        }

        //Setting initial synapse weights
        public void Initialize(float startRange, float endRange)
        {
            foreach(var layer in Layers)
                layer.Initialize(startRange, endRange);
            _isInitialized = true;
            _startRange = startRange;
            _endRange = endRange;
        }

        //Running network
        public IEnumerable<float> Run(IEnumerable<float> inputs)
        {
            if (!_isInitialized)
                throw new Exception("Network must be initialized.");

            //Setting inputs
            _lastInputs = inputs.ToList();
            _inputLayer.SetInputs(_lastInputs);

            //Running recurrent layer first
            if (_memory.Count > 0)
            {
                _recurrentLayer.SetInputs(_memory);
                _recurrentLayer.Run(false, true);
            }

            //Running network
            _inputLayer.Run(true);

            //Returning outputs
            if (_lastOutputs == null || !_lastOutputs.SequenceEqual(_outputLayer.GetOutputs()))
            {
                var firstRun = _lastOutputs == null;
                _lastOutputs = _outputLayer.GetOutputs();
                if(!firstRun) Remember();
            }
            return _lastOutputs;
        }

        //Learning network
        public void Learn(bool isPositive)
        {
            Learn(isPositive, LearningRate);
        }

        //Learning with specified rate
        public void Learn(bool isPositive, float learningRate)
        {
            if (_lastOutputs == null) return;
            for(var i=0; i < _lastOutputs.Count; i++)
            {
                var neuron = _outputLayer.Neurons[i];
                var positive = _lastOutputs[i] >= 1.0f;
                var list = neuron.SourceSynapses.Concat(neuron.RecurrentSynapses).ToList();
                foreach (var synapse in list)
                    synapse.Backpropagate(!(positive ^ isPositive), learningRate);
            }
            Remember();
        }

        public void Remember()
        {
            if (_lastInputs == null || _lastOutputs == null) return;
            var count = _inputLayer.NeuronCount + _outputLayer.NeuronCount;

            if (_recurrentLayer.Neurons.Count >= count*MemorySize) return;

                var newNeurons = _recurrentLayer.AddNeurons(_inputLayer.NeuronCount);

                for (var i = 0; i < _inputLayer.NeuronCount; i++)
                {
                    var neuron = newNeurons[i];
                    var inputNeuron = _inputLayer.Neurons[i];
                    for (var j = 0; j < inputNeuron.DestinationSynapses.Count; j++)
                        neuron.DestinationSynapses[j].Weight = inputNeuron.DestinationSynapses[j].Weight;
                }

                _recurrentLayer.AddNeurons(_outputLayer.NeuronCount,
                    _startRange, _endRange);

            if(_memory.Count >= count * MemorySize)
                _memory.RemoveRange(0, count);

            _memory.AddRange(_lastInputs);
            _memory.AddRange(_lastOutputs);
        }
    }
}
