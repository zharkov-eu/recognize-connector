using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public struct FuzzyStatementResult
    {
        public double Degree;
        public string SocketName;        
    }

    public class FuzzyStatement
    {
        public HashSet<FuzzyFactClustered> Facts;
        public FuzzyStatementResult Result;
    }
}