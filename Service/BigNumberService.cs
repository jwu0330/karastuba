using System.Linq;

namespace karastsuba.Service
{
    public class BigNumberService
    {
        private const int ChunkSize = 8;
        //private string  ChunkSize = 8;
        private static string? s1, s2;
        public static void Initial(string A, string B)
        {

        }

        public static string Add(string A, string B)
        {

            A = A.PadLeft(Math.Max(A.Length, B.Length), '0');
            int indexA = A.Length - ChunkSize, indexB = B.Length - ChunkSize;
            int carry = 0;
            string chunkStr;
            string[] chunks = new string[Math.Max(A.Length, B.Length) / ChunkSize + 1];

            for (int i = chunks.Length - 1; i >= 0; i--, indexA -= ChunkSize, indexB -= ChunkSize)
            {
                // 若 index 超出字串範圍，則 GetChunkValue 會補足不足的部分              
                chunkStr = (GetChunkValue(A, indexA) + GetChunkValue(B, indexB) + carry).ToString();
                if (chunkStr.Length > ChunkSize)
                {
                    carry = 1;
                    // 取後面 ChunkSize 位並補滿
                    chunks[i] = chunkStr[^ChunkSize..].PadLeft(ChunkSize, '0');
                }
                else
                {
                    carry = 0;
                    // 不足的左側補零
                    chunks[i] = chunkStr.PadLeft(ChunkSize, '0');
                }
            }
            string result = string.Concat(chunks).TrimStart('0');
            return result == "" ? "0" : result;
        }
        public static string Subtract(string A, string B)
        {
            //int borrow = 0, chunkNumber;
            int borrow = 0;
            long chunkNumber;
            A = A.PadLeft(Math.Max(A.Length, B.Length), '0');
            B = B.PadLeft(Math.Max(A.Length, B.Length), '0');
            // 決定要切成幾塊
            string[] chunks = new string[Math.Max(A.Length, B.Length) / ChunkSize + 1];
            int indexA = A.Length - ChunkSize, indexB = B.Length - ChunkSize;


            for (int i = chunks.Length - 1; i >= 0; i--, indexA -= ChunkSize, indexB -= ChunkSize)
            {
                // 當前塊的減法：A塊 - B塊 - 借位
                chunkNumber = GetChunkValue(A, indexA) - GetChunkValue(B, indexB) - borrow;
                if (chunkNumber < 0)
                {
                    borrow = 1;
                    chunkNumber += (long)Math.Pow(10, ChunkSize);
                }
                else
                {
                    borrow = 0;
                }
                chunks[i] = chunkNumber.ToString().PadLeft(ChunkSize, '0');
            }
            // 拼接結果
            string all = string.Concat(chunks).TrimStart('0');
            return all == "" ? "0" : all;
        }

        private static long GetChunkValue(string s, int index)
        {
            // 若 index 為負，代表該區間不足 ChunkSize
            if (index < 0)
            {
                if (index + ChunkSize <= 0) return 0;  // index 為負，相當於減少位數
                // 從字串開頭取得剩餘部分，並以'0'左側補齊
                string chunk = s[..(ChunkSize + index)].PadLeft(ChunkSize, '0');
                return long.Parse(chunk);
            }
            // 固定長度區間
            return long.Parse(s[index..(index + ChunkSize)]);
        }

        public static string Multiply(string A, string B)
        {
            int maxLen = Math.Max(A.Length, B.Length);
            maxLen += maxLen % 2;
            Initialization(A, B, maxLen);

            // 基底情況：兩數都很小，直接乘
            if (maxLen <= 8)
                return (long.Parse(s1) * long.Parse(s2)).ToString();

            int m = maxLen / 2;
            string s1L = s1[..m], s1R = s1[m..], s2L = s2[..m], s2R = s2[m..];
            // 遞迴
            string Z0 = Multiply(s1L, s2L), Z2 = Multiply(s1R, s2R);
            string Z1_temp = Multiply(Add(s1L, s1R), Add(s2L, s2R));
            string Z1 = Subtract(Subtract(Z1_temp, Z0), Z2);

            // 模擬 ×10^(2m) 和 ×10^m
            string Z0_shifted = Z0 + new string('0', 2 * m);
            string Z1_shifted = Z1 + new string('0', m);
            string result = Add(Add(Z0_shifted, Z1_shifted), Z2);
            return result;
        }
       
        private static void Initialization(string A, string B, int maxLen)
        {
            s1 = A.PadLeft(maxLen, '0');
            s2 = B.PadLeft(maxLen, '0');
        }

    }
}
