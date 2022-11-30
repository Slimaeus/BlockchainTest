using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Lib
{
    public class Block<T> where T : class
    {
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public T Data { get; set; }
        public long TimeStamp { get; set; }
        //private int nonce = 0;
        // Block Constructor
        public Block(T data, string previousHash, long timeStamp)
        {
            Data = data;
            PreviousHash = previousHash;
            TimeStamp = timeStamp;
            Hash = CalculateHash();
        }
        public static long GetTime(DateTime date)
        {
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (date.ToUniversalTime() - st);
            long retval = (long)(t.TotalMilliseconds + 0.5);
            return retval;
        }
        public static long GetTime()
        {
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
            long retval = (long)(t.TotalMilliseconds + 0.5);
            return retval;
        }
        public string CalculateHash()
        {
            var sha256 = new HashSha256();
            var calculatedhash = sha256.Hash(
                    PreviousHash +
                    TimeStamp.ToString() +
                    //nonce.ToString() +
                    Data.ToString());
            return calculatedhash;
        }
        public bool IsGenesisBlock()
        {
            return PreviousHash == "0";
        }
        public bool IsHashCorrect(string hash)
        {
            return Hash == hash;
        }
        public void MineBlock(int difficulty)
        {
            var target = new String(new char[difficulty]).Replace('\0', '0'); //Create a string with difficulty * "0" 
            while (!Hash.Substring(0, difficulty).Equals(target))
            {
                //nonce++;
                Hash = CalculateHash();
            }
            Console.WriteLine("Block Mined!!! : " + Hash);
        }
        public class HashSha256
        {
            public string Hash(string strInput)
            {
                try
                {
                    var crypt = SHA256.Create(); ;
                    var hash = new StringBuilder();
                    byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(strInput));
                    foreach (byte theByte in crypto)
                    {
                        hash.Append(theByte.ToString("x2"));
                    }
                    return hash.ToString();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
