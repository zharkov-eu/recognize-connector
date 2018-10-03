using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models
{
    public class FuzzyFact : Fact
    {
        new public FuzzyDomain Domain;
        public object value;
        
        public FuzzyFact(FuzzyDomain domain, object value) : base(domain.Domain, value)
        {
            Domain = domain;
            Value = value;
        }
    }
}