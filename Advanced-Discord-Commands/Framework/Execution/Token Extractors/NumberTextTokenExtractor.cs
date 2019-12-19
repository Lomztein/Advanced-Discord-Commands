using System;
using System.Collections.Generic;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework.Execution.TokenExtractors
{
    public class NumberTextTokenExtractor : ITokenExtractor
    {
        public object[] ExtractTokens(string input)
        {
            DelimitingTokenExtractor delimiter = new DelimitingTokenExtractor(' ');
            object[] split = delimiter.ExtractTokens(input);
            List<object> newArgs = new List<object>();

            StringBuilder sinceLastNumber = new StringBuilder();

            for (int i = 0; i < split.Length; i++)
            {
                if (double.TryParse(split[i].ToString(), out double value))
                {
                    AddIfNonEmpty();
                    sinceLastNumber.Clear();
                    newArgs.Add(value);
                }else
                {
                    sinceLastNumber.Append(split[i].ToString());
                }
            }

            AddIfNonEmpty();

            void AddIfNonEmpty ()
            {
                if (!string.IsNullOrWhiteSpace(sinceLastNumber.ToString()))
                {
                    newArgs.Add(sinceLastNumber.ToString());
                }
            }

            return newArgs.ToArray();
        }
    }
}
