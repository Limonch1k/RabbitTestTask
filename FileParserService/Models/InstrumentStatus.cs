using System.Xml.Serialization;

[XmlRoot("InstrumentStatus")]
public class InstrumentStatus
{
    [XmlElement("PackageID")]
    public string PackageID { get; set; }

    [XmlElement("DeviceStatus")]
    public DeviceStatus[] DeviceStatuses { get; set; }
}