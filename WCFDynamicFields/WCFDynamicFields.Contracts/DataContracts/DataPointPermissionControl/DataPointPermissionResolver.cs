using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace WCFDynamicFields.Contracts.DataContracts
{
    public class DataPointPermissionResolver : DefaultContractResolver
    {
        private JsonObjectContract objectContract = null;

        public override JsonContract ResolveContract(Type type)
        {
            JsonContract contract = base.ResolveContract(type);
            objectContract = contract as JsonObjectContract;
            return contract;
        }

        public void RemoveProperty(string name)
        {
            if (objectContract != null)
            {
                objectContract.Properties.Remove(objectContract.Properties.First(
                     p => p.PropertyName == name));
            }
        }
    }
}
