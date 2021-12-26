using System;

namespace Domain.Core.Exceptions.NotExitExceptions;

/// <summary>
/// Classes of exceptions which are child of this one
/// marks as "NotExit", that means they aren't critical
/// and the Doocutor will not close after throwing them.
/// </summary>
public abstract class NotExitException : Exception
{
    protected NotExitException(string message) : base(message) {}
}