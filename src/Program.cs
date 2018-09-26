using System;
using ExpertSystem.Models;
using ExpertSystem.Processor;

namespace ExpertSystem
{
    public class Program
    {
        protected readonly ProductionProcessor _productionProcessor;
        protected readonly LogicProcessor _logicProcessor;
        protected readonly FuzzyProcessor _fuzzyProcessor;

        public struct ProgramOptions
        { 
            public bool Debug;
        }

        protected Program(ProgramOptions options)
        {
            var socketFieldsProcessor = new SocketFieldsProcessor();
            var sockets = socketFieldsProcessor.GetSockets();
            var fieldValues = socketFieldsProcessor.GetFieldsWithPossibleValues(sockets);

            var rulesGenerator = new RulesGenerator();
            var rulesGraph = rulesGenerator.GenerateRules(sockets, fieldValues);

            _productionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions{ Debug = options.Debug });
            _logicProcessor = new LogicProcessor(rulesGraph, new ProcessorOptions{ Debug = options.Debug });
            _fuzzyProcessor = new FuzzyProcessor(rulesGraph, new ProcessorOptions{ Debug = options.Debug });
        }

        private static void Main(string[] args)
        {
            var program = new ConsoleProgram(new ProgramOptions{ Debug = true });
            program.Run();
        }
    }
}