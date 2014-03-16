namespace RecurrentNetworkLibrary.ActivationFunctions
{
    //Activation function is used for calculating a neuron's power
    public interface IActivationFunction
    {
        float Calculate(float value);
        float Calculate(float value, float parameter);
        float Derivative(float value);
    }
}
