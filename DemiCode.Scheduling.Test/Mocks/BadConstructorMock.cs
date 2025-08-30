using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace DemiCode.Scheduling.Test.Mocks
{
    /// <summary>
    /// Class that enables testing of Factory.CreateInstance.
    /// </summary>
    internal class BadConstructorMock : IConfigurableMock
    {
        private XmlNode _configurationNode;

        public XmlNode ConfigurationNode
        {
            get { return _configurationNode; }
            set { _configurationNode = value; }
        }

        public BadConstructorMock(XmlNode configurationNode)
        {
            throw new InvalidOperationException();
        }
    }
}
