namespace TestBoxPlot;

internal class Program
{
    static void Main(string[] args)
    {
        Test1(BoxPlot.GraphicsSystems.Gdi);
        //Test1(BoxPlot.GraphicsSystems.Svg);
    }

    static void Test1(BoxPlot.GraphicsSystem.IGraphicsSystem graphics)
    {
        var settings = new BoxPlot.Settings();
        settings.GraphicsSystem = graphics;
        settings.Width = 375;
        settings.Height = 480;
        settings.Values.Add(48.1, 40, 38.4, 34.4, 29.1, 21.6, 12.7, 1, -3.7, -24.4, -36.4);
        settings.Filename = ".\\Example.bmp";
        settings.LabeledValues.Add(12.7, "Campbell Soup");
        BoxPlot.Generator.Generate(settings);

        //            settings.Values = new double[] { 35.1399454162681, 22.467059624327, 22.2040257371108, 21.506358986582, 20.9888340384341, -0.824084841095328, -39.0345414605262 };
        //    BoxPlot.Generator.GenerateBoxPlot(graphics, 375, 480, ".", "CccPageBoxPlot", "COMPANHIA PARANAENSE", -39.0345414605262, 35.1399454162681, 22.467059624327, 22.2040257371108, 21.506358986582, 20.9888340384341, -0.824084841095328, -39.0345414605262);
        //     BoxPlot.Generator.GenerateBoxPlot(graphics, 375, 480, ".", "DsoPageBoxPlot", "COMPANHIA PARANAENSE", 1.8280027824141, 103.439015974455, 70.5947356903936, 54.4278801788634, 40.349240356872, 35.3360362424381, 17.8408885474103, 1.8280027824141);
        //     BoxPlot.Generator.GenerateBoxPlot(graphics, 375, 480, ".", "DioPageBoxPlot", "COMPANHIA PARANAENSE", 4.20123700948353, 22.2043952161156, 19.8862659266786, 8.84459594267494, 5.48235200586716, 4.20123700948353, 2.74844968626082, 0);
        //            BoxPlot.Generator.GenerateBoxPlot(graphics, 375, 480, ".", "DpoPageBoxPlot", "COMPANHIA PARANAENSE", 45.0637812524238, 86.4543083559952, 71.4188205314889, 45.0637812524238, 35.6699708785422, 27.4136901567195, 21.9766064480023, 16.7383204356549);
    }
}
