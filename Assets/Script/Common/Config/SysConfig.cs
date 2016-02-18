using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GameConfigTools.Model
{
    [XmlRoot("root")]
    public class SysConfig : XmlConfigBase
    {
        [XmlElement("serverConfigPath")]
        public string ServerConfigPath { get; set; }
        [XmlElement("clientConfigPath")]
        public string ClientConfigPath { get; set; }

        [XmlElement("excelPath")]
        public string ExcelPath { get; set; }

        [XmlElement("configCenterUrl")]
        public string ConfigCenterUrl { get; set; }

        [XmlElement("uploadUrl")]
        public string UploadUrl { get; set; }


    }
}
