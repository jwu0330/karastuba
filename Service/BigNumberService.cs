using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using System.Diagnostics;

namespace k7.Service
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
                chunkStr = (GetChunkValue(A, indexA) + GetChunkValue(B, indexB) + ((long)carry)).ToString();
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
                chunkNumber = ((long)GetChunkValue(A, indexA)) - ((long)GetChunkValue(B, indexB)) - ((long)borrow);
                if (chunkNumber < 0)
                {
                    borrow = 1;
                    chunkNumber += (long)Math.Pow(10, ChunkSize);
                }
                else
                {
                    borrow = 0;
                }
                chunks[i] = (chunkNumber).ToString().PadLeft(ChunkSize, '0');
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


                        //string Z0_plus_Z2 = Add(Z0, Z2);
                        //if (int.Parse(Z1_temp) < int.Parse(Z0_plus_Z2))
                        
            string Z1 = Subtract(Subtract(Z1_temp, Z0), Z2);
            //string Z1 = sub(sub(Z1_temp, Z0), Z2);

            // 模擬 ×10^(2m) 和 ×10^m
            string Z0_shifted = Z0 + new string('0', 2 * m);
            string Z1_shifted = Z1 + new string('0', m);
            string result = Add(Add(Z0_shifted, Z1_shifted), Z2);
            return result;

            //Z1 = Z1.PadLeft(m, '0');
            //Z2 = Z2.PadLeft(m, '0');
            //return Z0 + Z1 + Z2;

        }
       


        private static void Initialization(string A, string B, int maxLen)
        {
            s1 = A.PadLeft(maxLen, '0');
            s2 = B.PadLeft(maxLen, '0');
        }

    }
}




//public static string Multiply(string A, string B)
//{
//    int maxLen = Math.Max(A.Length, B.Length);
//    maxLen += maxLen % 2;
//    Initialization(A, B, maxLen);  // 對 s1 和 s2 補齊長度

//    //Console.WriteLine($"\n[Multiply] s1 = {s1}, s2 = {s2}, maxLen = {maxLen}");

//    // 基底情況：兩數都很小，直接乘
//    if (maxLen <= 8)
//    {
//        ulong _result = ulong.Parse(s1) * ulong.Parse(s2);
//        //Console.WriteLine($"[Base Case] {s1} * {s2} = {_result}");
//        return _result.ToString();
//    }

//    int m = maxLen / 2;
//    string s1L = s1[..m], s1R = s1[m..];
//    string s2L = s2[..m], s2R = s2[m..];

//    // 遞迴呼叫 Z0
//    //Console.WriteLine($"\n→ 呼叫 Z0 = Multiply(左左) → Multiply({s1L}, {s2L})");
//    string Z0 = Multiply(s1L, s2L);

//    // 遞迴呼叫 Z2
//    //Console.WriteLine($"\n→ 呼叫 Z2 = Multiply(右右) → Multiply({s1R}, {s2R})");
//    string Z2 = Multiply(s1R, s2R);

//    // 呼叫 Add：左 + 右
//    //Console.WriteLine($"\n→ 呼叫 A1 = Add(s1L + s1R) = Add({s1L}, {s1R})");
//    string A1 = Add(s1L, s1R);

//    //Console.WriteLine($"→ 呼叫 B1 = Add(s2L + s2R) = Add({s2L}, {s2R})");
//    string B1 = Add(s2L, s2R);

//    // 呼叫中間交叉乘積
//    //Console.WriteLine($"\n→ 呼叫 Z1_temp = Multiply(交叉) → Multiply({A1}, {B1})");
//    string Z1_temp = Multiply(A1, B1);

//    // 呼叫 Sub
//    //Console.WriteLine($"\n→ 呼叫 Z1 = sub(sub(Z1_temp, Z0), Z2)");
//    //Console.WriteLine($"   Z1_temp = {Z1_temp}");
//    //Console.WriteLine($"   Z0 = {Z0}");
//    //Console.WriteLine($"   Z2 = {Z2}");

//    string Z1 = sub(sub(Z1_temp, Z0), Z2);

//    // 模擬位移乘法
//    string Z0_shifted = Z0 + new string('0', 2 * m);
//    string Z1_shifted = Z1 + new string('0', m);

//    // 加總最後的三部分
//    //Console.WriteLine($"\n→ 組合結果 = Add(Add(Z0_shifted, Z1_shifted), Z2)");
//    //Console.WriteLine($"   Z0_shifted = {Z0_shifted}");
//    //Console.WriteLine($"   Z1_shifted = {Z1_shifted}");
//    //Console.WriteLine($"   Z2 = {Z2}");

//    string sresult = Add(Add(Z0_shifted, Z1_shifted), Z2);

//    //Console.WriteLine($"[完成] {A} * {B} = {sresult}");

//    return sresult;
//}




//public static string sub(string a, string b)
//{
//    try
//    {
//        // 將字串轉為 BigInteger
//        BigInteger numA = BigInteger.Parse(a);
//        BigInteger numB = BigInteger.Parse(b);

//        // 相減
//        BigInteger result = numA - numB;

//        // 回傳結果字串
//        return result.ToString();
//    }
//    catch (FormatException)
//    {
//        return "❌ 錯誤：請確認輸入為有效的整數字串";
//    }
//}




//public static string Subtract(string A, string B)
//{
//    //int borrow = 0, chunkNumber;
//    int borrow = 0;
//    long chunkNumber;
//    A = A.PadLeft(Math.Max(A.Length, B.Length), '0');
//    Console.WriteLine($"[Subtract] After Pad: A.Length={A.Length}, B.Length={B.Length}");

//    B = B.PadLeft(Math.Max(A.Length, B.Length), '0');
//    Console.WriteLine($"[Subtract] After Pad: chunks = {Math.Max(A.Length, B.Length) / ChunkSize + 1}");
//    // 決定要切成幾塊
//    string[] chunks = new string[Math.Max(A.Length, B.Length) / ChunkSize + 1];
//    Console.WriteLine($"[Subtract] chunks.Length={chunks.Length}");
//    int indexA = A.Length - ChunkSize, indexB = B.Length - ChunkSize;
//    Console.WriteLine($"[Subtract] initial indexA={indexA}, indexB={indexB}");


//    for (int i = chunks.Length - 1; i >= 0; i--, indexA -= ChunkSize, indexB -= ChunkSize)
//    {
//        Console.WriteLine($"[Subtract] chunk #{i}: indexA={indexA}, indexB={indexB}");

//        // 當前塊的減法：A塊 - B塊 - 借位
//        chunkNumber = ((long)GetChunkValue(A, indexA)) - ((long)GetChunkValue(B, indexB)) - ((long)borrow);
//        if (chunkNumber < 0)
//        {
//            borrow = 1;
//            chunkNumber += (long)Math.Pow(10 ,ChunkSize);
//        }
//        else
//        {
//            borrow = 0;
//        }
//        chunks[i] = (chunkNumber).ToString().PadLeft(ChunkSize, '0');
//    }
//    // 拼接結果
//    string all = string.Concat(chunks).TrimStart('0');
//    return all == "" ? "0" : all;
//}








//private static long GetChunkValue(string s, int index)
//{
//    Console.WriteLine($"[GetChunkValue] called by {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}, s.Length={s.Length}, index={index}");

//    //Console.WriteLine($"[GetChunkValue] s = \"{s}\", index = {index}");
//    //Console.WriteLine($"[GetChunkValue] s.Length={s.Length}, index={index}");
//    // 若 index 為負，代表該區間不足 ChunkSize
//    if (index < 0)
//    {
//        //Console.WriteLine($"→ index < 0 成立，ChunkSize + index = {ChunkSize + index}");

//        if (ChunkSize + index <= 0)
//        {
//            //Console.WriteLine($"→ ChunkSize + index <= 0，回傳 0");
//            return 0;  // index 為負，相當於減少位數
//        }

//        // 從字串開頭取得剩餘部分，並以'0'左側補齊
//        string chunk = s[..(ChunkSize + index)].PadLeft(ChunkSize, '0');
//        //Console.WriteLine($"→ 從 s[0..{ChunkSize + index}] 取出 \"{s[..(ChunkSize + index)]}\"，PadLeft 後為 \"{chunk}\"");

//        return long.Parse(chunk);
//    }

//    // 固定長度區間
//    string normalChunk = s[index..(index + ChunkSize)];
//    //Console.WriteLine($"→ 固定區段 s[{index}..{index + ChunkSize}] = \"{normalChunk}\"");

//    return long.Parse(normalChunk);
//}