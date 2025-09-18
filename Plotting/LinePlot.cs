namespace CSharpAlgorithms.Plotting;

using ScottPlot;

public static class LinePlot
{
    public static void Plot(double[] values)
    {
        Plot plot = new Plot();

        for (int i = 0; i < values.Length - 1; i++)
        {
            plot.Add.Line(i + 1, values[i], i + 2, values[i + 1]);
        }

        plot.SavePng("line_plot.png", 1280, 720);
    }
}