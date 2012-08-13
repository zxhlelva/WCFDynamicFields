using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WCFDynamicFields.Contracts.DataContracts
{
    [DataContract(Name = "{0}")]
    [Serializable]
    [XmlRoot("GenericNavigation", Namespace = "WWW.TEST", IsNullable = false)]
    [JsonObject]
    public class Navigation<T>
    {
        public T Obj { get; set; }
    }
}
