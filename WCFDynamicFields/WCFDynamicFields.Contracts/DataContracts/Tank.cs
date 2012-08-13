using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WCFDynamicFields.Contracts.DataContracts
{
    [DataContract]
    [Serializable]
    [XmlRoot(IsNullable = false)]
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Tank
    {
        [DataMember]
        [XmlElement(Order = 1)]
        [JsonProperty]
        public int Speed { get; set; }

        [XmlElement(Order = 2)]
        [DataMember]
        [JsonProperty]
        public int Weight { get; set; }

        [XmlAttribute("CanDive")]
        [DataMember]
        [JsonProperty("@CanDive")]
        public bool CanDive { get; set; }

        [XmlElement(ElementName = "TankCollection", Order = 3)]
        [DataMember(Name = "TankCollection")]
        [JsonProperty]
        public TankCollection<string> TankCollection { get; set; }

        [XmlIgnore]
        public List<string> History { get; set; }

        [XmlElement(ElementName = "Name", Order = 4)]
        [DataMember(IsRequired = false)]
        [JsonProperty]
        public string Name { get; set; }
    }
}
