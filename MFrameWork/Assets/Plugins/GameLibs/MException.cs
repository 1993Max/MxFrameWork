using System;

namespace MoonCommonLib
{
    [Serializable]
    public class MException : ApplicationException
    {
        public MException()
        { }

        public MException(string message)
            : base(message)
        { }

        public MException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MDoubleNewException : MException
    {
        public MDoubleNewException(string message)
            : base(message)
        { }

        public MDoubleNewException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MErrorEventArgTypeException : MException
    {
        public MErrorEventArgTypeException(string message)
            : base(message)
        { }

        public MErrorEventArgTypeException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MErrorUnregisteredComponentException : MException
    {
        public MErrorUnregisteredComponentException(string message)
            : base(message)
        { }

        public MErrorUnregisteredComponentException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MMissEventDescriptionException : MException
    {
        public MMissEventDescriptionException(string message)
            : base(message)
        { }

        public MMissEventDescriptionException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MZeroDelayException : MException
    {
        public MZeroDelayException(string message)
            : base(message)
        { }

        public MZeroDelayException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MInvalidSkillException : MException
    {
        public MInvalidSkillException(string message)
            : base(message)
        { }

        public MInvalidSkillException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MSkillUnexpectedFireException : MException
    {
        public MSkillUnexpectedFireException(string message)
            : base(message)
        { }

        public MSkillUnexpectedFireException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MMissAnimationException : MException
    {
        public MMissAnimationException(string message)
            : base(message)
        { }

        public MMissAnimationException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MZeroCoolDownTimeException : MException
    {
        public MZeroCoolDownTimeException(string message)
            : base(message)
        { }

        public MZeroCoolDownTimeException(string message, Exception inner)
            : base(message, inner)
        { }
    }

    [Serializable]
    public class MDataFileLoadException : MException
    {
        public MDataFileLoadException(string message)
            : base(message)
        { }

        public MDataFileLoadException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}