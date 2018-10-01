using System;
using System.Linq;
using System.Collections.Generic;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Models
{
    public struct LogicFactLevel
    {
        public int Depth;
        public bool Negation;

        public override bool Equals(object obj) 
        {
            if (!(obj is LogicFactLevel))
                return false;

            LogicFactLevel fact = (LogicFactLevel) obj;
            return Depth == fact.Depth && Negation == fact.Negation;
        }

        public override int GetHashCode()
        {
            int hashCode = 17;
            hashCode += Depth.GetHashCode();
            hashCode += Negation.GetHashCode();
            return hashCode;
        } 
    }

    public class LogicFact : Fact
    {
        /// <summary>
        /// Возвращает конъюктивно нормальную форму
        /// <summary>
        public static LinkedList<LogicFact> ConjuctionNormalFrom(LinkedList<LogicFact> facts, int priority = 100) {
            // Если список пуст или содержит один факт - вернуть его
            if (facts.Count <= 1) return facts;

            // Получить максимальный приоритет операции
            int maxPriority = Math.Min(facts.Select(fact => Priority[fact.RightOperation]).Max(), priority);

            // Получить первый элемент списка
            var node = facts.First;

            // Итерируемся по элементам списка
            while (node != null)
            {
                if (Priority[node.Value.RightOperation] == maxPriority)
                {
                    switch (node.Value.RightOperation)
                    {
                        case Operation.Implication:
                            // Заменяем импликацию дизъюнцией
                            node.Value.RightOperation = Operation.Disjunction;

                            // Создаем уровень ниже
                            var level = new LogicFactLevel { Depth = node.Value.Level.Depth + 1, Negation = true };
                            // Помещаем всю левую часть на уровень ниже
                            var currentNode = node;
                            while (currentNode != null)
                            {
                                currentNode.Value.Level = level;
                                currentNode = currentNode.Previous;
                            }

                            break;

                        case Operation.Conjunction:
                            if (node.Value.Level.Depth != 0 && node.Value.Level.Negation)
                            {
                                node.Value.Negation = !node.Value.Negation;
                                node.Value.RightOperation = Operation.Disjunction;
                                node.Value.Level = new LogicFactLevel { Depth = node.Value.Level.Depth - 1, Negation = false };
                            }
                            break;

                        case Operation.Disjunction:
                            if (node.Value.Level.Depth != 0 && node.Value.Level.Negation)
                            {
                                node.Value.Negation = !node.Value.Negation;
                                node.Value.Level = new LogicFactLevel { Depth = node.Value.Level.Depth - 1, Negation = false };
                            }
                            break;

                        case Operation.None:
                            return facts;
                    }
                }

                node = node.Next;
            }

            return ConjuctionNormalFrom(facts, maxPriority - 1);
        }
        
        public bool Negation { get; set; }
        public Operation RightOperation { get; set; }
        public LogicFactLevel Level { get; set; }

        public LogicFact(string domain, object value, Type type) : base(domain, value, type)
        {    
            Negation = false;
            RightOperation = LogicOperation.Operation.None;
            Level = new LogicFactLevel { Depth = 0, Negation = false };
        }

        public LogicFact(string domain, object value, Type type, Operation operation, bool negation = false)
            : base(domain, value, type)
        {
            Negation = negation;
            RightOperation = operation;
            Level = new LogicFactLevel { Depth = 0, Negation = false };
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var fact = obj as LogicFact;
            if (fact == null)
                return false;
            return base.Equals(obj) &&
                   Negation == fact.Negation &&
                   RightOperation == fact.RightOperation;
        }
        
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            hashCode += 27 * Negation.GetHashCode();
            hashCode += 27 * RightOperation.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            string output = Negation ? "~" : "";
            return output + $"({Domain}: {Value}) {GetOutput(RightOperation)}";
        }
    }
}