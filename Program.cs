using System;
using ExpertSystem.Parse;

namespace ExpertSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //const string fileName = @"C:\Temp\file.csv";
            var parse = new Sort();
            parse.Parse();
            Console.ReadLine();
        }
    }
}