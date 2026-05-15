using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SensorContracts
{
    [DataContract]
    public class ValidationFault
    {
        [DataMember]
        public string Reason { get; set; }

        [DataMember]
        public string FieldName { get; set; }
    }
}
