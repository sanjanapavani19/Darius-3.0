using System;
using Imsl.Math;
using Imsl.Stat;

public class Polynomial_fit : NonlinearRegression.IFunction
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

            Single[] xd = new Single[2];
            
            xd[0] = xdata[iobs * 2 ];
            xd[1] = xdata[iobs * 2+1];

            E=ComputeE(xd, theta);

            e[0] = Math.Abs(ydata[iobs] - E);

        }
        else
        {
            iend = false;
        }
        return iend;
    }
    public double ComputeE(Single[] xd, double[] teta)
    {
        double E = 0;
     

        E = xd[0] * teta[0] + xd[1] * teta[1];
        E += xd[0]*xd[0] * teta[2] + xd[1] *xd[1]* teta[3];
        E += teta[4];

        return E;
    }





    public double[] Main(Single[] vx, Single [] vy, double scal, double tol)
    {
        nobs = vy.GetLength(0);

  // To account x and y
        xdata = new Single[nobs*2];
        ydata = new Single[nobs];
        for (int i = 0; i < nobs; i++)
        {
            ydata[i] = vy[i];
                    }


        for (int i = 0; i < nobs*2; i++)
        {
            xdata[i] = vx[i];
        }
               
        int nparm = 5;
        double[] theta = new double[nparm];
        NonlinearRegression regression = new NonlinearRegression(nparm);
        theta = new double[] { 0, 0, 0, 0, 0 };
        regression.Guess = theta;


        double[] stupid = new double[nparm];


        for (int j = 0; j < nparm; j++)
        {
            stupid[j] = scal;
        }


        regression.Scale = stupid;
        regression.MaxIterations = 500;
        regression.AbsoluteTolerance = tol;

        NonlinearRegression.IFunction fcn = new Polynomial_fit();
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