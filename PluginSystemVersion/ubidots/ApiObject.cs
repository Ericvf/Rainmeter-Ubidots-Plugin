using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubidots
{
    public class ApiObject
    {
        private Dictionary<string, object> Raw;
        protected ApiClient Api;
        protected ServerBridge Bridge;

        public ApiObject(Dictionary<string, object> Raw, ApiClient Api)
        {
            this.Raw = Raw;
            this.Api = Api;
            Bridge = Api.GetServerBridge();
        }

        protected string GetAttributeString(string Attribute)
        {
            return (string)Raw[Attribute];
        }

        protected double GetAttributeDouble(string Attribute)
        {
            return (double)Raw[Attribute];
        }

        public object GetAttribute(string Attribute)
        {
            return Raw[Attribute];
        }

        protected string GetId()
        {
            return (string)Raw["id"];
        }
    }
}
