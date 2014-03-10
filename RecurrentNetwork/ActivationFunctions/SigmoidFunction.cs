using System;

namespace RecurrentNetworkLibrary.ActivationFunctions
{
    public class SigmoidFunction : IActivationFunction
    {
        public float Calculate(float value)
        {
            //return 1f / (1 + (float)Math.Exp(Math.E * (-value*2 + 1)));
            return 1f/(1 + (float) Math.Exp(-value));
        }

        public float Calculate(float value, float parameter)
        {
            return Calculate(value);
        }
    }
}
