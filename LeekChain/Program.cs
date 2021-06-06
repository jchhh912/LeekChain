using System;
using System.Diagnostics;
using System.Text.Json;

namespace LeekChain
{
    class Program
    {
        public static int Port { get; set; }
        public static Blockchain CurrentChain { get; set; } = new();
        public static string Name { get; set; }
        public static P2PServer server;
        public static P2PClient Client = new();
        static void Main(string[] args)
        {
          
            //Blockchain leekCoin = new Blockchain();
            ////Stopwatch sw = new();
            ////sw.Start();
            ////leekCoin.AddBlock(new Block(DateTime.UtcNow, null, "test1"));
            ////leekCoin.AddBlock(new Block(DateTime.UtcNow, null, "test2"));
            ////leekCoin.AddBlock(new Block(DateTime.UtcNow, null, "test3"));
            ////sw.Stop();
            //// Console.WriteLine(sw.ElapsedMilliseconds);


            //leekCoin.CreateTransaction(new Transaction("Jiang","Peter",250));
            //leekCoin.ProcessPendingTransaltion("Edi");
            //leekCoin.CreateTransaction(new Transaction("Peter", "Jiang", 200));
            //leekCoin.ProcessPendingTransaltion("Edi");
            //Console.WriteLine($"{leekCoin.GetBalance("Jiang")}");
            //Console.WriteLine($"{leekCoin.GetBalance("Peter")}");
            //Console.WriteLine($"{leekCoin.GetBalance("Edi")}");
            //var json = JsonSerializer.Serialize(leekCoin, new JsonSerializerOptions() { WriteIndented = true });
            //Console.WriteLine(json);
            ////验证是否合法
            //Console.WriteLine($"Is Chain Valid:{ leekCoin.IsValid()}");
            // //leekCoin.Chain[1].Hash = "123456";
            ////Console.WriteLine($"Is Chain Valid:{ leekCoin.IsValid()}");
            //Console.ReadLine();
            CurrentChain.InitializeChain();
            if (args.Length>=1)
            {
                Port = int.Parse(args[0]);
            }
            if (args.Length >= 2)
            {
                Name = args[1];
            }
            if (Port>0)
            {
                server = new P2PServer();
                server.Start();
            }
            Console.WriteLine($"Current user is {Name}");
            Console.WriteLine("-------------------------");
            Console.WriteLine("1.连接到服务器");
            Console.WriteLine("2.添加交易 ");
            Console.WriteLine("3.显示区块链信息");
            Console.WriteLine("4.退出");
            Console.WriteLine("-------------------------");
            int selection = 0;
            while (selection!=4)
            {
                switch (selection)
                {
                    case 1:
                        Console.WriteLine("输入服务器URL");
                        string serverUrl = Console.ReadLine();
                        Client.Connect($"{serverUrl}/Blockchain");
                        break;
                    case 2:
                        Console.WriteLine("输入转账账户");
                        string receiverName = Console.ReadLine();
                        Console.WriteLine("转账数量");
                        string amount = Console.ReadLine();
                        CurrentChain.CreateTransaction(new Transaction(Name,receiverName,int.Parse(amount)));
                        CurrentChain.ProcessPendingTransaltion(Name);
                        Client.Broadcast(JsonSerializer.Serialize(CurrentChain));
                        break;
                    case 3:
                        Console.WriteLine("当前区块所有信息");
                        Console.WriteLine(JsonSerializer.Serialize(CurrentChain,new JsonSerializerOptions() { WriteIndented=true}));
                        break;
                }
                Console.WriteLine("再次输入");
                string action = Console.ReadLine();
                selection = int.Parse(action);
            }
            Client.Close();
        }
    }
}
