using System.Collections.Generic;
using System.IO;

namespace Taikon.Core;

public enum ObjectType
{
    Orange = -1,
    Blue = -2,
    BigOrange = -3,
    BigBlue = -4,
    Spinner = -5
}
public class GameNode {
    public int Time;
}

public class HitNode : GameNode {
       public ObjectType Type;
}

public class SliderNode : GameNode {
    public int EndTime;
}

public class TimingNode
{
    public int Time;
    public int BPM;
}

public class SubMap
{
    public string Song;
    public string Artist;
    public string Creator;
    public string AudioFile;
    public string Background;
    public int BPM;
    public List<GameNode> Objects;
    public List<TimingNode> TimingNodes;
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
            string[] lines = File.ReadAllLines(file);
            bool objectsStarted = false;
            bool timingStarted = false;
            bool metadataStarted = false;
            foreach (var line in lines)
            {
                if (line == "")
                    continue;
                if (line.StartsWith("//"))
                    continue;
                if (line.StartsWith("["))
                {
                    if (line == "[Objects]")
                    {
                        objectsStarted = true;
                        timingStarted = false;
                        metadataStarted = false;
                    }

                    if (line == "[Timing]")
                    {
                        timingStarted = true;
                        objectsStarted = false;
                        metadataStarted = false;
                    }

                    if (line == "[Metadata]")
                    {
                        metadataStarted = true;
                        objectsStarted = false;
                        timingStarted = false;
                    }

                    if (metadataStarted)
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

                    if (timingStarted)
                    {
                        var split = line.Split(",");
                        var timingNode = new TimingNode();
                        timingNode.Time = int.Parse(split[0]);
                        timingNode.BPM = int.Parse(split[1]);
                        subMap.TimingNodes.Add(timingNode);
                    }
                    
                    if (objectsStarted)
                    {
                        var split = line.Split(",");
                        if (split.Length == 1)
                        {
                            // Slider
                            var sliderNode = new SliderNode();
                            var properSplit = split[0].Split("-");
                            sliderNode.Time = int.Parse(properSplit[0]);
                            sliderNode.EndTime = int.Parse(properSplit[1]);
                            subMap.Objects.Add(sliderNode);
                        }
                        else
                        {
                            // Hit
                            var hitNode = new HitNode();
                            hitNode.Time = int.Parse(split[0]);
                            hitNode.Type = (ObjectType)int.Parse(split[1]);
                            subMap.Objects.Add(hitNode);
                        }
                    }
                }
            }
            
            Map.SubMaps.Add(subMap);
        }

        return Map;
    }
}

