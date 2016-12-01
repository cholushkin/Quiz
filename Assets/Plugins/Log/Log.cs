//#define DISABLE_LOG


using System;

[Serializable]
public class LogChecker
{

    public enum Level
    {
        Disabled,
        Important,
        Normal,
        Verbose
    }

    public LogChecker(Level level)
    {
        CheckerLevel = level;
    }

    private bool IsAtLeast(Level level)
    {
#if DISABLE_LOG
        return false;
#else
        return level <= CheckerLevel;
#endif
    }

    public bool Important()
    {
        return IsAtLeast(Level.Important);
    }

    public bool Normal()
    {
        return IsAtLeast(Level.Normal);
    }

    public bool Verbose()
    {
        return IsAtLeast(Level.Verbose);
    }

    public Level CheckerLevel = Level.Disabled;

}
