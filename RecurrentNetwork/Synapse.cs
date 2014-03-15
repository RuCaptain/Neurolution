namespace RecurrentNetworkLibrary
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
        public void Backpropagate(bool isPositive, float learningRate)
        {
            //Recalculating weight
            var k = 1 - learningRate;
            if (isPositive) k = 1/k;
            Weight = NetworkUtils.Range(Weight*k, 0.001f, 0.999f);

            //Backpropagating source synapses
            foreach(var synapse in SourceNeuron.SourceSynapses)
                synapse.Backpropagate(isPositive, learningRate);
        }
    }
}
