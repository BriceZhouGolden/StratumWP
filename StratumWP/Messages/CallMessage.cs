using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratumWP.Messages
{
    public class CallMessage : BaseMessage
    {
        public string Method
        {
            get { return (string)base["method"] ?? ""; }
            set { base["method"] = value; }
        }

        public JArray Params
        {
            get { return (JArray)base["params"]; }
            set { base["params"] = value; }
        }

        public CallMessage(string json)
            : base(json) { }

        public CallMessage(string method, IEnumerable<object> param)
            : this(0, method, param) { }

        public CallMessage(long id, string method, IEnumerable<object> param)
            : base(id)
        {
            Method = method;

            if (param == null) param = new List<object>();
            Params = new JArray(param);
        }

        public void AddParam(string param)
        {
            Params.Add(param);
        }

        public void AddParams(IEnumerable<string> param)
        {
            foreach (var p in param)
                Params.Add(p);
        }
    }
}
