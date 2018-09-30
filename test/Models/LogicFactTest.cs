using System.Collections.Generic;
using Xunit;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Models
{
    public class LogicFactTest
    {
        [Fact]
        public void CojunctionNormalForm_IsCorrectWithSingleFact()
        {
            // Arrange
            LogicFact singleFact = new LogicFact("domain", "value", Operations.None, true);
            LinkedList<LogicFact> singleFacts = new LinkedList<LogicFact>();
            singleFacts.AddLast(singleFact);

            // Act
            LinkedList<LogicFact> cnfStatement = LogicFact.ConjuctionNormalFrom(singleFacts);
            
            // Assert
            Assert.True(singleFact.Equals(cnfStatement.First.Value), "Исходный факт не соответвует полученному");
        }
        
        [Fact]
        public void CojunctionNormalForm_IsCorrectWithDisjunction()
        {
            // Arrange
            LogicFact disjunctionFactA = new LogicFact("domainA", "valueA", Operations.Disjunction);
            LogicFact disjunctionFactB = new LogicFact("domainB", "valueB", Operations.None);
            LinkedList<LogicFact> disjunctionFacts = new LinkedList<LogicFact>();
            disjunctionFacts.AddLast(disjunctionFactA);
            disjunctionFacts.AddLast(disjunctionFactB);

            // Act
            disjunctionFacts = LogicFact.ConjuctionNormalFrom(disjunctionFacts);

            Assert.True(disjunctionFactA.Equals(disjunctionFacts.First.Value));
            Assert.True(disjunctionFactB.Equals(disjunctionFacts.Last.Value));
        }

        [Fact]
        public void CojunctionNormalForm_IsCorrectWithImplication()
        {
            LogicFact implicationFactA = new LogicFact("domainA", "valueA", Operations.Conjunction);
            LogicFact implicationFactB = new LogicFact("domainB", "valueB", Operations.Implication);
            LogicFact implicationResult = new LogicFact("domainC", "valueC", Operations.None);
            LinkedList<LogicFact> implicationFacts = new LinkedList<LogicFact>();
            implicationFacts.AddLast(implicationFactA);
            implicationFacts.AddLast(implicationFactB);
            implicationFacts.AddLast(implicationResult);
            implicationFacts = LogicFact.ConjuctionNormalFrom(implicationFacts);

            LogicFact cnfImplicationFactA = new LogicFact("domainA", "valueA", Operations.Disjunction, true);
            LogicFact cnfImplicationFactB = new LogicFact("domainB", "valueB", Operations.Disjunction, true);

            Assert.True(cnfImplicationFactA.Equals(implicationFacts.First.Value));
            Assert.True(cnfImplicationFactB.Equals(implicationFacts.First.Next.Value));
            Assert.True(implicationResult.Equals(implicationFacts.Last.Value));
        }
    }
}