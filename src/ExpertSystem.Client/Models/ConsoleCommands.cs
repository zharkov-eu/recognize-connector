using System;
using System.Collections.Generic;
using System.Text;

namespace ExpertSystem.Client.Models
{
    public enum ConsoleCommands
    {
        Exit,
        ForwardProcessing,
        BackProcessing,
        LogicProcessing,
        FuzzyProcessingMamdani,
        FuzzyProcessingSugeno,
        FuzzyNeuralProcessing,

        LoadSockets,
        AddNewSocket,
        UpdateExistingSocket,
        DeleteExistingSocket,

        GetSocketGroups,
        AddSocketGroup,
        RemoveSocketGroup,

        GetSocketsInGroup,
        AddSocketToGroup,
        RemoveSocketFromGroup
    }
}
