using Rf7_CustomAttributes.ValidationAttributes;

namespace Rf7_CustomAttributes
{
  public class User
  {
    [MyNumber(ErrorMessage = "Id must be a number!")]
    public int Id { get; set; }

    [MyRequired(ErrorMessage = "User name cannot be empty!")]
    public string Name { get; set; }

    [MyRequired]
    [MyPhone(ErrorMessage = "Invalid phone number!")]
    public string Phone { get; set; }

    [MyRequired]
    [MyEmail]
    public string Email { get; set; }
  }
}
