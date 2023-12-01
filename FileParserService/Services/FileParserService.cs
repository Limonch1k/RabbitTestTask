using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Services;

public class FileParserService
{
    private string spec_symbol {get;set;} = "\\";

    private ILogger _logger {get;set;} = null;

    private ConcurrentDictionary<int, string> ModuleStates = new ConcurrentDictionary<int, string>
    (
        new[] 
        {
            new KeyValuePair<int,string> (0, "Online"),
            new KeyValuePair<int,string> (1, "Run"),
            new KeyValuePair<int,string> (2, "NotReady"),
            new KeyValuePair<int,string> (3, "Offline")
        }
    ) ;

    public FileParserService(ILogger logger)
    {
        _logger = logger;
        var platform = Environment.OSVersion.Platform;

        if (platform == PlatformID.Win32NT)
        {
            spec_symbol = @"\";
        }
        else if (platform == PlatformID.Unix)
        {
           spec_symbol = "/";
        }
    }

    public async Task<List<InstrumentStatus>> DeserializeXmlFiles(string directory)
    {
        _logger.LogInformation("Try find files and directory...");

        string[] xmlFiles = null;

        try
        {
            xmlFiles = Directory.GetFiles(directory, "*.xml");

            if (xmlFiles.Length == 0)
            {
                _logger.LogInformation("Does not found any xml files...");
                return null;
            }
            _logger.LogInformation("The next list of xml files was found: " + string.Join(", ", xmlFiles));
        }
        catch
        {
            _logger.LogInformation("Does not found directory...");
            return null;
        }
        
        ConcurrentBag<InstrumentStatus> statusList = new ConcurrentBag<InstrumentStatus>();

        List<Task> tasks = new List<Task>();

        var t = Parallel.ForEachAsync(xmlFiles, async (xml, cans) =>
        {
            var mass = xml.Split(spec_symbol);
            var xml_string = mass[mass.Length - 1];

            XmlDocument xmlDoc = new XmlDocument();
            
            string filePath = directory + spec_symbol + xml_string;

            try
            {
                _logger.LogInformation("try read file " + xml_string);
                xmlDoc.Load(filePath);
            }
            catch
            {
                _logger.LogInformation("Could not read xml file");
                return;
            }
            
            XmlSerializer serializer = new XmlSerializer(typeof(InstrumentStatus));

            StringReader stringReader = new StringReader(xmlDoc.OuterXml);

            InstrumentStatus instrument = null;

            try
            {
                instrument = serializer.Deserialize(stringReader) as InstrumentStatus;
            }
            catch
            {
                _logger.LogInformation("Could not deserialize this file " + xml_string);
                return;
            } 

            //тут у вас в параметре xml примера содержится строка, которая из себя тоже представляет xml
            // я решил ее обработать так
            foreach(var device in instrument.DeviceStatuses)
            {
                Random rnd = new Random();

                int a  = rnd.Next(0, 3);

                byte[] utf16Bytes = Encoding.Unicode.GetBytes(device.RapidControlStatus);
                string decodedString = Encoding.Unicode.GetString(utf16Bytes);

                try
                {
                    _logger.LogInformation("try to load xml string property as xml doc for this file: " + xml_string);
                    xmlDoc.LoadXml(decodedString);
                }
                catch
                {
                    _logger.LogInformation("Could not load xml string property as xml doc for this file: " + xml_string);
                    continue;
                }
                

                //тут у вас в примере xml может быть разный заголовок, и одинаковое тело xml
                // меня интересует только 1 параметр в теле
                // поэтому тут я игнорирую разные заголовки

                XmlElement rootElement = xmlDoc.DocumentElement;

                string rootName = rootElement.Name;

                XmlRootAttribute xmlRoot = new XmlRootAttribute();

                xmlRoot.ElementName = rootName;
                
                serializer = new XmlSerializer(typeof(RapidControlStatus), xmlRoot);

                stringReader = new StringReader(decodedString);

                RapidControlStatus rapid = null;

                try
                {
                    _logger.LogInformation("Try deserialize xml property string to object for file " + xml_string);
                    rapid = serializer.Deserialize(stringReader) as RapidControlStatus;
                }
                catch
                {
                    _logger.LogInformation("Could not deserialize xml property string to object for file " + xml_string);
                    continue;
                }

                device.RapidControl = rapid;
                device.RapidControl.ModuleState = ModuleStates[a];
            }

            statusList.Add(instrument);
        });
        await t;

        return statusList.ToList();
    }

    public string SerializeToJson(List<InstrumentStatus> models)
    {
        var json = JsonConvert.SerializeObject(models);

        return json;
    }
}