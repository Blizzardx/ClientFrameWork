using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("trunkTable")]
public class RunnerTrunkTableConfig : XmlConfigBase
{
    [XmlArray("trunk")]
    public List<RunnerTrunkConfig> TrunkList { get; set; }
    
}