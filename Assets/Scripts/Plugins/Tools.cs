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
}
