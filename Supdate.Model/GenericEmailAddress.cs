namespace Supdate.Model
{
  public class GenericEmailAddress
  {
    public GenericEmailAddress(string emailAddress, string name)
    {
      Address = emailAddress;
      Name = name;
    }

    public string Name { get; set; }
    public string Address { get; set; }
  }
}
