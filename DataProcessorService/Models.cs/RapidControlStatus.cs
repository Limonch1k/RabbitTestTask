using System.Xml.Serialization;

public class RapidControlStatus
{
    public string ModuleState { get; set; }

    public bool IsBusy { get; set; }

    public bool IsReady { get; set; }

    public bool IsError { get; set; }

    public bool KeyLock { get; set; }

    public int Status { get; set; }

    public string Vial { get; set; }

    public int Volume { get; set; }

    public int MaximumInjectionVolume { get; set; }

    public string RackL { get; set; }

    public string RackR { get; set; }

    public int RackInf { get; set; }

    public bool Buzzer { get; set; }
}