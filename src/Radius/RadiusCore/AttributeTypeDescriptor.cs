using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore
{
    public class AttributeTypeDescriptor
    {
        public byte Type { get; set; }

        public string Name { get; set; }

        public AttributeValueFormat Format { get; set; }
    }
}
