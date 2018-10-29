using Xunit;
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
            var singleRules = new LogicRule();
            singleRules.AddLast(singleFact);

            // Act
            var cnfStatement = LogicRule.ConjunctionNormalFrom(singleRules);

            // Assert
            Assert.True(singleFact.Equals(cnfStatement.First.Value), "Исходный факт не соответствует полученному");
        }

        [Fact]
        public void ConjunctionNormalForm_IsCorrectWithDisjunction()
        {
            // Arrange
            var disjunctionFactA = new LogicFact(SocketDomain.Color, "valueA", LogicOperation.Operation.Disjunction);
            var disjunctionFactB = new LogicFact(SocketDomain.HousingColor, "valueB", LogicOperation.Operation.None);
            var disjunctionRules = new LogicRule();
            disjunctionRules.AddLast(disjunctionFactA);
            disjunctionRules.AddLast(disjunctionFactB);

            // Act
            disjunctionRules = LogicRule.ConjunctionNormalFrom(disjunctionRules);

            Assert.True(disjunctionFactA.Equals(disjunctionRules.First.Value));
            Assert.True(disjunctionFactB.Equals(disjunctionRules.Last.Value));
        }

        [Fact]
        public void ConjunctionNormalForm_IsCorrectWithImplication()
        {
            var implicationFactA = new LogicFact(SocketDomain.Material, "valueA", LogicOperation.Operation.Conjunction);
            var implicationFactB =
                new LogicFact(SocketDomain.HousingMaterial, "valueB", LogicOperation.Operation.Implication);
            var implicationResult = new LogicFact(SocketDomain.SocketName, "valueC", LogicOperation.Operation.None);
            var implicationRules = new LogicRule();
            implicationRules.AddLast(implicationFactA);
            implicationRules.AddLast(implicationFactB);
            implicationRules.AddLast(implicationResult);
            implicationRules = LogicRule.ConjunctionNormalFrom(implicationRules);

            var cnfImplicationFactA =
                new LogicFact(SocketDomain.Material, "valueA", LogicOperation.Operation.Disjunction, true);
            var cnfImplicationFactB = new LogicFact(SocketDomain.HousingMaterial, "valueB",
                LogicOperation.Operation.Disjunction, true);

            Assert.True(cnfImplicationFactA.Equals(implicationRules.First.Value));
            Assert.True(cnfImplicationFactB.Equals(implicationRules.First?.Next?.Value));
            Assert.True(implicationResult.Equals(implicationRules.Last.Value));
        }
    }
}