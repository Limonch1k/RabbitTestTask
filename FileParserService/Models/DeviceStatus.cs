using System.Xml.Serialization;
using Newtonsoft.Json;

public class DeviceStatus
{
    [XmlElement("ModuleCategoryID")]
    public string ModuleCategoryID { get; set; }

    [XmlElement("IndexWithinRole")]
    public int IndexWithinRole { get; set; }

    [XmlElement("RapidControlStatus")]
    [JsonIgnore]
    public string RapidControlStatus { get; set; }

    [XmlIgnore]
    public RapidControlStatus RapidControl {get;set;}
}