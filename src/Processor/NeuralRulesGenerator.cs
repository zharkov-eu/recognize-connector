using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;
using ExpertSystem.Models.FuzzyLogic;
using ExpertSystem.Services;
using ExpertSystem.Utils;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
    public class NeuralRulesGenerator
    {
        private readonly FuzzyRulesGenerator _generator = new FuzzyRulesGenerator();

        public List<FuzzyRuleStatement> GetNeuralFuzzyRuleStatements(List<CustomSocket> sockets)
        {
            var statements = new List<FuzzyRuleStatement>();
            var domains = _generator.GetFuzzyDomains(sockets);
            var funcStatements = _generator.GetFuzzyFuncStatements(domains);
            var resultFacts = new Dictionary<CustomSocket, List<FuzzyFact>>();
            foreach (var socket in sockets)
            {
                if (socket.NumberOfContacts == 0 || socket.SizeWidth == 0 || socket.SizeLength == 0)
                    continue;
                resultFacts.Add(socket, _generator.GetAmperageCircuitFact(funcStatements, socket));
            }

            return null;
        }
    }
}