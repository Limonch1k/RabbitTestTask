using System.Xml.Serialization;

[XmlRoot("InstrumentStatus")]
public class InstrumentStatus
{
    public string PackageID { get; set; }

    public DeviceStatus[] DeviceStatuses { get; set; }
}