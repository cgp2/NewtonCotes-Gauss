using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewtonComparation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Lines> lines = new List<Lines>();
        private double size, coffx = 0, coffy = 0, size0;
        private static double a = 1.1, b = 2.5, bt = 2.0 / 5, I = 18.6029478572781;
        private static int m = 1, n = 3;
        public MainWindow()
        {
            InitializeComponent();

            size = Canv.Height;
            size0 = size / 2;

            Lines X = new Lines(0, size0, size, size0);
            X.BuildLine(2, System.Windows.Media.Brushes.Black);
            Canv.Children.Add(X.ln);

            Lines Y = new Lines(size0, 0, size0, size);
            Y.BuildLine(2, System.Windows.Media.Brushes.Black);
            Canv.Children.Add(Y.ln);

            for (int i = 0; i <= size; i = i + (int)size / 20)
            {
                Lines zX = new Lines(i, size0 - 4, i, size0 + 4);
                zX.BuildLine(2, System.Windows.Media.Brushes.Black);
                Canv.Children.Add(zX.ln);
            }

            for (int i = 0; i <= size; i = i + (int)size / 20)
            {
                Lines zY = new Lines(size0 - 4, i, size0 + 4, i);
                zY.BuildLine(2, System.Windows.Media.Brushes.Black);
                Canv.Children.Add(zY.ln);
            }

            List<PointCollection> points = new List<PointCollection>();
            for (int i = 0; i < 2; i++)
                points.Add(new PointCollection());
            List<PointCollection> points0 = new List<PointCollection>();
            for (int i = 0; i < 2; i++)
                points0.Add(new PointCollection());

            for (int i = 2; i < 41; i++)
            {
                points[0].Add(new Point(i, Math.Abs(I - NewtonKotes(a, b, 1, i))));
                points[1].Add(new Point(i, Math.Abs(I - NewtonKotes(a, b, i, 2))));
            }

            double maxxp = 0, minxn = 0, maxyp = 0, minyn = 0, stepx = 0, stepy = 0; 
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < points[i].Count; j++)
                {
                    if (points[i][j].X > 0)
                    {
                        if (points[i][j].X > maxxp)
                            maxxp = points[i][j].X;
                    }
                    else
                    {
                        if (points[i][j].X < maxxp)
                            minxn = points[i][j].X;
                    }
                    if (points[i][j].Y > 0)
                    {
                        if (points[i][j].Y > maxyp)
                            maxyp = points[i][j].Y;
                    }
                    else
                    {
                        if (points[i][j].Y < minyn)
                            minyn = points[i][j].Y;
                    }
                }
            }
            coffx = maxxp > -minxn ? maxxp : minxn;
            coffy = maxyp > -minyn ? maxyp : minyn;

            stepx = maxxp > -minxn ? maxxp / 10 : -minxn / 10;
            stepy = maxyp > -minyn ? maxyp / 10 : -minyn / 10;

            double k = 2;
            for (int i = (int)size0 / 5; i <= size; i = i + (int)size0 / 5)
            {
                TextBlock tx1 = new TextBlock();
                Canv.Children.Add(tx1);
                tx1.Margin = new Thickness(size0 + i + 2, size0, 0, 0);
                tx1.FontSize = 10;
                tx1.Text = Convert.ToString(k * stepx);
                TextBlock tx2 = new TextBlock();
                Canv.Children.Add(tx2);
                tx2.Margin = new Thickness(size0 - i - 2, size0, 0, 0);
                tx2.FontSize = 10;
                tx2.Text = Convert.ToString(-k * stepx);

                TextBlock tx3 = new TextBlock();
                Canv.Children.Add(tx3);
                tx3.Margin = new Thickness(size0 + 10, size0 + i - 7, 0, 0);
                tx3.FontSize = 10;
                tx3.Text = Convert.ToString(-k * stepy);
                TextBlock tx4 = new TextBlock();
                Canv.Children.Add(tx4);
                tx4.Margin = new Thickness(size0 + 10, size0 - i - 7, 0, 0);
                tx4.FontSize = 10;
                tx4.Text = Convert.ToString(k * stepy);
                k += 2;
            }          

            for (int i = 0; i < 2; i++)
            {
                points0[i] = Lines.Transform(points[i], size, coffx, coffy);
                for (int j = 1; j < points[i].Count; j++)
                {
                    Lines line = new Lines(points0[i][j - 1].X, points0[i][j - 1].Y, points0[i][j].X, points0[i][j].Y);
                    switch (i)
                    {
                        case 0:
                            line.BuildLine(2, System.Windows.Media.Brushes.Blue);
                            break;
                        case 1:
                            line.BuildLine(2, System.Windows.Media.Brushes.Red);
                            break;
                    }

                    Canv.Children.Add(line.ln);
                }
            }
        }


        private static double NewtonKotes(double a, double b, int m, int n)
        {
            double h1 = (b - a) / m, res = 0;
            for (int i = 1; i <= m; i++)
            {
                double[] mu = new double[n],
                         x = new double[n];
                double[,] matr = new double[n, n];

                double hm = h1 / (n - 1);

                for (int j = 0; j < n; j++)
                {
                    x[j] = (a + (i - 1) * h1) + j * hm;
                    mu[j] = GetMu(j, -bt, a + i * h1) - GetMu(j, -bt, a + (i - 1) * h1);
                }

                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        matr[j, k] = Math.Pow(x[k], j);
                    }
                }

                Eq equat = new Eq(matr, mu);

                double[] coof = Gauss(equat);

                for (int j = 0; j < n; j++)
                    res += coof[j] * func(x[j]);
            }
            return res;

        }

        private static double[] Gauss(Eq equat)
        {
            double[,] m = new double[equat.n, equat.n];
            for (int i = 0; i < equat.n; i++)
            {
                for (int j = 0; j < equat.n; j++)
                {
                    m[i, j] = equat.matr[i, j];
                }
            }
            double d = Eq.Det(m);
            if (d != 0)
            {
                equat.Diag();
                for (int i = equat.n - 1; i >= 0; i--)
                {
                    double sum = 0;
                    for (int j = equat.n - 1; j > i; j--)
                    {
                        sum += equat.x[j] * equat.matr[i, j];
                    }
                    equat.x[i] = (equat.b[i] - sum) / equat.matr[i, i];
                }
            }
            return equat.x;
        }

        private static double GetMu(int s, double al, double x)
        {
            if (s != 0)
                return (Math.Pow(x, s) * Math.Pow(x - 1.1, 1 + al) - s * GetMu(s - 1, 1 + al, x)) / (1 + al);
            else
                return Math.Pow(x - 1.1, 1 + al) / (1 + al);

        }

        private static double func(double x)
        {
            return 0.5 * Math.Cos(2 * x) * Math.Exp(2 * x / 5) + 2.4 * Math.Sin(1.5 * x) * Math.Exp(-6 * x) + 6 * x;
        }

    }
}
