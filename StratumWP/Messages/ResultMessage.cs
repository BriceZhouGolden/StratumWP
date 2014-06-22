using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratumWP.Messages
{
    public class ResultMessage : BaseMessage
    {
        public JArray Result
        {
            get
            {
                if (base["result"] == null)
                    return new JArray();

                if (base["result"] is JArray)
                    return (JArray)base["result"];

                return new JArray() { base["result"] };
            }
        }

        public ResultMessage(string json)
            : base(json) { }
    }
}
