using System.Xml.Serialization;

public class DeviceStatus
{
    public string ModuleCategoryID { get; set; }

    public int IndexWithinRole { get; set; }

    public RapidControlStatus RapidControl { get; set; }
}