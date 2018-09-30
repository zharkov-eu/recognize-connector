using Xunit;
using System.Collections.Generic;
using static ExpertSystem.Models.LogicOperation;


namespace ExpertSystem.Models
{
    public class LogicFactTest
    {
        [Fact]
        public void ConjuctionNormalFrom_IsCorrect()
        {
            LogicFact singleFact = new LogicFact("domain", "value", Operations.None, true);
            LinkedList<LogicFact> singleFacts = new LinkedList<LogicFact>();
            singleFacts.AddLast(singleFact);

            Assert.True(singleFact.Equals(LogicFact.ConjuctionNormalFrom(singleFacts).First.Value));

            LogicFact disjuctionFactA = new LogicFact("domainA", "valueA", Operations.Disjunction);
            LogicFact disjuctionFactB = new LogicFact("domainB", "valueB", Operations.None);
            LinkedList<LogicFact> disjuctionFacts = new LinkedList<LogicFact>();
            disjuctionFacts.AddLast(disjuctionFactA);
            disjuctionFacts.AddLast(disjuctionFactB);
            disjuctionFacts = LogicFact.ConjuctionNormalFrom(disjuctionFacts);

            Assert.True(disjuctionFactA.Equals(disjuctionFacts.First.Value));
            Assert.True(disjuctionFactB.Equals(disjuctionFacts.Last.Value));
        }
    }
}