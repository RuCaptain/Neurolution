using System;
using System.Collections.Generic;
using System.Linq;
using RecurrentNetworkLibrary.ActivationFunctions;

namespace RecurrentNetworkLibrary
{
    //Neuron layer contains neurons and may be connected with other ones
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

        //Connecting own neurons with ones in connected layers
        public void Initialize(List<Neuron> neurons, float startRange, float endRange)
        {
            if (OutputLayer == null) return;
            foreach (var neuron in neurons)
                foreach (var outputNeuron in OutputLayer.Neurons)
                {
                    var synapse = new Synapse(neuron, outputNeuron,
                        NetworkUtils.RandomRange(startRange, endRange));
                    neuron.DestinationSynapses.Add(synapse);

                    if (IsRecurrent)
                        outputNeuron.RecurrentSynapses.Add(synapse);
                    else 
                        outputNeuron.SourceSynapses.Add(synapse);

                }
        }

        public void Initialize(float startRange, float endRange)
        {
            Initialize(Neurons, startRange, endRange);
        }


        //Running own neurons and next layer (if it exists)
        public void Run(bool activate, bool runOnce = false)
        {
            foreach(var neuron in Neurons)
                neuron.Run(IsRecurrent || activate, runOnce);

            if (OutputLayer == null || (!IsRecurrent && runOnce)) return;
                OutputLayer.Run(activate, runOnce);
        }

        //Setting inputs (only in input and recurrent layers)
        public void SetInputs(List<float> inputs)
        {
            if(inputs.Count != Neurons.Count)
                if(IsRecurrent)
                    Neurons.ForEach(a => a.Input = 0);
                else
                    throw new Exception("Inputs count must be same as neurons count.");


            for (var i = 0; i < inputs.Count; i++)
                Neurons[i].Input = inputs[i];
        }

        public List<float> GetOutputs()
        {
            return Neurons.Select(neuron => neuron.Output).ToList();
        }

        public List<Neuron> AddNeurons(int count, float startRange, float endRange)
        {
            var newNeurons = new List<Neuron>();

            for (var i = 0; i < count; i++)
                newNeurons.Add(new Neuron(this));

            Initialize(newNeurons, startRange, endRange);
            Neurons.AddRange(newNeurons);

            return newNeurons;
        }
        public List<Neuron> AddNeurons(int count)
        {
            return AddNeurons(count, 0f, 0f);
        }

        public void SetErrors(List<float> expectedOutputs)
        {
            if(expectedOutputs.Count != NeuronCount)
                throw new Exception("Outputs count must be same as neurons count.");


            for (var i = 0; i < NeuronCount; i++)
                Neurons[i].Error = expectedOutputs[i] - Neurons[i].Output;
            
        }

        public void CleanErrors()
        {
            foreach (var neuron in Neurons)
                neuron.Error = 0f;
        }

        public void EvaluateErrors()
        {
            foreach (var neuron in Neurons)
                neuron.EvaluateError();
            if(InputLayer != null)
                InputLayer.EvaluateErrors();
            if(RecurrentLayer != null)
                RecurrentLayer.EvaluateErrors();
        }

        public void Learn(float learningRate)
        {
            foreach(var neuron in Neurons)
                neuron.Learn(learningRate);
        }
    }
}
