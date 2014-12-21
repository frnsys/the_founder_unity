using UnityEngine;

class Tools {
    // http://stackoverflow.com/q/5817490/1097920
    public static double RandomGaussian()
    {
        double U, u, v, S;

        do
        {
            u = 2.0 * Random.value - 1.0;
            v = 2.0 * Random.value - 1.0;
            S = u * u + v * v;
        }
        while (S >= 1.0);

        double fac = System.Math.Sqrt(-2.0 * System.Math.Log(S) / S);
        return u * fac;
    }

    public static float RandomGaussian(float mean, float std) {
        float r = (float)RandomGaussian();
        return (r * std) + mean;
    }

    public static int Fibonacci(int n) {
        if (n == 0)
            return 0;
        else if (n == 1)
            return 1;
        else
            return Fibonacci(n-1) + Fibonacci(n-2);
    }

    public static float Gaussian(float x, float mean, float sd) {
        return ( 1 / ( sd * (float)System.Math.Sqrt(2 * (float)System.Math.PI) ) ) * (float)System.Math.Exp( -System.Math.Pow(x - mean, 2) / ( 2 * System.Math.Pow(sd, 2) ) );
    }
    public static float LimitRange(float value, float min, float max) {
        return (value < min) ? min : (value > max) ? max : value;
    }
}
