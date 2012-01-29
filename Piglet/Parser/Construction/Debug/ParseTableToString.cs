﻿using System.Linq;
using System.Text;
using Piglet.Parser.Configuration;

namespace Piglet.Parser.Construction.Debug
{
    public static class ParseTableToString
    {
        public static string ToDebugString<T>(this SLRParseTable<T> table, IGrammar<T> grammar, int numStates)
        {
            int numTokens = grammar.AllSymbols.Count();
            int numTerminals = grammar.AllSymbols.OfType<Terminal<T>>().Count();

            var formatString = new StringBuilder("{0,8}|");
            for (int i = 0; i < numTokens; ++i)
            {
                if (i == numTerminals)
                    formatString.Append("|"); // Extra bar to separate actions and gotos
                formatString.Append("|{" + (i + 1) + ",8}");
            }
            formatString.Append("|\n");
            string format = formatString.ToString();
            var sb = new StringBuilder();
            sb.Append(string.Format(format, new[] { "STATE" }.Concat(grammar.AllSymbols.Select(f => f.DebugName)).ToArray<object>()));
            for (int i = 0; i < numStates; ++i)
            {
                object[] formatParams = new[] { i.ToString() }.Concat(grammar.AllSymbols.OfType<Terminal<T>>().Select(f =>
                {
                    var actionValue = table.Action[i, f.TokenNumber];
                    if (actionValue == int.MaxValue)
                    {
                        return "acc";
                    }

                    if (actionValue == int.MinValue)
                    {
                        return "";
                    }

                    if (actionValue < 0)
                    {
                        return "r" + -(actionValue + 1);
                    }

                    return "s" + actionValue;
                }).Concat(grammar.AllSymbols.OfType<NonTerminal<T>>().Select(f => table.Goto[i, f.TokenNumber] ==
                                                                                      int.MinValue
                                                                                          ? ""
                                                                                          : table.Goto[i, f.TokenNumber].ToString()))).ToArray<object>();
                sb.Append(string.Format(format, formatParams));
            }
            return sb.ToString();
        }
    }
}
