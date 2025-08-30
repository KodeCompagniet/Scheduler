using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DemiCode.Scheduling.Test.Mocks
{
    /// <summary>
    /// Class that enables testing of Factory.CreateInstance.
    /// </summary>
    internal class ConfigurableMock : IConfigurableMock
    {
        private XmlNode _configurationNode;

        public XmlNode ConfigurationNode
        {
            get { return _configurationNode; }
            set { _configurationNode = value; }
        }

        public ConfigurableMock(XmlNode configurationNode)
        {
            _configurationNode = configurationNode;
        }
    }
}
