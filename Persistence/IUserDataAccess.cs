using System.Collections.Generic;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// CRUD for UserModel (login, profile, roles).
    /// </summary>
    public interface IUserDataAccess
    {
        IEnumerable<UserModel> GetAll();
        IEnumerable<UserModel> GetByRole(string role);
        UserModel? GetById(int id);
        UserModel? GetByEmail(string email);
        bool EmailExists(string email);
        UserModel Create(UserModel user);
        void Update(UserModel user);
        bool Delete(int id);
    }
}
