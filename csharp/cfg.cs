using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static void Main()
    {
        var grammar = new Dictionary<string, string[]>
        {
            {"S", new[] {"ACB", "Cbb", "Ba"}},
            {"A", new[] {"da", "BC"}},
            {"B", new[] {"g", "Є"}},
            {"C", new[] {"h", "Є"}}
        };

        var firstSet = calculateFirstSet(grammar);

        foreach (var pair in firstSet)
        {
            Console.Write("First set of " + pair.Key + ": ");
            foreach (var symbol in pair.Value)
            {
                Console.Write(symbol + " ");
            }
            Console.WriteLine();
        }
		
		var followSet = calculateFollowSet(grammar, firstSet);
		Console.WriteLine("---------------------");
		foreach (var pair in followSet)
        {
            Console.Write("First set of " + pair.Key + ": ");
            foreach (var symbol in pair.Value)
            {
                Console.Write(symbol + " ");
            }
            Console.WriteLine();
        }
    }

    public static Dictionary<string, HashSet<string>> calculateFirstSet(Dictionary<string, string[]> grammar)
    {
        var firstSet = new Dictionary<string, HashSet<string>>();
        const string epsilon = "Є";

        foreach (var nonTerminal in grammar.Keys)
        {
            firstSet[nonTerminal] = new HashSet<string>();
        }

        bool hasChanged;
        do
        {
            hasChanged = false;
            foreach (var nonTerminal in grammar.Keys)
            {
                var productions = grammar[nonTerminal];
                var oldSize = firstSet[nonTerminal].Count;
                foreach (var production in productions)
                {
                    var symbols = production.ToCharArray();
                    var allSymbolsCanProduceEpsilon = true;
                    foreach (var symbol in symbols)
                    {
                        if (grammar.ContainsKey(symbol.ToString()))
                        {
                            foreach (var s in firstSet[symbol.ToString()])
                            {
                                if (s != epsilon)
                                {
                                    firstSet[nonTerminal].Add(s);
                                }
                            }
                            if (!firstSet[symbol.ToString()].Contains(epsilon))
                            {
                                allSymbolsCanProduceEpsilon = false;
                                break;
                            }
                        }
                        else
                        {
                            firstSet[nonTerminal].Add(symbol.ToString());
                            allSymbolsCanProduceEpsilon = false;
                            break;
                        }
                    }
                    if (allSymbolsCanProduceEpsilon)
                    {
                        firstSet[nonTerminal].Add(epsilon);
                    }
                }
                if (firstSet[nonTerminal].Count != oldSize)
                {
                    hasChanged = true;
                }
            }
        } while (hasChanged);

        return firstSet;
    }
	
	public static Dictionary<string, HashSet<string>> calculateFollowSet(Dictionary<string, string[]> grammar, Dictionary<string, HashSet<string>> firstSet)
    {
        var followSet = new Dictionary<string, HashSet<string>>();
        const string epsilon = "Є";
        var startSymbol = grammar.Keys.First();

        foreach (var nonTerminal in grammar.Keys)
        {
            followSet[nonTerminal] = new HashSet<string>();
        }

        followSet[startSymbol].Add("$");

        bool hasChanged;
        do
        {
            hasChanged = false;
            foreach (var nonTerminal in grammar.Keys)
            {
                var productions = grammar[nonTerminal];
                foreach (var production in productions)
                {
                    var symbols = production.ToCharArray();
                    for (int i = 0; i < symbols.Length; i++)
                    {
                        if (grammar.ContainsKey(symbols[i].ToString()))
                        {
                            var oldSize = followSet[symbols[i].ToString()].Count;
                            if (i < symbols.Length - 1)
                            {
                                if (grammar.ContainsKey(symbols[i + 1].ToString()))
                                {
                                    foreach (var s in firstSet[symbols[i + 1].ToString()])
                                    {
                                        if (s != epsilon)
                                        {
                                            followSet[symbols[i].ToString()].Add(s);
                                        }
                                    }
                                    if (firstSet[symbols[i + 1].ToString()].Contains(epsilon))
                                    {
                                        for (int j = i + 2; j < symbols.Length; j++)
                                        {
                                            if (grammar.ContainsKey(symbols[j].ToString()))
                                            {
                                                foreach (var s in firstSet[symbols[j].ToString()])
                                                {
                                                    if (s != epsilon)
                                                    {
                                                        followSet[symbols[i].ToString()].Add(s);
                                                    }
                                                }
                                                if (!firstSet[symbols[j].ToString()].Contains(epsilon))
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                followSet[symbols[i].ToString()].Add(symbols[j].ToString());
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    followSet[symbols[i].ToString()].Add(symbols[i + 1].ToString());
                                }
                            }
                            if (i == symbols.Length - 1 || symbols.Skip(i + 1).All(symbol => firstSet.ContainsKey(symbol.ToString()) && (firstSet[symbol.ToString()]?.Contains(epsilon) ?? false)))
                            {
                                foreach (var s in followSet[nonTerminal])
                                {
                                    followSet[symbols[i].ToString()].Add(s);
                                }
                            }
                            if (followSet[symbols[i].ToString()].Count != oldSize)
                            {
                                hasChanged = true;
                            }
                        }
                    }
                }
            }
        } while (hasChanged);

        return followSet;
    }

}