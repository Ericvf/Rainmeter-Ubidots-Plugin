using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ubidots
{
    public class DataSource : ApiObject
    {
        public DataSource(Dictionary<string, object> Raw, ApiClient Api) : base(Raw, Api) { }

        /// <summary>
        /// Get the name of the Datasource
        /// </summary>
        /// <returns>The name of the Datasource</returns>
        public string GetName()
        {
            return GetAttributeString("name");
        }

        /// <summary>
        /// Deletes the Datasource and all its contents.
        /// </summary>
        public void Remove()
        {
            Bridge.Delete("datasources/" + GetId());
        }

        /// <summary>
        /// Get the Variables of a Datasource
        /// </summary>
        /// <returns>The list of Variables in a Datasource</returns>
        public Variable[] GetVariables()
        {
            string Json = Bridge.Get("datasources/" + GetId() + "/variables");

            List<Dictionary<string, object>> RawValues =
                JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(Json);

            Variable[] Variables = new Variable[RawValues.Count];

            for (var i = 0; i < RawValues.Count; i++)
            {
                Variables[i] = new Variable(RawValues[i], Api);
            }

            return Variables;
        }

        /// <summary>
        /// Creates a new Variable in the Datasource
        /// </summary>
        /// <param name="Name">The name of the new Variable</param>
        /// <returns>The newly created Variable</returns>
        public Variable CreateVariable(string Name)
        {
            return CreateVariable(Name, null, null, null, null);
        }

        /// <summary>
        /// Creates a new Variable in the Datasource
        /// </summary>
        /// <param name="Name">The name of the new Variable</param>
        /// <param name="Unit">The units of the new Variable</param>
        /// <param name="Description">The description of the new 
        /// variable</param>
        /// <param name="Properties">The properties of the new Variable</param>
        /// <param name="Tags">The tags of the new Variable</param>
        /// <returns>The newly created Variable</returns>
        public Variable CreateVariable(string Name, string Unit, 
            string Description, Dictionary<string, string> Properties, 
            string[] Tags)
        {
            if (Name == null)
            {
                throw new ArgumentNullException();
            }

            Dictionary<string, object> Data = new Dictionary<string, object>();
            Data.Add("name", Name);

            if (Unit != null)
                Data.Add("unit", Unit);

            if (Description != null)
                Data.Add("description", Description);

            if (Properties != null)
                Data.Add("properties", Properties);

            if (Tags != null)
                Data.Add("tags", Tags);

            string Json = Bridge.Post("datasources/" + GetId() + "/variables",
                JsonConvert.SerializeObject(Data));

            Variable Var = new Variable(JsonConvert.DeserializeObject<Dictionary<string, object>>(Json), Api);

            return Var;
        }
    }
}
