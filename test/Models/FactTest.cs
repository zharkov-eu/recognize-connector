using Xunit;

namespace ExpertSystem.Models
{
    public class Fact_IsCorrect
    {
        private readonly Fact _fact;

        public Fact_IsCorrect()
        {
            _fact = new Fact("domain", "value", typeof(string));
        }

        [Fact]
        public void Fact_IsDefaultValue()
        {
            var intDefault = new Fact("domain", -1, typeof(int));
            var floatDefault = new Fact("domain", -1.0f, typeof(float));
            var stringDefault = new Fact("domain", "", typeof(string));

            Assert.True(intDefault.IsDefaultValue(), "Факт типа int должен являться фактом по умолчанию");
            Assert.True(floatDefault.IsDefaultValue(), "Факт типа float должен являться фактом по умолчанию");
            Assert.True(stringDefault.IsDefaultValue(), "Факт типа string должен являться фактом по умолчанию");
        }

        [Fact]
        public void Fact_IsEqual()
        {
            var factEqual = new Fact("domain", "value", typeof(string));
            var factNotEqual = new Fact("domain", "notequal", typeof(string));

            Assert.True(_fact.Equals(_fact), "Факт должен быть равен самому себе");
            Assert.True(_fact.Equals(factEqual), "Факт должен быть равен аналогичному по содержанию факту");
            Assert.False(_fact.Equals(null), "Факт не должен быть равен null");
            Assert.False(_fact.Equals(factNotEqual), "Факт не должен быть равен различному по содержанию факту");
        }

        [Fact]
        public void Fact_IsEqualityEqual()
        {
            var factEqual = new Fact("domain", "value", typeof(string));
            var factNotEqual = new Fact("domain", "notequal", typeof(string));

            Assert.True(_fact.Equals(_fact, _fact), "Факт должен быть равен самому себе");
            Assert.True(_fact.Equals(_fact, factEqual), "Факт должен быть равен аналогичному по содержанию факту");
            Assert.False(_fact.Equals(_fact, null), "Факт не должен быть равен null");
            Assert.False(_fact.Equals(_fact, factNotEqual), "Факт не должен быть равен различному по содержанию факту");
        }

        public class FactSet_IsCorrect
        {
            private readonly FactSet _factSet;
            public FactSet_IsCorrect()
            {
                _factSet = new FactSet(
                    new Fact("domain_a", "value_a", typeof(string)),
                    new Fact("domain_b", "value_b", typeof(string))
                );
            }

            [Fact]
            public void FactSet_IsEqual()
            {
                var factSetEqual = new FactSet(
                    new Fact("domain_a", "value_a", typeof(string)),
                    new Fact("domain_b", "value_b", typeof(string))
                );
                var factSetNotEqual = new FactSet(
                    new Fact("domain_a", "value_a", typeof(string)),
                    new Fact("domain_b", "value_notequal", typeof(string))
                );

                Assert.True(_factSet.Equals(_factSet), "Множество фактов должно быть равено самому себе");
                Assert.True(_factSet.Equals(factSetEqual), "Множество фактов должно быть равно аналогичному по содержанию множеству фактов");
                Assert.False(_factSet.Equals(null), "Множество фактов не должно быть равен null");
                Assert.False(_factSet.Equals(factSetNotEqual), "Множество фактов не должно быть равно различному по содержанию множеству фактов");
            }
        }
    }
}