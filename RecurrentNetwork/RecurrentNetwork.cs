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
        private readonly Layer _inputLayer;
        private readonly Layer _outputLayer;
        private readonly Layer _recurrentLayer;
        private List<float> _lastOutputs;
        private bool _isInitialized;

        public RecurrentNetwork(int inputNeuronsCount, int outputNeuronsCount, int hiddenLayersCount, float threshold = 0.5f)
        {
            //Initizating input layer
            _inputLayer = new Layer(inputNeuronsCount, new LinearFunction(), null, null);
            Layers.Add(_inputLayer);

            //Initizating hidden layers
            for (var i = 0; i < hiddenLayersCount; i++)
            {
                var hiddenLayer = new Layer(inputNeuronsCount, new SigmoidFunction(), Layers.Last(), null);
                Layers.Last().SetOutputLayer(hiddenLayer);
                Layers.Add(hiddenLayer);
            }

            //Initizating output layer
            _outputLayer = new Layer(outputNeuronsCount, new ThresholdFunction(threshold), Layers.Last(), null);
            Layers.Last().SetOutputLayer(_outputLayer);
            Layers.Add(_outputLayer);

            //Initizating recurrent layer
            _recurrentLayer = new Layer(outputNeuronsCount, new SigmoidFunction(), null, _outputLayer);
            _outputLayer.SetRecurrentLayer(_recurrentLayer);
            Layers.Add(_recurrentLayer);

        }

        //Setting initial synapse weights
        public void Initialize(float startRange, float endRange)
        {
            foreach(var layer in Layers)
                layer.Initialize(startRange, endRange);
            _isInitialized = true;
        }

        //Running network
        public IEnumerable<float> Run(IEnumerable<float> inputs)
        {
            if (!_isInitialized)
                throw new Exception("Network must be initialized.");

            //Setting inputs
            _inputLayer.SetInputs(inputs.ToList());

            //Runnung primary layers
            _inputLayer.Run(true);

            //Running recurrent layer and output again
            if (_lastOutputs != null)
            {
                _recurrentLayer.SetInputs(_lastOutputs);
                _recurrentLayer.Run(true, true);
            }

            //Returning outputs
            if(_lastOutputs == null || !_lastOutputs.SequenceEqual(_outputLayer.GetOutputs()))
                _lastOutputs = _outputLayer.GetOutputs();
            return _lastOutputs;
        }

        //Learning network
        public void Learn(bool isPositive)
        {
            foreach (var neuron in _outputLayer.Neurons)
            {
                var positive = neuron.Output >= 1.0f;
                var list = neuron.SourceSynapses.Concat(neuron.RecurrentSynapses).ToList();
                foreach(var synapse in list)
                    synapse.Backpropagate(!(positive ^ isPositive), LearningRate);
            }
        }

        //Learning with specified rate
        public void Learn(bool isPositive, float learningRate)
        {
            foreach (var neuron in _outputLayer.Neurons)
            {
                var positive = neuron.Output >= 1.0f;
                var list = neuron.SourceSynapses.Concat(neuron.RecurrentSynapses).ToList();
                foreach (var synapse in list)
                    synapse.Backpropagate(!(positive ^ isPositive), learningRate);
            }
        }
    }
}
