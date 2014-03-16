using System;

namespace RecurrentNetworkLibrary.ActivationFunctions
{
    public class SigmoidFunction : IActivationFunction
    {
        public float Calculate(float value)
        {
            return 1f / (1 + (float)Math.Exp(Math.E * (-value + 0.5f)));
            //Result values of this function is in range of (~0.2, ~0.8) except of basic sigmoid's (~0.5, ~0.7) for x in (0, 1).

            //return 1f/(1 + (float) Math.Exp(-value));
        }

        public float Calculate(float value, float parameter)
        {
            return 1f / (1 + (float)Math.Exp(Math.E * parameter * (-value + 0.5f)));
        }

        public float Derivative(float value)
        {
            return (float)Math.Exp(value)/(float)Math.Pow(Math.Exp(value) + 1, 2);
        }
    }
}
