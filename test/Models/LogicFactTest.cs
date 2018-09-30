using System.Collections.Generic;
using Xunit;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Models
{
    public class LogicFactTest
    {
        [Fact]
        public void ConjunctionNormalForm_IsCorrectWithSingleFact()
        {
            // Arrange
            LogicFact singleFact = new LogicFact("domain", "value", Operations.None, true);
            LinkedList<LogicFact> singleFacts = new LinkedList<LogicFact>();
            singleFacts.AddLast(singleFact);

            // Act
            LinkedList<LogicFact> cnfStatement = LogicFact.ConjunctionNormalForm(singleFacts);
            
            // Assert
            Assert.True(singleFact.Equals(cnfStatement.First.Value), "Исходный факт не соответвует полученному");
        }
        
        [Fact]
        public void ConjunctionNormalForm_IsCorrectWith2Facts()
        {
            // Arrange
            LogicFact disjunctionFactA = new LogicFact("domainA", "valueA", Operations.Disjunction);
            LogicFact disjunctionFactB = new LogicFact("domainB", "valueB", Operations.None);
            LinkedList<LogicFact> disjunctionFacts = new LinkedList<LogicFact>();
            disjunctionFacts.AddLast(disjunctionFactA);
            disjunctionFacts.AddLast(disjunctionFactB);

            // Act
            disjunctionFacts = LogicFact.ConjunctionNormalForm(disjunctionFacts);

            // Assert
            Assert.True(disjunctionFactA.Equals(disjunctionFacts.First.Value), "Первый исходный факт не сооветвует первому полученному");
            Assert.True(disjunctionFactB.Equals(disjunctionFacts.Last.Value), "Последний исходный факт не сооветвует последнему полученному");
        }
    }
}