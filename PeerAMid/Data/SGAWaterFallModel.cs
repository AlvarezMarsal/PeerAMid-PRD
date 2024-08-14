#nullable enable

namespace PeerAMid.Data;

public class SGAWaterfallModel
{
    public string? TargetCompName { get; set; }
    public decimal RevenueComp { get; set; }
    public List<SGAWaterfallItemModel> WaterfallChartItemList { get; set; } = new List<SGAWaterfallItemModel>();

    public override string ToString()
    {
        var text = "SGAWaterfallModel\n" +
                   "TargetCompName: " + TargetCompName + "\n" +
                   "RevenueComp: " + RevenueComp;

        foreach (var item in WaterfallChartItemList) text += "\n" + item;
        return text;
    }

    public bool IsAllZeroes()
    {
        foreach (var item in WaterfallChartItemList)
        {
            if (!item.IsZero())
                return false;
        }

        return true;
    }
}

public class SGAWaterfallItemModel
{
    public decimal StartValue { get; set; }
    public decimal EndValue { get; set; }
    public string? DepartmentName { get; set; }
    public decimal DepartmentValue { get; set; }
    public decimal TotalCount { get; set; }

    public override string ToString()
    {
        var text = $"{DepartmentName} Value={DepartmentValue} Start={StartValue} End={EndValue} TotalCount={TotalCount}";
        return text;
    }

    public bool IsZero()
    {
        return DepartmentValue == 0;
    }
}


public class DecomposedSgaWaterFallData
{
    public SGAWaterfallModel TopQChartData { get; set; } = new SGAWaterfallModel();
    public SGAWaterfallModel MedianChartData { get; set; } = new SGAWaterfallModel();
    public SGAWaterfallModel TopDChartData { get; set; } = new SGAWaterfallModel();
}

