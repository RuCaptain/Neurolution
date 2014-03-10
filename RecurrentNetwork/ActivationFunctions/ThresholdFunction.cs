namespace RecurrentNetworkLibrary.ActivationFunctions
{
    public class ThresholdFunction : IActivationFunction
    {
        public float Calculate(float value)
        {
            return (value >= 0.5f) ? 1 : 0;
        }

        public float Calculate(float value, float parameter)
        {
            return (value >= parameter) ? 1 : 0;
        }
    }
}
