using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WCFDynamicFields.Contracts.DataContracts
{
    public partial class Tank
    {
        [XmlIgnore]
        [JsonIgnore]
        public bool SpeedSpecified { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool WeightSpecified { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool CanDiveSpecified { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool TankCollectionSpecified { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool NameSpecified { get; set; }
    }
}
