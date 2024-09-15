namespace CyclingMaps.Models;

using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

public static class GpxReader
{
    public async static Task<Track> ParseFileAsync(StreamReader fileContent) {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Async = true;
        XmlReader reader = XmlReader.Create(fileContent, settings);

        string name = "";
        string type = "";
        var positions = new List<Point>(1000);
        
        string currentElement = "";
        string currentValue = "";

        double currentLat = 0.0d;
        double currentLon = 0.0d;
        double currentElev = 0.0d;

        while (await reader.ReadAsync()) {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    currentElement = reader.Name;

                    if (reader.HasAttributes) {
                        while (reader.MoveToNextAttribute()) {
                            switch (reader.Name) {
                            case "lat": 
                                currentLat = double.Parse(await reader.GetValueAsync());
                                break;
                            case "lon": 
                                currentLon = double.Parse(await reader.GetValueAsync());
                                break;
                            }
                        }
                    }

                    break;
                case XmlNodeType.Text:
                    currentValue = await reader.GetValueAsync();
                    break;
                case XmlNodeType.EndElement:
                    switch (reader.Name) {
                    case "name": 
                        name = currentValue;
                        break;
                    case "type": 
                        type = currentValue;
                        break;
                    case "ele": 
                        currentElev = double.Parse(currentValue);
                        break;
                    case "trkpt":
                        positions.Add(new Point(currentLat, currentLon, currentElev));
                        break;
                    }
                break;
            }
        }

        return new Track(name, type, positions);
    }
}