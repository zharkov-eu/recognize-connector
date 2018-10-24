using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.CommonLogic
{
    public struct LogicFactLevel
    {
        public int Depth;
        public bool Negation;

        public override bool Equals(object obj)
        {
            if (!(obj is LogicFactLevel))
                return false;

            var fact = (LogicFactLevel) obj;
            return Depth == fact.Depth && Negation == fact.Negation;
        }

        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode += Depth.GetHashCode();
            hashCode += Negation.GetHashCode();
            return hashCode;
        }
    }

    public class LogicFact : Fact
    {
        public LogicFact(CustomSocketDomain.SocketDomain domain, object value) : base(domain, value)
        {
            Negation = false;
            RightOperation = LogicOperation.Operation.None;
            Level = new LogicFactLevel {Depth = 0, Negation = false};
        }

        public LogicFact(CustomSocketDomain.SocketDomain domain, object value, LogicOperation.Operation operation,
            bool negation = false)
            : base(domain, value)
        {
            Negation = negation;
            RightOperation = operation;
            Level = new LogicFactLevel {Depth = 0, Negation = false};
        }

        public bool Negation { get; set; }
        public LogicOperation.Operation RightOperation { get; set; }
        public LogicFactLevel Level { get; set; }

        /// <summary>
        ///     Возвращает конъюктивную нормальную форму
        /// </summary>
        public static LinkedList<LogicFact> ConjunctionNormalFrom(LinkedList<LogicFact> facts, int priority = 100)
        {
            // Если список пуст или содержит один факт - вернуть его
            if (facts.Count <= 1)
                return facts;

            // Получить максимальный приоритет операции
            var maxPriority = Math.Min(
                facts.Select(fact => LogicOperation.Priority[fact.RightOperation]).Max(), priority
            );

            // Получить первый элемент списка
            var node = facts.First;

            // Итерируемся по элементам списка
            while (node != null)
            {
                if (LogicOperation.Priority[node.Value.RightOperation] == maxPriority)
                    switch (node.Value.RightOperation)
                    {
                        case LogicOperation.Operation.Implication:
                            // Заменяем импликацию дизъюнцией
                            node.Value.RightOperation = LogicOperation.Operation.Disjunction;

                            // Создаем уровень ниже
                            var level = new LogicFactLevel {Depth = node.Value.Level.Depth + 1, Negation = true};
                            // Помещаем всю левую часть на уровень ниже
                            var currentNode = node;
                            while (currentNode != null)
                            {
                                currentNode.Value.Level = level;
                                currentNode = currentNode.Previous;
                            }

                            break;

                        case LogicOperation.Operation.Conjunction:
                            if (node.Value.Level.Depth != 0 && node.Value.Level.Negation)
                            {
                                node.Value.Negation = !node.Value.Negation;
                                node.Value.RightOperation = LogicOperation.Operation.Disjunction;
                                node.Value.Level =
                                    new LogicFactLevel {Depth = node.Value.Level.Depth - 1, Negation = false};
                            }

                            break;

                        case LogicOperation.Operation.Disjunction:
                            if (node.Value.Level.Depth != 0 && node.Value.Level.Negation)
                            {
                                node.Value.Negation = !node.Value.Negation;
                                node.Value.Level =
                                    new LogicFactLevel {Depth = node.Value.Level.Depth - 1, Negation = false};
                            }

                            break;

                        case LogicOperation.Operation.None:
                            return facts;
                    }

                node = node.Next;
            }

            return ConjunctionNormalFrom(facts, maxPriority - 1);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LogicFact fact))
                return false;
            return base.Equals(obj) &&
                   Negation == fact.Negation &&
                   RightOperation == fact.RightOperation;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode += 27 * Negation.GetHashCode();
            hashCode += 27 * RightOperation.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            var output = Negation ? "~" : "";
            output += $"({Domain}: {Value})";
            return string.IsNullOrEmpty(LogicOperation.GetOutput(RightOperation))
                ? output
                : output + $" {LogicOperation.GetOutput(RightOperation)}";
        }
    }
}