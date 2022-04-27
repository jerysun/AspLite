using AspLite.Services;

namespace AspLite.Controllers
{
  /// <summary>
  /// A controller class that automatically instantiatiate some members with DI via constructor
  /// </summary>
  public class AspController
  {
    private readonly IThirdParty _thirdParty;

    public AspController(IThirdParty thirdParty)
    {
      _thirdParty = thirdParty;
    }

    /// <summary>
    /// An Action method with two parameters reutrns a string
    /// </summary>
    /// <returns></returns>
    public string AspAction(string a, string b)
    {
      // Validate the parameters from the Http Request
      if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return "Bad Request";

      // Call the service to process this request
      var result = _thirdParty.Add(a, b);
      return result;
    }
  }
}
