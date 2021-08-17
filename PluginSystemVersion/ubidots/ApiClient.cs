using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ubidots
{
    public class ApiClient
    {
        private ServerBridge Bridge;

        public ApiClient(string ApiKey)
        {
            Bridge = new ServerBridge(ApiKey);
        }

        public ApiClient(string ApiKey, string BaseUrl)
        {
            Bridge = new ServerBridge(ApiKey, BaseUrl);
        }

        /// <summary>
        /// Gets the ServerBridge used in this ApiClient.
        /// </summary>
        /// <returns>The ServerBridge used in this ApiClient</returns>
        internal ServerBridge GetServerBridge() 
        {
            return Bridge;
        }

        /// <summary>
        /// Gets all the DataSources in the user account.
        /// </summary>
        /// <returns>A list with all the DataSources</returns>
        public DataSource[] GetDataSources()
        {
            string Json = Bridge.Get("datasources");

            List<Dictionary<string, object>> RawValues =
                JsonConvert.DeserializeObject <List<Dictionary<string, object>>>(Json);

            DataSource[] DataSources = new DataSource[RawValues.Count];

            for (var i = 0; i < RawValues.Count; i++)
            {
                DataSources[i] = new DataSource(RawValues[i], this);
            }


            return DataSources;
        }

        /// <summary>
        /// Gets a single DataSource in the user account
        /// </summary>
        /// <param name="Id">The DataSource Id</param>
        /// <returns>The DataSource wanted by the user</returns>
        public DataSource GetDataSource(string Id)
        {
            string Json = Bridge.Get("datasources/" + Id);

            Dictionary<string, object> RawValues =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(Json);

            if (RawValues.ContainsKey("detail"))
            {
                return null;
            }
            else
            {
                return new DataSource(RawValues, this);
            }
        }

        /// <summary>
        /// Creates a DataSource in the user account
        /// </summary>
        /// <param name="Name">The name of the DataSource</param>
        /// <returns>The newly created DataSource</returns>
        public DataSource CreateDataSource(string Name)
        {
            return CreateDataSource(Name, null, null);
        }

        /// <summary>
        /// Creates a DataSource in the user account
        /// </summary>
        /// <param name="Name">The name of the DataSource</param>
        /// <param name="Context">The context of the DataSource</param>
        /// <param name="Tags">The tags of the DataSource</param>
        /// <returns>The newly created DataSource</returns>
        public DataSource CreateDataSource(string Name, 
            Dictionary<string, string> Context, string[] Tags)
        {
            if (Name == null)
            {
                throw new ArgumentNullException();
            }

            Dictionary<string, object> Data = new Dictionary<string, object>();
            Data.Add("name", Name);

            if (Context != null)
                Data.Add("context", Context);

            if (Tags != null)
                Data.Add("tags", Tags);

            string Json = Bridge.Post("datasources/", 
                JsonConvert.SerializeObject(Data));

            DataSource Var = new DataSource(JsonConvert.DeserializeObject<Dictionary<string, object>>(Json), this);

            return Var;
        }

        /// <summary>
        /// Get all the variables of the user account
        /// </summary>
        /// <returns>A list with all the variables</returns>
        public Variable[] GetVariables()
        {
            string Json = Bridge.Get("variables");

            List<Dictionary<string, object>> RawValues =
                JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(Json);

            Variable[] Variables = new Variable[RawValues.Count];

            for (var i = 0; i < RawValues.Count; i++)
            {
                Variables[i] = new Variable(RawValues[i], this);
            }

            return Variables;
        }

        /// <summary>
        /// Get a specific Variable in the user account
        /// </summary>
        /// <param name="Id">The ID of the Variable</param>
        /// <returns>The Variable with that ID</returns>
        public Variable GetVariable(string Id)
        {
            string Json = Bridge.Get("variables/" + Id);

            Dictionary<string, object> RawValues =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(Json);

            if (RawValues.ContainsKey("details"))
            {
                return null;
            }
            else
            {
                return new Variable(RawValues, this);
            }
        }

        // Testing purposes
        public void SetServerBridge(ServerBridge Bridge)
        {
            this.Bridge = Bridge;
        }
    }
}
