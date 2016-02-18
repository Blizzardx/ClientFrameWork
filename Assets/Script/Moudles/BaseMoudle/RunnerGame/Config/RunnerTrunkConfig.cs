using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;


public class RunnerTrunkConfig : XmlConfigBase
{
    [XmlAttribute("trunkId")]
    public int TrunkId { get; set; }

    [XmlAttribute("trunkLength")]
    public int TrunkLength { get; set; }

    [XmlAttribute("trunkDiff")]
    public int TrunkDiff { get; set; }

    [XmlArray("trunkItem")]
    public List<RunnerTrunkItemConfig> ItemList { get; set; }

}
[XmlRoot("trunkElement")]
public class RunnerTrunkElementConfig : XmlConfigBase
{
    [XmlAttribute("trunkId")]
    public int TrunkId { get; set; }

    [XmlAttribute("trunkLength")]
    public int TrunkLength { get; set; }

    [XmlAttribute("trunkDiff")]
    public int TrunkDiff { get; set; }

    [XmlAttribute("trunkDesc")]
    public string TrunkDesc { get; set; }

    [XmlArray("trunkItem")]
    public List<RunnerTrunkItemConfig> ItemList { get; set; }


    public static RunnerTrunkConfig ConvertToConfig(RunnerTrunkElementConfig source)
    {
        RunnerTrunkConfig res = new RunnerTrunkConfig();
        res.TrunkId = source.TrunkId;
        res.TrunkLength = source.TrunkLength;
        res.TrunkDiff = source.TrunkDiff;
        res.ItemList = new List<RunnerTrunkItemConfig>(source.ItemList);
        return res;
    }
    public static RunnerTrunkElementConfig ConvertToElement(RunnerTrunkConfig source)
    {
        RunnerTrunkElementConfig res = new RunnerTrunkElementConfig();
        res.TrunkId = source.TrunkId;
        res.TrunkLength = source.TrunkLength;
        res.TrunkDiff = source.TrunkDiff;
        res.TrunkDesc = string.Empty;
        res.ItemList = new List<RunnerTrunkItemConfig>(source.ItemList);
        return res;
    }
}