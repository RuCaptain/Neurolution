﻿namespace RecurrentNetworkLibrary.ActivationFunctions
{
    public class LinearFunction : IActivationFunction
    {
        public float Calculate(float value)
        {
            return value;
        }

        public float Calculate(float value, float parameter)
        {
            return value;
        }
    }
}
