using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using StratumWP;
using StratumWP.Messages;
using System.Threading.Tasks;

namespace StratumUnitTests
{
    [TestClass]
    public class CommunicationTests
    {
        StratumClient client;

        [TestInitialize]
        public async Task Prepare()
        {
            client = new StratumClient(new System.Net.DnsEndPoint("test.coinomi.com", 15001));
            await client.ConnectAsync();
        }

        [TestMethod, TestCategory("Live Communication")]
        public async Task TestCallCommand()
        {
            var msg = await client.Call(new CallMessage("blockchain.address.get_history",
                new[] { "mrx4EmF6zHXky3zDoeJ1K7KvYcuNn8Mmc4" }));

            Assert.AreEqual(msg.Result[0].Value<string>("tx_hash"),
                "3aa2a5a9825ca767e092bcc19487aa13969eeb217fd0fba8492543bbb8c30954");
            Assert.AreEqual(msg.Result[0].Value<int>("height"), 260144);
        }

        [TestMethod, TestCategory("Live Communication")]
        public async Task TestCallCommandFail()
        {
            var msg = await client.Call(new CallMessage("blockchain.dummy", new[] { "" }));

            Assert.IsTrue(msg.ErrorOccured);
            Assert.IsTrue(msg.Error.StartsWith("unknown method"));
        }

        [TestMethod, TestCategory("Live Communication")]
        public async Task TestSubscribeCommand()
        {
            var tcs = new TaskCompletionSource<ResultMessage>();

            client.Subscibe(new CallMessage("blockchain.address.get_history",
                new[] { "mrx4EmF6zHXky3zDoeJ1K7KvYcuNn8Mmc4" }), r =>
                {
                    tcs.SetResult(r);
                });

            var msg = await tcs.Task;

            Assert.AreEqual(msg.Result[0].Value<string>("tx_hash"),
                "3aa2a5a9825ca767e092bcc19487aa13969eeb217fd0fba8492543bbb8c30954");
            Assert.AreEqual(msg.Result[0].Value<int>("height"), 260144);
        }
    }
}
