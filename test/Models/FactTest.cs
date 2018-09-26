using Xunit;
using ExpertSystem.Models;

namespace ExpertSystem.Models
{
    public class Fact_IsCorrect
    {
        private readonly Fact _fact;
        public Fact_IsCorrect()
        {
            _fact = new Fact("domain", "value");
        }

        [Fact]
        public void Fact_IsEqual()
        {
            var factEqual = new Fact("domain", "value");
            var factNotEqual = new Fact("domain", "notequal");

            Assert.True(_fact.Equals(_fact), "Факт должен быть равен самому себе");
            Assert.True(_fact.Equals(factEqual), "Факт должен быть равен аналогичному по содержанию факту");
            Assert.False(_fact.Equals(null), "Факт не должен быть равен null");
            Assert.False(_fact.Equals(factNotEqual), "Факт не должен быть равен различному по содержанию факту");
        }

        [Fact]
        public void Fact_IsEqualityEqual()
        {
            var factEqual = new Fact("domain", "value");
            var factNotEqual = new Fact("domain", "notequal");

            Assert.True(_fact.Equals(_fact, _fact), "Факт должен быть равен самому себе");
            Assert.True(_fact.Equals(_fact, factEqual), "Факт должен быть равен аналогичному по содержанию факту");
            Assert.False(_fact.Equals(_fact, null), "Факт не должен быть равен null");
            Assert.False(_fact.Equals(_fact, factNotEqual), "Факт не должен быть равен различному по содержанию факту");
        }
    }
}