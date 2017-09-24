


   using System;

    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Configuration;
    using System.Xml;

    /// <summary>
    /// AppConfigFileSettings: This class is used to Change the AppConfigs Paramters at rumtime through User Interface
    /// </summary>
    /// <remarks></remarks>
    /// 
    /// 
    namespace AX_CRT_MAge_Connector
{
    public class AppConfigFileSettings
    {

        /// <summary>
        /// UpdateAppSettings: It will update the app.Config file AppConfig key values
        /// </summary>
        /// <param name="KeyName">AppConfigs KeyName</param>
        /// <param name="KeyValue">AppConfigs KeyValue</param>
        /// <remarks></remarks>
        public static void UpdateAppSettings(string KeyName, string KeyValue)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            foreach (XmlElement xElement in XmlDoc.DocumentElement)
            {
                if (xElement.Name == "system.serviceModel")
                {
                    foreach (XmlNode xNode in xElement.ChildNodes)
                    {
                        if (xNode.Name == "client")
                                                    {
                            if (xNode.ChildNodes[0].Attributes[0].Name == KeyName)
                            {
                                xNode.ChildNodes[0].Attributes[0].Value = KeyValue;
                            }
                        }
                    }
                }
            }
            XmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }
    }
}
