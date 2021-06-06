using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text.Json;
namespace LeekChain
{
   public class P2PServer:WebSocketBehavior
    {
        private bool _chainSynced;
        private WebSocketServer _wss;
        public void Start() 
        {
            var address = $"ws://127.0.0.1:{Program.Port}";
            _wss = new WebSocketServer(address);
            _wss.AddWebSocketService<P2PServer>("/Blockchain");
            _wss.Start();
            Console.WriteLine($"Server started:{address}");
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data=="Hi Server")
            {
                Console.WriteLine(e.Data);
                Send("Hi Client");
            }
            else
            {
                var newChain = JsonSerializer.Deserialize<Blockchain>(e.Data);
                if (newChain is not null && newChain.IsValid()&&newChain.Chain.Count>Program.CurrentChain.Chain.Count)
                {
                    Console.WriteLine($"{nameof(P2PServer)}::Sync new chain");
                    //保存新链未处理交易
                    var newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    //自身未处理的交易
                    newTransactions.AddRange(Program.CurrentChain.PendingTransactions);
                    newChain.PendingTransactions = newTransactions;
                    Program.CurrentChain = newChain;
                }
                if (_chainSynced)
                {
                    Console.WriteLine($"{nameof(P2PServer)}::! chainSynced,send chain");
                    Send(JsonSerializer.Serialize(Program.CurrentChain));
                    _chainSynced = true;
                }
            }
        }
    }
}
