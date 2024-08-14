namespace PeerAMid.Utility;

public static class NotParallel
{
    public static void Invoke(params Action[] actions)
    {
        Exception? exception = null;

        foreach (var action in actions)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                exception = e;
            }
        }

        if (exception != null)
            throw exception;
    }
}
