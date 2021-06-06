using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebSocketSharp;

namespace LeekChain
{
    public class P2PClient
    {
        private readonly IDictionary<string, WebSocket> _wsDict = new Dictionary<string,WebSocket>();
        public void Connect(string url)
        {
            if (_wsDict.ContainsKey(url)) return;
            var ws = new WebSocket(url);
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();
            ws.Send("Hi Server");
            ws.Send(JsonSerializer.Serialize(Program.CurrentChain));
            _wsDict.Add(url,ws);
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Data=="Hi Client")
            {
                Console.WriteLine(e.Data);
            }
            else
            {
                var newChain = JsonSerializer.Deserialize<Blockchain>(e.Data);
                if (newChain is not null && newChain.IsValid() && newChain.Chain.Count > Program.CurrentChain.Chain.Count)
                {
                    Console.WriteLine($"{nameof(P2PClient)}::Sync new chain");
                    //保存新链未处理交易
                    var newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    //自身未处理的交易
                    newTransactions.AddRange(Program.CurrentChain.PendingTransactions);
                    newChain.PendingTransactions = newTransactions;
                    Program.CurrentChain = newChain;
                }
            }
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(string data) 
        {
            foreach (var item in _wsDict)
            {
                Console.WriteLine($"{nameof(P2PClient)}::Broadcast to {item.Key}");
                item.Value.Send(data);
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close() 
        {
            foreach (var item in _wsDict)
            {
                item.Value.Close();

            }
        }
    }

}
