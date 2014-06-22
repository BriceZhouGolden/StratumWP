using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StratumWP.Messages
{
    public class BaseMessage : JObject
    {
        public long Id
        {
            get { return (long)base["id"]; }
            set { base["id"] = value; }
        }

        public string Error
        {
            get { return (string)base["error"] ?? ""; }
        }

        public bool ErrorOccured
        {
            get { return Error != ""; }
        }

        public string FailedRequest
        {
            get { return (string)base["request"] ?? ""; }
        }

        public override string ToString()
        {
            return base.ToString(Newtonsoft.Json.Formatting.None) + "\n";
        }

        public BaseMessage(long id)
        {
            Id = id;
        }

        public BaseMessage(string json)
            : base(Parse(json))
        {
            //TODO: Test if Id is null;
        }
    }
}
