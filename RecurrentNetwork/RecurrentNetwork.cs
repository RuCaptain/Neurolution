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
        public float EvaluationFactor = 0.1f;
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

        public RecurrentNetwork(int inputNeuronsCount, int outputNeuronsCount, int hiddenLayersCount, int hiddenNeuronsCount)
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
            _outputLayer = new Layer(outputNeuronsCount, new SigmoidFunction(), Layers.Last(), null);
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
        public IEnumerable<float> Run(IEnumerable<float> inputs, bool doRemember = true)
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
                if(!firstRun && doRemember) Remember();
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
            if (_memory.Count == 0) return;

            var inputsCount = _inputLayer.NeuronCount;
            var outputsCount = _outputLayer.NeuronCount;
            var count = inputsCount + outputsCount;

            for (var i = 0; i < _memory.Count; i += count)
            {
                var inputs = _memory.GetRange(i, count - outputsCount);
                var outputs = _memory.GetRange(i + inputsCount, outputsCount);
                if(!isPositive)outputs = outputs.Select(s => 1 - s).ToList();

                TrainNetwork(learningRate, inputs, outputs);
            }

            _memory.Clear();
        }

        private void TrainNetwork( float learningRate, IEnumerable<float> inputs, List<float> outputs)
        {
            Run(inputs, false);
            
            _outputLayer.SetErrors(outputs);
            _outputLayer.EvaluateErrors();
            
            foreach (var layer in Layers)
                layer.Learn(learningRate);

            foreach (var layer in Layers)
                layer.CleanErrors();
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
                        neuron.DestinationSynapses[j].Weight = inputNeuron.DestinationSynapses[j].Weight/2f;
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
