using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using StratumWP;
using StratumWP.Messages;

namespace StratumUnitTests
{
    [TestClass]
    public class CommunicationTests
    {
        [TestMethod, TestCategory("Live Communication")]
        public void TestCallCommand()
        {
            var client = new StratumClient("test.coinomi.com", 15001);
            client.ConnectAsync().Wait();
            var callTask = client.Call(new CallMessage("blockchain.address.get_history",
                new[] { "mrx4EmF6zHXky3zDoeJ1K7KvYcuNn8Mmc4" }));
            callTask.Wait();
            var msg = callTask.Result;

            Assert.AreEqual(msg.Result[0].Value<string>("tx_hash"),
                "3aa2a5a9825ca767e092bcc19487aa13969eeb217fd0fba8492543bbb8c30954");
            Assert.AreEqual(msg.Result[0].Value<int>("height"), 260144);
        }

        [TestMethod, TestCategory("Live Communication")]
        public void TestSubscribeCommand()
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<ResultMessage>();

            var client = new StratumClient("test.coinomi.com", 15001);
            client.ConnectAsync().Wait();
            client.Subscibe(new CallMessage("blockchain.address.get_history",
                new[] { "mrx4EmF6zHXky3zDoeJ1K7KvYcuNn8Mmc4" }), r =>
                {
                    tcs.SetResult(r);
                });

            tcs.Task.Wait();
            var msg = tcs.Task.Result;

            Assert.AreEqual(msg.Result[0].Value<string>("tx_hash"),
                "3aa2a5a9825ca767e092bcc19487aa13969eeb217fd0fba8492543bbb8c30954");
            Assert.AreEqual(msg.Result[0].Value<int>("height"), 260144);
        }
    }
}
