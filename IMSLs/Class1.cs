using System;
using Imsl.Math;
using Imsl.Stat;

public class Activation_functions : NonlinearRegression.IFunction
{

    static Single[] ydata;
    static Single[] xdata;
    static int nobs;
    public static int Z;
    public static int L = 2;
    static int d;
    public Single SigmoidSlope = 10;


    public bool f(double[] theta, int iobs, double[] frq, double[] wt, double[] e)
    {

        bool iend;



        if (iobs < nobs)
        {
            wt[0] = 1.0;
            frq[0] = 1.0;
            iend = true;

            double E = 0;
            double E1 = 0;
            double E2 = 0;
            double S = 0;
            double sum = 0;

            Single[] xd = new Single[Z];


            for (int c = 0; c < Z; c++)
            {
                xd[c] = xdata[iobs * Z + c];

            }
            E1 = ComputeE(xd, theta, Z);
            E = SegmentA(E1, 0);
            //   E += 2 * SegmentB(E1);
            e[0] = Math.Abs(ydata[iobs] - E);

        }
        else
        {
            iend = false;
        }
        return iend;
    }
    public double ComputeE(Single[] xd, double[] teta, int Z)
    {
        double E = 0;
        double I = 0;

        //   for (int c = 0; c < Z; c++) { I += xd[c] + Math.Pow((xd[c]), 0.5); }

        for (int c = 0; c < Z; c++)
        {
            // runing trough the layeres
            for (int l = c * L * 3; l < c * L * 3 + L * 3; l += 3) { E += teta[l] * Sigmoid(xd[c], teta[l + 1], teta[l + 2]); }
        }

        return E;
    }

    public double SegmentA(double x, double offset)
    {
        return Sigmoid(x, offset, SigmoidSlope);
        //return Convert.ToDouble(x > 0);
        //return Gauss(x, 10, 10);
    }


    public double SegmentB(double x)
    {
        //return SigmoidR(x, 0, 0.5);
        return Gauss(x, -20, 10);
    }


    public double Sigmoid(double x, double b, double a)
    {
        return 1 / (1 + Math.Exp(-(x - b) * a));

    }

    public double SigmoidR(double x, double b, double a)
    {
        return 1 / (1 + Math.Exp((x - b) * a));
    }


    public double Gauss(double x, double x0, double b)
    {
        return Math.Exp(-Math.Pow(Math.Abs(x - x0), 1) / b);
    }

    public double[] Main(byte[] vx, byte[] vy, double scal, double tol, int Zin)
    {
        nobs = vy.GetLength(0);

        Z = Zin;
        xdata = new Single[nobs * Z];
        ydata = new Single[nobs];
        for (int i = 0; i < nobs; i++)
        {
            ydata[i] = vy[i];

        }


        for (int i = 0; i < nobs * Z; i++)
        {
            xdata[i] = vx[i];

        }



        int nparm = L * Z * 3;
        double[] theta = new double[nparm];
        NonlinearRegression regression = new NonlinearRegression(nparm);
        theta = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        regression.Guess = theta;


        double[] stupid = new double[nparm];


        for (int j = 0; j < nparm; j++)
        {
            stupid[j] = scal;
        }


        regression.Scale = stupid;
        regression.MaxIterations = 500;
        regression.AbsoluteTolerance = tol;

        NonlinearRegression.IFunction fcn = new Activation_functions();
        // GenerateData();
        double[] coef = regression.Solve(fcn);

        Console.Out.WriteLine
        ("The computed regression coefficients are {" + coef[0] + ", "
        + coef[1] + "}");

        Console.Out.WriteLine("The sums of squares for error is "
        + regression.GetSSE());
        return coef;
    }
}