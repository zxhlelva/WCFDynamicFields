using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using WCFDynamicFields.Contracts.DataContracts;
using System.Runtime.Serialization;

namespace WCFDynamicFields.CustomServiceBehaviors
{
    public class MessageFormatter : IDispatchMessageFormatter
    {
        private const string CONTENT_TYPE_XML = "text/xml";
        private const string CONTENT_TYPE_JSON = "application/Json";
        private const string CONTENT_TYPE_BINARY = "application/binary";
        private readonly IDispatchMessageFormatter originalFormatter;

        public MessageFormatter(IDispatchMessageFormatter dispatchMessageFormatter)
        {
            this.originalFormatter = dispatchMessageFormatter;
        }

        #region IDispatchMessageFormatter
        public void DeserializeRequest(Message message, object[] parameters)
        {
            this.originalFormatter.DeserializeRequest(message,parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            string alt = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["alt"];
            string action = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsoluteUri;
            switch (alt)
            {
                case "xml":
                    {
                        return this.CreateXMLResponse(messageVersion, result, action);
                    }
                case "json":
                    {
                        return this.CreateJsonResponse(result);
                    }
                case "binary":
                    {
                        return this.CreateBinaryResponse(result);
                    }
                default:
                    {
                        return this.CreateXMLResponse(messageVersion, result, action);
                        //return this.originalFormatter.SerializeReply(messageVersion, parameters, result);
                    }
            }
        }

        private  Message CreateXMLResponse(MessageVersion messageVersion,object result, string action)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(result.GetType());
                serializer.Serialize(stream, result);
                stream.Position = 0;
                using (XmlReader xmlReader = new XmlTextReader(stream))
                {
                    Message message = Message.CreateMessage(messageVersion, action, xmlReader);
                    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
                    WebOperationContext.Current.OutgoingResponse.ContentType = CONTENT_TYPE_XML;
                    return message;
                }
            }
        }

        private Message CreateJsonResponse(object result)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
                WebOperationContext.Current.OutgoingResponse.ContentType = CONTENT_TYPE_JSON;
                DataPointPermissionResolver resolver = new DataPointPermissionResolver();
                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    ContractResolver = resolver,
                    Context = new StreamingContext(new StreamingContextStates(), resolver)
                };
                string jsonString = JsonConvert.SerializeObject(result, settings);
                return WebOperationContext.Current.CreateTextResponse(jsonString);
                //JsonSerializer serializer = new JsonSerializer();
                //DataPointPermissionResolver resolver = new DataPointPermissionResolver();
                //serializer.ContractResolver = resolver;
                //serializer.Context = new StreamingContext(new StreamingContextStates(), resolver);
                //using (StreamWriter streamWriter = new StreamWriter(stream))
                //{
                //    using (JsonWriter writer = new JsonTextWriter(streamWriter))
                //    {
                //        serializer.Serialize(writer, result);
                //        stream.Position = 0;
                //        return WebOperationContext.Current.CreateStreamResponse(stream, CONTENT_TYPE_JSON);
                //    }
                //}
            }
        }

        private Message CreateBinaryResponse(object result)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, result);
            stream.Position = 0;
            return WebOperationContext.Current.CreateStreamResponse(stream, CONTENT_TYPE_BINARY);
        }

        public static string BuildXmlRootElementName(Type theType)
        {
            object[] aElementNameTemplateArray = theType.GetCustomAttributes(typeof(XmlRootAttribute), false);
            if (aElementNameTemplateArray.Length > 0
         && aElementNameTemplateArray[0] != null)
            {
                XmlRootAttribute aXmlRootAttribute = aElementNameTemplateArray[0] as XmlRootAttribute;
                if (aXmlRootAttribute != null
             && !string.IsNullOrEmpty(aXmlRootAttribute.ElementName))
                {
                    if (aXmlRootAttribute.ElementName.Contains("{")
                 && aXmlRootAttribute.ElementName.Contains("}")
                 && theType.IsGenericType)
                    {
                        Type[] aItemTypeArray = theType.GetGenericArguments();
                        List<string> aParameterList = new List<string>();
                        foreach (var aItemType in aItemTypeArray)
                        {
                            aParameterList.Add(BuildXmlRootElementName(aItemType));
                        }

                        return string.Format(aXmlRootAttribute.ElementName, aParameterList.ToArray());
                    }

                    return aXmlRootAttribute.ElementName;
                }

                return theType.Name;
            }

            return theType.Name;
        }

        public static XmlRootAttribute GetXmlRootAttribute(Type theType, bool theInherit)
        {
            XmlRootAttribute aResultXmlRootAttribute = null;
            object[] aXmlRootAttributeArray = theType.GetCustomAttributes(typeof(XmlRootAttribute), theInherit);
            if (aXmlRootAttributeArray.Length > 0
                && aXmlRootAttributeArray[0] != null)
            {
                aResultXmlRootAttribute = aXmlRootAttributeArray[0] as XmlRootAttribute;
            }

            return aResultXmlRootAttribute;
        }

        #endregion
    }
}
