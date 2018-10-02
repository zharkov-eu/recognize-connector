using System.Collections.Generic;
using Xunit;
using static ExpertSystem.Models.LogicOperation;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models
{
    public class LogicFactTest
    {
        [Fact]
        public void CojunctionNormalForm_IsCorrectWithSingleFact()
        {
            // Arrange
            LogicFact singleFact = new LogicFact(SocketDomain.Color, "value", Operation.None, true);
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
            LogicFact disjunctionFactA = new LogicFact(SocketDomain.Color, "valueA", Operation.Disjunction);
            LogicFact disjunctionFactB = new LogicFact(SocketDomain.HousingColor, "valueB", Operation.None);
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
            var implicationFactA = new LogicFact(SocketDomain.Material, "valueA", Operation.Conjunction);
            var implicationFactB = new LogicFact(SocketDomain.HousingMaterial, "valueB", Operation.Implication);
            var implicationResult = new LogicFact(SocketDomain.SocketName, "valueC", Operation.None);
            var implicationFacts = new LinkedList<LogicFact>();
            implicationFacts.AddLast(implicationFactA);
            implicationFacts.AddLast(implicationFactB);
            implicationFacts.AddLast(implicationResult);
            implicationFacts = LogicFact.ConjuctionNormalFrom(implicationFacts);

            var cnfImplicationFactA = new LogicFact(SocketDomain.Material, "valueA", Operation.Disjunction, true);
            var cnfImplicationFactB = new LogicFact(SocketDomain.HousingMaterial, "valueB", Operation.Disjunction, true);

            Assert.True(cnfImplicationFactA.Equals(implicationFacts.First.Value));
            Assert.True(cnfImplicationFactB.Equals(implicationFacts.First?.Next?.Value));
            Assert.True(implicationResult.Equals(implicationFacts.Last.Value));
        }
    }
}