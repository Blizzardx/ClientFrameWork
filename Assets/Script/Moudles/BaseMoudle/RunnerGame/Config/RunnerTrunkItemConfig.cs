using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class RunnerTrunkItemConfig : XmlConfigBase
{
    [XmlAttribute("itemName")]
    public string ItemName { get; set; }

    [XmlAttribute("itemOffsetX")]
    public int ItemOffsetX { get; set; }

    [XmlAttribute("itemOffsetY")]
    public int ItemOffsetY { get; set; }
}
