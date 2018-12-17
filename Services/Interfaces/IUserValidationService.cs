using System.Threading.Tasks;

namespace MerryClosets.Services.Interfaces
{
    public interface IUserValidationService
    {
        Task<bool> ValidateContentManager(string tokenString);
        Task<bool> Validate(string tokenString);
        Task<string> GetUserRef(string tokenString);
        bool CheckAuthorizationToken(string authorization);
    }
}