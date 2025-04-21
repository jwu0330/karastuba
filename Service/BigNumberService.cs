using System;
using System.Linq;

namespace karastsuba.Service
{
    public class BigNumberService
    {
        private const int ChunkSize = 8;
        public static void Initial(ref string A, ref string B, out int indexA, out int indexB, out int temp, out string chunkStr, out string[] chunks)
        {
            A = A.PadLeft(Math.Max(A.Length, B.Length), '0');
            B = B.PadLeft(Math.Max(A.Length, B.Length), '0');
            indexA = A.Length - ChunkSize;
            indexB = B.Length - ChunkSize;
            temp = 0;
            chunkStr = string.Empty;
            chunks = new string[Math.Max(A.Length, B.Length) / ChunkSize + 1];
        }

        public static string Add(string A, string B)
        {
            Initial(ref A, ref B, out int indexA, out int indexB, out int carry, out string chunkStr, out string[] chunks);

            for (int i = chunks.Length - 1; i >= 0; i--, indexA -= ChunkSize, indexB -= ChunkSize)
            {
                chunkStr = (GetChunkValue(A, indexA) + GetChunkValue(B, indexB) + carry).ToString();
                if (chunkStr.Length > ChunkSize)
                {
                    carry = 1;
                    chunks[i] = chunkStr[^ChunkSize..].PadLeft(ChunkSize, '0'); // 取後面 ChunkSize 位並補滿
                }
                else
                {
                    carry = 0;                    
                    chunks[i] = chunkStr.PadLeft(ChunkSize, '0');
                }
            }
            string result = string.Concat(chunks).TrimStart('0');
            return result == "" ? "0" : result;
        }
        public static string Sub(string A, string B)
        {
            Initial(ref A, ref B, out int indexA, out int indexB, out int borrow, out string chunkStr, out string[] chunks);
            long chunkNumber;
           
            for (int i = chunks.Length - 1; i >= 0; i--, indexA -= ChunkSize, indexB -= ChunkSize)
            {
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
            string all = string.Concat(chunks).TrimStart('0');
            return all == "" ? "0" : all;
        }
        public static string Multiply(string A, string B)
        {
            int maxLen = Math.Max(A.Length, B.Length);
            maxLen += maxLen % 2;
            Initialization(A, B, maxLen);
               
            if (maxLen <= 8) // 基底情況：兩數都很小，直接乘
                return (long.Parse(A) * long.Parse(B)).ToString();

            int m = maxLen / 2;
            string AL = A[..m], AR = A[m..], BL = B[..m], BR = B[m..];
            
            string Z0 = Multiply(AL, BL), Z2 = Multiply(AR, BR); // 遞迴
            string Z1_temp = Multiply(Add(AL, AR), Add(BL, BR));
            string Z1 = Sub(Sub(Z1_temp, Z0), Z2);

            string Z0_shifted = Z0 + new string('0', 2 * m); // 模擬 ×10^(2m) 和 ×10^m
            string Z1_shifted = Z1 + new string('0', m);
            return Add(Add(Z0_shifted, Z1_shifted), Z2); // result
        }

        private static long GetChunkValue(string s, int index)
        {
            
            if (index < 0)// 若 index 為負，代表該區間不足 ChunkSize
            {
                if (index + ChunkSize <= 0) return 0;  // index 為負，相當於減少位數
                string chunk = s[..(ChunkSize + index)].PadLeft(ChunkSize, '0');  // 從字串開頭取得剩餘部分，並以'0'左側補齊
                return long.Parse(chunk);
            }
            return long.Parse(s[index..(index + ChunkSize)]);
        }
       
        private static void Initialization(string A, string B, int maxLen)
        {
            A = A.PadLeft(maxLen, '0');
            B = B.PadLeft(maxLen, '0');
        }
    }
}
