namespace PeerAMid.Support;

/*
public class PageChange
{
    public readonly string After;
    public readonly string Before;
    public readonly string Event;

    public PageChange(string @event, string before, string after)
    {
        Before = before;
        Event = @event;
        After = after;
    }
}

public class PageHistory
{
    private readonly List<PageChange> _stack = new();
    public readonly string Name;

    public PageHistory(string name)
    {
        Name = name;
        _stack = new List<PageChange>();
    }

    public int Index { get; private set; } = -1;
    public int Count => _stack.Count;

    public PageChange? Current => Index < 0 ? null : _stack[Index];

    public PageChange Save(string @event, string before, string after)
    {
        var pc = new PageChange(@event, before, after);
        ++Index;
        if (Index >= _stack.Count)
        {
            _stack.Add(pc);
        }
        else
        {
            _stack[Index] = pc;
            var excess = _stack.Count - Index - 1;
            if (excess > 0)
                _stack.RemoveRange(Index + 1, excess);
        }

        return pc;
    }

    public void Clear()
    {
        _stack.Clear();
        Index = -1;
    }

    public bool Revert()
    {
        if (Index < 0)
            return false;
        --Index;
        return true;
    }

    public bool Advance()
    {
        if (Index == _stack.Count - 1)
            return false;
        ++Index;
        return true;
    }

    public override string ToString()
    {
        return ToString("");
    }

    public string ToString(string linePrefix)
    {
        var b = new StringBuilder();
        linePrefix = linePrefix ?? "";
        b.Append(linePrefix).Append("PageStateStack ").Append(Name).Append(" ").Append(Index).Append("/").Append(Count);
        for (var i = 0; i < Count; ++i)
        {
            b.Append(linePrefix).Append(linePrefix).Append("[").Append(i).Append("] ").AppendLine(_stack[i].Event);
            b.Append(linePrefix).Append(linePrefix).Append("   before ").AppendLine(_stack[i].Before);
            b.Append(linePrefix).Append(linePrefix).Append("   after  ").AppendLine(_stack[i].After);
        }

        return b.ToString();
    }
}

public class PageStateStacks
{
    private readonly Dictionary<string, PageHistory> _stacks = new();

    public PageHistory this[string name]
    {
        get
        {
            name = name ?? "";
            if (!_stacks.TryGetValue(name, out var pss))
                _stacks.Add(name, pss = new PageHistory(name));
            return pss;
        }
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        b.AppendLine("PageStateStacks:");
        foreach (var psp in _stacks.Values)
            b.Append(psp.ToString("    "));
        return b.ToString();
    }
}

public class PageStateReturnValue
{
    public PageStateReturnValue(PageHistory ph, bool success = true)
    {
        PageName = ph.Name;
        Index = ph.Index;
        Count = ph.Count;
        var pc = ph.Current;
        Event = pc?.Event;
        Before = pc?.Before;
        After = pc?.After;
        Error = !success;
        // Log.Debug("PageStateReturnValue " + $"PageName {PageName} Index {Index} Count {Count} Error {Error} PC {pc}");
    }

    public string? PageName { get; set; }
    public int Index { get; set; }
    public int Count { get; set; }
    public string? Event { get; set; }
    public string? Before { get; set; }
    public string? After { get; set; }
    public bool Error { get; set; }

    public override string ToString()
    {
        return $"PageName {PageName} Index {Index} Count {Count} Event {Event} Error {Error} Before {Before ?? "null"} After {After ?? "null"}";
    }
}
*/
