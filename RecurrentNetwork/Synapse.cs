﻿namespace RecurrentNetworkLibrary
{
    public class Synapse
    {
        public Neuron SourceNeuron;
        public Neuron DestinationNeuron;
        public float Weight;

        public Synapse(Neuron sourceNeuron, Neuron destinationNeuron, float weight)
        {
            SourceNeuron = sourceNeuron;
            DestinationNeuron = destinationNeuron;
            Weight = weight;
        }

        //Calculating destination neuron's power considering own weight
        public void Propagate()
        {
            DestinationNeuron.Input += SourceNeuron.Output*Weight;
        }

        //Primitive verion of backpropagation algorithm
        public void Backpropagate()
        {
            SourceNeuron.Error += DestinationNeuron.Error*Weight;
        }

        public void OptimizeWeight(float learningRate)
        {
            var delta = learningRate*DestinationNeuron.Error*SourceNeuron.Output;
            Weight += delta;
        }
    }
}
