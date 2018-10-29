using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Aggregator.Models.CommonLogic
{
    public class LogicRule : IEnumerable<LogicFact>
    {
        /// <summary>Возвращает конъюктивную нормальную форму</summary>
        public static LogicRule ConjunctionNormalFrom(LogicRule rule, int priority = 100)
        {
            // Если список пуст или содержит один факт - вернуть его
            if (rule.Count <= 1)
                return rule;

            // Получить максимальный приоритет операции
            var maxPriority = Math.Min(
                rule.Select(fact => LogicOperation.Priority[fact.RightOperation]).Max(), priority
            );

            // Получить первый элемент списка
            var node = rule.First;

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
                            return rule;
                    }

                node = node.Next;
            }

            return ConjunctionNormalFrom(rule, maxPriority - 1);
        }

        public int Count => _facts?.Count ?? 0;
        public LinkedListNode<LogicFact> First => _facts?.First;
        public LinkedListNode<LogicFact> Last => _facts?.Last;

        private readonly LinkedList<LogicFact> _facts;

        public LogicRule()
        {
            _facts = new LinkedList<LogicFact>();
        }

        public LogicRule(IEnumerable<LogicFact> facts)
        {
            _facts = new LinkedList<LogicFact>(facts);
        }

        public void AddLast(LogicFact fact) => _facts.AddLast(fact);

        public IEnumerator<LogicFact> GetEnumerator() => _facts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class LogicRules : IEnumerable<LogicRule>
    {
        private readonly List<LogicRule> _rules;

        public LogicRules()
        {
            _rules = new List<LogicRule>();
        }

        public void AddSocket(CustomSocket socket)
        {
            var logicRule = new LogicRule();
            foreach (var domain in GetSocketDomains())
            {
                var operation = domain.Equals(SocketDomain.SocketName)
                    ? LogicOperation.Operation.Implication
                    : LogicOperation.Operation.Conjunction;
                var fact = new LogicFact(domain,
                    CustomSocketExtension.SocketType.GetProperty(domain.ToString()).GetValue(socket),
                    operation);
                if (!fact.IsDefaultValue())
                    logicRule.AddLast(fact);
            }

            logicRule.AddLast(new LogicFact(SocketDomain.SocketName, socket.SocketName,
                LogicOperation.Operation.None));

            _rules.Add(logicRule);
        }

        public void RemoveSocket(CustomSocket socket)
        {
            var rule = _rules.Where(p =>
            {
                var socketNameFact = p.Last.Value;
                return socket.SocketName.Equals(socketNameFact.Value);
            }).FirstOrDefault();
            _rules.Remove(rule);
        }

        public IEnumerator<LogicRule> GetEnumerator() => _rules.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}