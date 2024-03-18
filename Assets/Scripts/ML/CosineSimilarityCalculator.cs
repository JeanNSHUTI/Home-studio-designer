using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class CosineSimilarityCalculator : MonoBehaviour
{
    public static double CalculateCosineSimilarity(string text1, string text2)
    {
        Dictionary<string, int> tokenFrequency1 = TokenizeAndCount(text1);
        Dictionary<string, int> tokenFrequency2 = TokenizeAndCount(text2);

        double dotProduct = CalculateDotProduct(tokenFrequency1, tokenFrequency2);
        double magnitude1 = CalculateMagnitude(tokenFrequency1);
        double magnitude2 = CalculateMagnitude(tokenFrequency2);

        double cosineSimilarity = dotProduct / (magnitude1 * magnitude2);
        return cosineSimilarity;
    }

    private static Dictionary<string, int> TokenizeAndCount(string text)
    {
        string[] tokens = text.Split(new[] { ' ', ',', '.', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, int> tokenFrequency = new Dictionary<string, int>();

        foreach (string token in tokens)
        {
            string cleanedToken = token.ToLower();
            if (!tokenFrequency.ContainsKey(cleanedToken))
            {
                tokenFrequency[cleanedToken] = 1;
            }
            else
            {
                tokenFrequency[cleanedToken]++;
            }
        }

        return tokenFrequency;
    }

    private static double CalculateDotProduct(Dictionary<string, int> frequency1, Dictionary<string, int> frequency2)
    {
        double dotProduct = 0;

        foreach (var token in frequency1.Keys)
        {
            if (frequency2.ContainsKey(token))
            {
                dotProduct += frequency1[token] * frequency2[token];
            }
        }

        return dotProduct;
    }

    private static double CalculateMagnitude(Dictionary<string, int> frequency)
    {
        double magnitude = 0;

        foreach (var count in frequency.Values)
        {
            magnitude += count * count;
        }

        return Math.Sqrt(magnitude);
    }

}
