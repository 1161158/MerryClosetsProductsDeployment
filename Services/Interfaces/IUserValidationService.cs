using System.Threading.Tasks;

namespace MerryClosets.Services.Interfaces
{
    public interface IUserValidationService
    {
        Task<bool> validateContentManager(string tokenString);
    }
}