using System.Collections.Generic;

namespace RecurrentNetworkLibrary
{
    public class Neuron
    {
        public List<Synapse> SourceSynapses = new List<Synapse>();
        public List<Synapse> DestinationSynapses = new List<Synapse>();
        public List<Synapse> RecurrentSynapses = new List<Synapse>(); 
        public float Input;
        public float Output;
        private readonly Layer _parent;

        public Neuron(Layer parent)
        {
            _parent = parent;
        }

        //Gathering power of previous layer and calculation own power
        public void Run(bool activate, bool runOnce)
        {
            var synapseList = runOnce ? RecurrentSynapses : SourceSynapses;
            if(synapseList.Count > 0)
            {
                if(!runOnce) Input = 0f;
                foreach (var synapse in synapseList)
                    synapse.Propagate();
            }
            Output = activate ? _parent.ActivationFunction.Calculate(Input) : Input;
        }
    }
}
