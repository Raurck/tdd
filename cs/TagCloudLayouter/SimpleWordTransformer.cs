using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TagCloudLayouter.Interfaces;

namespace TagCloudLayouter
{
    public class SimpleWordTransformer : IWordTransformer
    {
        private readonly IEnumerable<string> boringWords;

        public SimpleWordTransformer(IEnumerable<string> boringWords)
        {
            this.boringWords = boringWords?.Select(word => word.ToLower()) ?? throw new ArgumentNullException();
        }

        public string GetTransformedWord(string word)
        {
            return boringWords.Any(boringWord => boringWord == word.ToLower()) ? string.Empty : word;
        }
    }
}