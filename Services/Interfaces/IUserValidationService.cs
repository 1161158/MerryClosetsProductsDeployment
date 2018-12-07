using System.Threading.Tasks;

namespace MerryClosets.Services.Interfaces
{
    public interface IUserValidationService
    {
        Task<bool> validateContentManager(string tokenString);
        Task<string> GetUserRef(string tokenString);
    }
}