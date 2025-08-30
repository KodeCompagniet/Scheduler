using System;
using System.Xml;
namespace DemiCode.Scheduling.Test.Mocks
{
    internal interface IConfigurableMock
    {
        XmlNode ConfigurationNode { get; set; }
    }

    internal interface ISecondConfigurableMock
    {
        XmlNode ConfigurationNode { get; set; }
    }
}
