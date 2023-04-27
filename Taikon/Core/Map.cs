using System.Collections.Generic;
using System.IO;

namespace Taikon.Core;

public class SubMap
{
    public string Song;
    public string Artist;
    public string Creator;
    public string AudioFile;
    public string Background;
    public int BPM;
    public List<string> Objects;
}
public class Map
{
    public string FolderName;
    public List<SubMap> SubMaps;
    
    public static Map ReadMaps(string folderName)
    {
        var Map = new Map();
        Map.SubMaps = new();
        Map.FolderName = folderName;
        foreach (var file in Directory.GetFiles(folderName, "*.tai"))
        {
            SubMap subMap = new();
            List<string> objects = new();
            string[] lines = File.ReadAllLines(file);
            bool objectsStarted = false;
            foreach (var line in lines)
            {
                if (line == "")
                    continue;
                if (line.StartsWith("//"))
                    continue;
                if (line.StartsWith("["))
                {
                    if (line == "[Objects]")
                        objectsStarted = true;
                    continue;
                }
    
                if (!objectsStarted)
                {
                    var split = line.Split(":");
                    switch (split[0])
                    {
                        case "Song":
                            subMap.Song = split[1].Trim();
                            break;
                        case "Artist":
                            subMap.Artist = split[1].Trim();
                            break;
                        case "Creator":
                            subMap.Creator = split[1].Trim();
                            break;
                        case "Audio":
                            subMap.AudioFile = split[1].Trim();
                            break;
                        case "BPM":
                            subMap.BPM = int.Parse(split[1].Trim());
                            break;
                        case "Background":
                            subMap.Background = split[1].Trim();
                            break;
                    }
                }
                else
                {
                    objects.Add(line);
                }
            }
            
            subMap.Objects = objects;
            Map.SubMaps.Add(subMap);
        }

        return Map;
    }
}

