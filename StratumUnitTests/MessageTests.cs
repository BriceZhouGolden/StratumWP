using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using StratumWP.Messages;

namespace StratumUnitTests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void TestBaseMessage()
        {
            var msg = new BaseMessage(999);

            Assert.AreEqual(msg.Id, 999);
            Assert.AreEqual(msg.Error, "");
            Assert.AreEqual(msg.FailedRequest, "");
        }

        [TestMethod]
        public void TestBaseMessageParse()
        {
            var str = "{\"id\": 999, \"method\": \"blockchain.headers.subscribe\", \"params\": []}";
            var msg = new BaseMessage(str);

            Assert.AreEqual(msg.Id, 999);
        }

        [TestMethod]
        public void TestBaseMessageParseFailed()
        {
            string str = "{\"request\": \"Lorem ipsum dolor sit amet, consectetuer adipiscing elit.\", " +
                    "\"error\": \"bad JSON\"}";
            BaseMessage msg = new BaseMessage(str);

            Assert.IsTrue(msg.ErrorOccured);
            Assert.AreEqual(msg.Error, "bad JSON");
            Assert.AreEqual(msg.FailedRequest, "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.");
        }

        [TestMethod]
        public void TestCallMessageParse()
        {
            String str = "{\"id\": 0, \"method\": \"blockchain.headers.subscribe\", \"params\": []}";
            CallMessage call = new CallMessage(str);

            Assert.AreEqual(call.Method, "blockchain.headers.subscribe");
        }

        [TestMethod]
        public void TestCallMessage()
        {
            CallMessage call = new CallMessage(1L, "blockchain.address.listunspent",
                    new Object[] { "npF3ApeWwMS8kwXJyybPZ76vNbv5txVjDf" });

            Assert.AreEqual(call.Id, 1L);
            Assert.AreEqual(call.Method, "blockchain.address.listunspent");
            //Assert.AreEqual("{\"id\":1,\"method\":\"blockchain.address.listunspent\"," +
            //        "\"params\":[\"npF3ApeWwMS8kwXJyybPZ76vNbv5txVjDf\"]}", call.ToString());
        }

        [TestMethod]
        public void TestResultMessage()
        {
            String resultString = "{\"id\": 1, \"result\": [{" +
                    "\"tx_hash\": \"3aa2a5a9825ca767e092bcc19487aa13969eeb217fd0fba8492543bbb8c30954\", " +
                    "\"height\": 260144}]}";
            ResultMessage result = new ResultMessage(resultString);

            Assert.AreEqual(result.Id, 1L);
            Assert.IsTrue(result.Result.Count > 0);
            Assert.AreEqual(result.Result[0].Value<string>("tx_hash"),
                    "3aa2a5a9825ca767e092bcc19487aa13969eeb217fd0fba8492543bbb8c30954");
            Assert.AreEqual(result.Result[0].Value<int>("height"), 260144);
        }
    }
}
