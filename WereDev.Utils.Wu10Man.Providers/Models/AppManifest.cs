using System;
using System.Xml.Serialization;

namespace WereDev.Utils.Wu10Man.Providers.Models
{
    public class Package
    {
        public ManifestProperties Properties { get; set; }

        public class ManifestProperties
        {
            public string Logo { get; set; }
        }
    }
}
