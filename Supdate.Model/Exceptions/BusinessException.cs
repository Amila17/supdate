using System;
using System.Runtime.Serialization;

namespace Supdate.Model.Exceptions
{
  [Serializable]
  public class BusinessException : Exception
  {
    public BusinessException()
    {
    }

    public BusinessException(string message)
      : base(message)
    {
    }

    public BusinessException(string message, Exception inner)
      : base(message, inner)
    {
    }

    // This constructor is needed for serialization.
    protected BusinessException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
