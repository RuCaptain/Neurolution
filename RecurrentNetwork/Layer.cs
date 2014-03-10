using System;
using System.Collections.Generic;
using System.Linq;
using RecurrentNetworkLibrary.ActivationFunctions;

namespace RecurrentNetworkLibrary
{
    //Neuron layer contains neurons and may be connected with 
    public class Layer
    {
        public List<Neuron> Neurons = new List<Neuron>();
        public IActivationFunction ActivationFunction;

        public Layer InputLayer;
        public Layer OutputLayer;
        public Layer RecurrentLayer;
        public int NeuronCount;
        public bool IsInput;
        public bool IsOutput;
        public bool IsRecurrent;
        public Layer(int neuronCount, IActivationFunction activationFunction, Layer inputLayer, Layer outputLayer, Layer recurrentLayer = null)
        {
            NeuronCount = neuronCount;
            InputLayer = inputLayer;
            OutputLayer = outputLayer;
            RecurrentLayer = recurrentLayer;
            ActivationFunction = activationFunction;
            UpdateMarkers();

            for (var i = 0; i < NeuronCount; i++)
                Neurons.Add(new Neuron(this));
        }

        public void SetInputLayer(Layer inputLayer)
        {
            InputLayer = inputLayer;
            UpdateMarkers();
        }

        public void SetOutputLayer(Layer outputLayer)
        {
            OutputLayer = outputLayer;
            UpdateMarkers();
        }

        public void SetRecurrentLayer(Layer recurrentLayer)
        {
            RecurrentLayer = recurrentLayer;
            RecurrentLayer.UpdateMarkers();
        }

        private void UpdateMarkers()
        {
            IsInput = (InputLayer == null && OutputLayer != null && OutputLayer.RecurrentLayer == null);
            IsOutput = ((InputLayer != null && OutputLayer == null) || (OutputLayer != null && OutputLayer.IsRecurrent));
            IsRecurrent = (InputLayer == null && OutputLayer != null && OutputLayer.RecurrentLayer != null);
        }


        //Connection own neurons with ones in connected layers
        public void Initialize(float startRange, float endRange)
        {

            if (InputLayer != null)
                foreach(var inputNeuron in InputLayer.Neurons)
                    foreach(var neuron in Neurons)
                        neuron.SourceSynapses.Add(new Synapse(inputNeuron, neuron,
                            NetworkUtils.RandomRange(startRange, endRange)));

            if (OutputLayer != null)
            foreach(var neuron in Neurons)
                foreach(var outputNeuron in OutputLayer.Neurons)
                    neuron.DestinationSynapses.Add(new Synapse(neuron, outputNeuron,
                        NetworkUtils.RandomRange(startRange, endRange)));

            if (RecurrentLayer == null) return;
            foreach (var inputNeuron in RecurrentLayer.Neurons)
                foreach(var neuron in Neurons)
                    neuron.RecurrentSynapses.Add(new Synapse(inputNeuron, neuron,
                        NetworkUtils.RandomRange(startRange, endRange)));
        }

        //Running own neurons and next layer (if it exists)
        public void Run(bool activate, bool recurrently = false)
        {
            foreach(var neuron in Neurons)
                neuron.Run(activate, recurrently);

            if (OutputLayer == null) return;

            OutputLayer.Run(OutputLayer.RecurrentLayer != null ? recurrently : activate, recurrently);
        }

        //Setting inputs (only in input and recurrent layers)
        public void SetInputs(List<float> inputs)
        {
            if(inputs.Count != NeuronCount)
                throw new Exception("Inputs count must be same as neurons count.");

            for (var i = 0; i < NeuronCount; i++)
                Neurons[i].Input = inputs[i];
        }

        public List<float> GetOutputs()
        {
            return Neurons.Select(neuron => neuron.Output).ToList();
        }

    }
}
