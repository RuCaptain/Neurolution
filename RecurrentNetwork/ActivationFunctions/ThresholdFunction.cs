namespace RecurrentNetworkLibrary.ActivationFunctions
{
    public class ThresholdFunction : IActivationFunction
    {
        private readonly float _threshold;
        public ThresholdFunction()
        {
            _threshold = 0.5f;
        }

        public ThresholdFunction(float threshold)
        {
            _threshold = threshold;
        }
        public float Calculate(float value)
        {
            return (value >= _threshold) ? 1 : 0;
        }

        public float Calculate(float value, float parameter)
        {
            return (value >= parameter) ? 1 : 0;
        }

        public float Derivative(float value)
        {
            throw new System.NotImplementedException();
        }
    }
}
