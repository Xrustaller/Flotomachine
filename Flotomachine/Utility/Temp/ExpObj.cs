using System.Collections.Generic;
using System.Linq;
using Flotomachine.Services;

namespace Flotomachine.Utility;

public class ExpObj
{
    public string Time { get; set; }
    public string Timer1 { get; set; }
    public string Timer2 { get; set; }
    public string Temperature { get; set; }
    public string Pres { get; set; }
    public string Scale { get; set; }
    public string Oboroti { get; set; }

    public ExpObj()
    {

    }

    public ExpObj(ExperimentData experimentData)
    {
        ExperimentDataValue temp;
        var data = DataBaseService.GetExperimentDataValues(experimentData.Id);
        Time = experimentData.Date.ToLongTimeString();

        temp = data.FirstOrDefault(p => p.ModuleFieldId == 1);
        Timer1 = temp == null ? "NULL" : temp.ModuleData.ToString();

        temp = data.FirstOrDefault(p => p.ModuleFieldId == 2);
        Timer2 = temp == null ? "NULL" : temp.ModuleData.ToString();
        
        temp = data.FirstOrDefault(p => p.ModuleFieldId == 3);
        Temperature = temp == null ? "NULL" : temp.ModuleData.ToString();

        temp = data.FirstOrDefault(p => p.ModuleFieldId == 4);
        Pres = temp == null ? "NULL" : temp.ModuleData.ToString();

        temp = data.FirstOrDefault(p => p.ModuleFieldId == 7);
        Scale = temp == null ? "NULL" : temp.ModuleData.ToString();

        temp = data.FirstOrDefault(p => p.ModuleFieldId == 8);
        Oboroti = temp == null ? "NULL" : temp.ModuleData.ToString();
    }
}