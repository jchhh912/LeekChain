using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeekChain
{
    public class Blockchain
    {
        //区块
        public IList<Block> Chain { get; set; }
        //未处理完的交易
       public  IList<Transaction> PendingTransactions = new List<Transaction>();
        //计算难度
        public int Difficulty { get; set; } = 3;
        public void InitializeChain()
        {
            Chain = new List<Block>();
            AddGenesisBlock();
        }

        //奖励数量
        public int Reward => 1;
        /// <summary>
        /// 获取余额
        /// </summary>
        /// <param name="address">账户地址</param>
        /// <returns></returns>
        public int GetBalance(string address) 
        {
            int balance = 0;
            foreach (var block in Chain)
            {
                foreach (var transaction in block.Transactions)
                {
                    if (transaction.FromAddress==address)
                    {
                        balance -= transaction.Amount;
                    }
                    if (transaction.ToAddress==address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }
        /// <summary>
        /// 创建交易并给矿工奖励
        /// </summary>
        /// <param name="minerAddress"></param>
        public void ProcessPendingTransaltion(string minerAddress) 
        {
            //将所有_pendingTransactions 加入一个区块中
            var block = new Block(DateTime.UtcNow, GetLatestBlock().Hash, PendingTransactions);
            //进行挖矿
            AddBlock(block);
            //清空_pendingTransactions
            PendingTransactions = new List<Transaction>();
            //奖励矿工
            CreateTransaction(new Transaction(null, minerAddress, Reward));
        }
        /// <summary>
        /// 创建交易
        /// </summary>
        /// <param name="transaction"></param>
        public void CreateTransaction(Transaction transaction) 
        {
            Console.WriteLine(JsonSerializer.Serialize(transaction));
            PendingTransactions.Add(transaction);
        }
        /// <summary>
        /// 创世区块
        /// </summary>
        /// <returns></returns>
        public Block CreateGenesisBlock()
        {
           var block=new Block(DateTime.UtcNow, null, PendingTransactions);
            block.Mine(Difficulty);
            PendingTransactions = new List<Transaction>();
            return block;

        }
        /// <summary>
        /// 添加创世区块
        /// </summary>
        public void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }
        /// <summary>
        /// 找到区块链中最后一个区块
        /// </summary>
        /// <returns></returns>
        public Block GetLatestBlock() 
        {
            return Chain[^1];
        }
        /// <summary>
        /// 添加区块
        /// </summary>
        /// <param name="block"></param>
        public void AddBlock(Block block) 
        {
            Block latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            block.Mine(Difficulty); //添加Nonce 属性防止篡改
           // block.Hash = block.CalculateHash();  未防篡改
            Chain.Add(block);
        }
        /// <summary>
        /// 验证区块链是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsValid() 
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];
                //当前区块的Hash值是否被篡改 和生成Hash值是否一致
                if (currentBlock.Hash!=currentBlock.CalculateHash())
                {
                    return false;
                }
                //当前区块的上一个区块Hash是否和上个区块Hash值一致
                if (currentBlock.PreviousHash!=previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        
        }
    }
}
