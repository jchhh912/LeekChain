using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeekChain
{
    public class Block
    {
        //序号
        public int Index { get; set; }
        //时间戳
        public DateTime TimeStamp { get; set; }
        //上一个区块的Hash
        public string PreviousHash { get; set; }
        //当前区块的Hash
        public string Hash { get; set; }
        //当前区块保存的数据
        //public string Data { get; set; }
        public IList<Transaction> Transactions { get; set; }
        //调整Nonce值满足HAsh条件的次数
        public int Nonce { get; set; } = 0;
        //Block的构造函数
        public Block(DateTime timeStamp,string previousHash, IList<Transaction> transactions)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
            Hash = CalculateHash();
        }
        /// <summary>
        /// 计算自身hash的方法
        /// </summary>
        /// <returns></returns>
        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash??string.Empty}-{JsonSerializer.Serialize(Transactions)}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToBase64String(outputBytes);
        }
        /// <summary>
        /// 挖矿函数
        /// </summary>
        /// <param name="difficulty">难度</param>
        public void Mine(int difficulty) 
        {
            var leadingZeros = new string('0',difficulty);
            while (Hash==null||Hash.Substring(0,difficulty)!=leadingZeros)
            {
                Nonce++;
                Hash = CalculateHash();
            }
        }
    }
}
