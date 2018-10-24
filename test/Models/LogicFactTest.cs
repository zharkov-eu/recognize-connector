using Xunit;
using System.Collections.Generic;
using ExpertSystem.Aggregator.Models.CommonLogic;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Tests.Models
{
    public class LogicFactTest
    {
        [Fact]
        public void ConjunctionNormalForm_IsCorrectWithSingleFact()
        {
            // Arrange
            var singleFact = new LogicFact(SocketDomain.Color, "value", LogicOperation.Operation.None, true);
            var singleFacts = new LinkedList<LogicFact>();
            singleFacts.AddLast(singleFact);

            // Act
            var cnfStatement = LogicFact.ConjunctionNormalFrom(singleFacts);

            // Assert
            Assert.True(singleFact.Equals(cnfStatement.First.Value), "Исходный факт не соответвует полученному");
        }

        [Fact]
        public void ConjunctionNormalForm_IsCorrectWithDisjunction()
        {
            // Arrange
            var disjunctionFactA = new LogicFact(SocketDomain.Color, "valueA", LogicOperation.Operation.Disjunction);
            var disjunctionFactB = new LogicFact(SocketDomain.HousingColor, "valueB", LogicOperation.Operation.None);
            var disjunctionFacts = new LinkedList<LogicFact>();
            disjunctionFacts.AddLast(disjunctionFactA);
            disjunctionFacts.AddLast(disjunctionFactB);

            // Act
            disjunctionFacts = LogicFact.ConjunctionNormalFrom(disjunctionFacts);

            Assert.True(disjunctionFactA.Equals(disjunctionFacts.First.Value));
            Assert.True(disjunctionFactB.Equals(disjunctionFacts.Last.Value));
        }

        [Fact]
        public void ConjunctionNormalForm_IsCorrectWithImplication()
        {
            var implicationFactA = new LogicFact(SocketDomain.Material, "valueA", LogicOperation.Operation.Conjunction);
            var implicationFactB =
                new LogicFact(SocketDomain.HousingMaterial, "valueB", LogicOperation.Operation.Implication);
            var implicationResult = new LogicFact(SocketDomain.SocketName, "valueC", LogicOperation.Operation.None);
            var implicationFacts = new LinkedList<LogicFact>();
            implicationFacts.AddLast(implicationFactA);
            implicationFacts.AddLast(implicationFactB);
            implicationFacts.AddLast(implicationResult);
            implicationFacts = LogicFact.ConjunctionNormalFrom(implicationFacts);

            var cnfImplicationFactA =
                new LogicFact(SocketDomain.Material, "valueA", LogicOperation.Operation.Disjunction, true);
            var cnfImplicationFactB = new LogicFact(SocketDomain.HousingMaterial, "valueB",
                LogicOperation.Operation.Disjunction, true);

            Assert.True(cnfImplicationFactA.Equals(implicationFacts.First.Value));
            Assert.True(cnfImplicationFactB.Equals(implicationFacts.First?.Next?.Value));
            Assert.True(implicationResult.Equals(implicationFacts.Last.Value));
        }
    }
}