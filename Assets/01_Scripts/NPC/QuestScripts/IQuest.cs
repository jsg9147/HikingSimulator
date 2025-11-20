using System.Collections.Generic;

public interface IQuest
{
    string QuestName { get; }
    string Description { get; }
    bool IsCompleted { get; }
    int CurrentCount { get; }
    int TargetCount { get; }
    List<string> Dialogue { get; }

    List<string> CompliteDialogue { get; }

    bool CheckCompletion();
}
