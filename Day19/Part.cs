namespace Day19;

record Part(Range X, Range M, Range A, Range S, string CurrentWorkflow)
{
    public long Count => X.Count * M.Count * A.Count * S.Count;
}