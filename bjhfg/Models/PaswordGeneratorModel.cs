using System;
using System.Linq;
using System.Text;

namespace PasswordGenerator.Models;

public class PasswordGeneratorModel
{
    private static readonly Random _rng = new();

   
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*()-_=+[]{};:,.<>?";

    
    private const string SimilarChars = "0O1lI";

    public string Generate(int length, bool useUpper, bool useLower,
                          bool useDigits, bool useSpecial, bool excludeSimilar)
    {
        var pool = new StringBuilder();

        if (useUpper) pool.Append(UpperCase);
        if (useLower) pool.Append(LowerCase);
        if (useDigits) pool.Append(Digits);
        if (useSpecial) pool.Append(SpecialChars);

        if (pool.Length == 0)
            throw new InvalidOperationException("Выберите хотя бы один набор символов");

        string charPool = pool.ToString();

        if (excludeSimilar)
        {
            charPool = new string(charPool.Where(c => !SimilarChars.Contains(c)).ToArray());
            if (string.IsNullOrEmpty(charPool))
                throw new InvalidOperationException("После исключения похожих символов не осталось доступных символов");
        }

        return new string(Enumerable.Range(0, length)
            .Select(_ => charPool[_rng.Next(charPool.Length)])
            .ToArray());
    }

   
    public double CalculateEntropy(int length, bool useUpper, bool useLower,
                                  bool useDigits, bool useSpecial, bool excludeSimilar)
    {
        int poolSize = 0;
        if (useUpper) poolSize += UpperCase.Length;
        if (useLower) poolSize += LowerCase.Length;
        if (useDigits) poolSize += Digits.Length;
        if (useSpecial) poolSize += SpecialChars.Length;

        if (excludeSimilar)
        {
            poolSize -= SimilarChars.Length;
        }

        if (poolSize <= 0) return 0;

        return length * Math.Log2(poolSize);
    }
}