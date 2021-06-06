using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeekChain
{
   public class Transaction
    {
        /// <summary>
        /// 交易机制
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        public Transaction(string fromAddress,string toAddress,int amount) 
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
        }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public int Amount { get; set; }
    }
}
