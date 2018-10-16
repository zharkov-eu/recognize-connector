using System;
using System.Collections.Generic;

namespace ExpertSystem.Models.CommonLogic
{
    public class LogicOperation
    {
        public enum Operation
        {
            [Output("")] None,
            [Output("->")] Implication,
            [Output("^")] Conjunction,
            [Output("v")] Disjunction
        }

        public static Dictionary<Operation, int> Priority = new Dictionary<Operation, int>
        {
            {Operation.None, 0},
            {Operation.Implication, 3},
            {Operation.Conjunction, 2},
            {Operation.Disjunction, 1}
        };

        public static string GetOutput(Operation operation)
        {
            var type = operation.GetType();
            var memInfo = type.GetMember(operation.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(Output), false);
                if (attrs != null && attrs.Length > 0)
                    return ((Output) attrs[0]).Text;
            }

            return operation.ToString();
        }

        private class Output : Attribute
        {
            public readonly string Text;

            public Output(string text)
            {
                Text = text;
            }
        }
    }
}