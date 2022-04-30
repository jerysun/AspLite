using System;

namespace Rf7_CustomAttributes.ValidationAttributes
{
  public abstract class ValidationAttributeBase : Attribute
  {
    private string Message;
    /// <summary>
    /// Used when the validation fails
    /// </summary>
    public string ErrorMessage
    {
      get { return string.IsNullOrEmpty(Message) ? "Error!" : Message; }
      set { Message = value; }
    }

    public virtual bool IsValid(object value) => value != null;
  }
}
