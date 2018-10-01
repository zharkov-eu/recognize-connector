using System;
using System.Reflection;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class LogicOperation
    {
        public static string GetOutput(Operation operation)
        {

            Type type = operation.GetType();
            MemberInfo[] memInfo = type.GetMember(operation.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(Output), false);
                if (attrs != null && attrs.Length > 0)
                    return ((Output)attrs[0]).Text;
            }
            return operation.ToString();
        }

        public enum Operation {
            [Output("")]
            None,
            [Output("->")]
            Implication,
            [Output("^")]
            Conjunction,
            [Output("v")]
            Disjunction
        }

        class Output : Attribute
        {
            public string Text;
            public Output(string text)
            {
                Text = text;
            }
        }

        public static Dictionary<Operation, int> Priority = new Dictionary<Operation, int>()
        {
            { Operation.None, 0 },
            { Operation.Implication, 3},
            { Operation.Conjunction, 2},
            { Operation.Disjunction, 1},
        };
    }
}