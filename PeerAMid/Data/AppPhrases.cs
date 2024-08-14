using System.Collections;

#nullable enable

namespace PeerAMid.Data;

// The phrases for a given app and language, including all subjects, topics and options
public class AppPhrases : IEnumerable<SubjectClass>
{
    public readonly string App;
    public readonly string Language;
    private readonly Dictionary<string, SubjectClass> subjects;
    public long Version; // from UTC time

    public AppPhrases(string app, string language)
    {
        App = app;
        Language = language;
        subjects = new Dictionary<string, SubjectClass>();
    }

    public SubjectClass this[string subject]
    {
        get
        {
            if (!subjects.TryGetValue(subject, out var s))
                subjects.Add(subject, s = new SubjectClass(this, subject));
            return s;
        }
    }

    public TopicClass this[string subject, string topic] => this[subject][topic];

    public IEnumerator<SubjectClass> GetEnumerator()
    {
        foreach (var s in subjects.Values)
            yield return s;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public Phrase AddPhrase(string subject, string subjectAndtopic, int id, string condition, string text)
    {
        var s = this[subject];
        return s.AddPhrase(subjectAndtopic, id, condition, text);
    }
}

public class SubjectClass : IEnumerable<TopicClass>
{
    public readonly AppPhrases Parent;
    public readonly string Subject;
    private readonly Dictionary<string, TopicClass> topics;

    public SubjectClass(AppPhrases parent, string subject)
    {
        Parent = parent;
        Subject = subject;
        topics = new Dictionary<string, TopicClass>();
    }

    public TopicClass
        this[string subjectAndTopic] // given the value for 'topic', gives all the text for all the options
    {
        get
        {
            if (!topics.TryGetValue(subjectAndTopic, out var t))
                topics.Add(subjectAndTopic, t = new TopicClass(this, subjectAndTopic));
            return t;
        }
    }

    public IEnumerator<TopicClass> GetEnumerator()
    {
        foreach (var t in topics.Values)
            yield return t;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public Phrase AddPhrase(string subjectAndtopic, int id, string condition, string text)
    {
        var t = this[subjectAndtopic];
        return t.Add(id, condition, text);
    }
}

public class TopicClass : IList<Phrase>
{
    public readonly SubjectClass Parent;
    private readonly List<Phrase> phrases = new();
    public readonly string SubjectAndTopic;

    public TopicClass(SubjectClass parent, string subjectAndTopic)
    {
        Parent = parent;
        SubjectAndTopic = subjectAndTopic;
    }

    public string Subject => Parent.Subject;

    public Phrase Add(int id, string condition, string text)
    {
        var phrase = new Phrase(this, id, condition, text);
        Add(phrase);
        return phrase;
    }

    #region IList

    public Phrase this[int index]
    {
        get => ((IList<Phrase>)phrases)[index];
        set => ((IList<Phrase>)phrases)[index] = value;
    }

    public int Count => ((ICollection<Phrase>)phrases).Count;

    public bool IsReadOnly => ((ICollection<Phrase>)phrases).IsReadOnly;

    public void Add(Phrase item)
    {
        ((ICollection<Phrase>)phrases).Add(item);
    }

    public void Clear()
    {
        ((ICollection<Phrase>)phrases).Clear();
    }

    public bool Contains(Phrase item)
    {
        return ((ICollection<Phrase>)phrases).Contains(item);
    }

    public void CopyTo(Phrase[] array, int arrayIndex)
    {
        ((ICollection<Phrase>)phrases).CopyTo(array, arrayIndex);
    }

    public IEnumerator<Phrase> GetEnumerator()
    {
        return ((IEnumerable<Phrase>)phrases).GetEnumerator();
    }

    public int IndexOf(Phrase item)
    {
        return ((IList<Phrase>)phrases).IndexOf(item);
    }

    public void Insert(int index, Phrase item)
    {
        ((IList<Phrase>)phrases).Insert(index, item);
    }

    public bool Remove(Phrase item)
    {
        return ((ICollection<Phrase>)phrases).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<Phrase>)phrases).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)phrases).GetEnumerator();
    }

    #endregion IList
}

public class Phrase
{
    public readonly string Condition;
    public readonly int LogicId;
    public readonly TopicClass Parent;
    public readonly string Text;
    public object? Tag;

    public Phrase(TopicClass parent, int id, string condition, string text)
    {
        Parent = parent;
        LogicId = id;
        Condition = condition;
        Text = text;
    }

    public string SubjectAndTopic => Parent.SubjectAndTopic;
    public string Subject => Parent.Subject;
}
