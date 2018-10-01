namespace ExpertSystem.Models.Graph
{
    public class RulesGraph
    {
        public static readonly string ROOT_DOMAIN = ".";
        public GraphNode Root { get; set; }

        public RulesGraph()
        {
            Root = new GraphNode(new Fact(
                ROOT_DOMAIN, CustomSocket.DefaultValue[typeof(string)], typeof(string)
            ));
        }
    }
}