using System;
using ExpertSystem.Models;

namespace ExpertSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var socketFieldsProcessor = new SocketFieldsProcessor();
            socketFieldsProcessor.GetFieldsWithPossibleValues();
            Console.ReadLine();
        }
    }
}