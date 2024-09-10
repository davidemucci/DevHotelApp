namespace DevHotelAPI.Services.Contracts
{
    public interface IAccountRepository
    {
        public string? GenerateToken(string userName);
    }
}
