namespace ExchangeService.BusinessLogic.Models.Authentication;
internal class AuthenticationOptions
{
    static readonly int lifeTime = 5;
    public string Source { get; set; }
    public int UserId { get; set; }
}
