using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly IDatabaseService _db;

        public UserRepo(IDatabaseService db)
        {
            _db = db;
        }

        public User? GetUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "(SELECT ts.SchoolId FROM SchoolUser ts WHERE ts.UserId = u.UserId LIMIT 1) AS SchoolId, " +
                "(SELECT sg.GroupId FROM StudentGroup sg WHERE sg.UserId = u.UserId LIMIT 1) AS GroupId " +
                "FROM User u " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "WHERE u.Username = @username " +
                "LIMIT 1;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@username"] = username
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            if (dt.Rows.Count == 0) return null;

            User user = MapUser(dt.Rows[0]);
            return user;
        }

        public User? GetMail(string mail)
        {
            if (string.IsNullOrWhiteSpace(mail)) return null;

            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "(SELECT ts.SchoolId FROM SchoolUser ts WHERE ts.UserId = u.UserId LIMIT 1) AS SchoolId, " +
                "(SELECT sg.GroupId FROM StudentGroup sg WHERE sg.UserId = u.UserId LIMIT 1) AS GroupId " +
                "FROM User u " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "WHERE u.Email = @mail " +
                "LIMIT 1;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@mail"] = mail
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            if (dt.Rows.Count == 0) return null;

            User user = MapUser(dt.Rows[0]);
            return user;
        }

        public User? GetUserById(int id)
        {
            if (id <= 0) return null;

            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "(SELECT ts.SchoolId FROM SchoolUser ts WHERE ts.UserId = u.UserId LIMIT 1) AS SchoolId, " +
                "(SELECT sg.GroupId FROM StudentGroup sg WHERE sg.UserId = u.UserId LIMIT 1) AS GroupId " +
                "FROM User u " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "WHERE u.UserId = @id " +
                "LIMIT 1;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@id"] = id
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            if (dt.Rows.Count == 0) return null;

            User user = MapUser(dt.Rows[0]);
            return user;
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "(SELECT ts.SchoolId FROM SchoolUser ts WHERE ts.UserId = u.UserId LIMIT 1) AS SchoolId, " +
                "(SELECT sg.GroupId FROM StudentGroup sg WHERE sg.UserId = u.UserId LIMIT 1) AS GroupId " +
                "FROM User u " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "ORDER BY u.UserId ASC;";

            DataTable dt = _db.ReadQuery(sql);

            _db.Close();

            IEnumerable<User> users = dt.AsEnumerable().Select(MapUser);
            return Task.FromResult(users);
        }

        public Task<IEnumerable<User>> GetTeachersBySchoolIdAsync(int schoolId)
        {
            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "@schoolId AS SchoolId, " +
                "NULL AS GroupId " +
                "FROM SchoolUser ts " +
                "INNER JOIN User u ON u.UserId = ts.UserId " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "WHERE ts.SchoolId = @schoolId AND r.Role = 'Docent' " +
                "ORDER BY u.UserId ASC;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@schoolId"] = schoolId
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            IEnumerable<User> users = dt.AsEnumerable().Select(MapUser);
            return Task.FromResult(users);
        }

        public Task<IEnumerable<User>> GetStudentsByGroupIdAsync(int groupId)
        {
            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "NULL AS SchoolId, " +
                "@groupId AS GroupId " +
                "FROM StudentGroup sg " +
                "INNER JOIN User u ON u.UserId = sg.UserId " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "WHERE sg.GroupId = @groupId AND r.Role = 'Leerling' " +
                "ORDER BY u.UserId ASC;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@groupId"] = groupId
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            IEnumerable<User> users = dt.AsEnumerable().Select(MapUser);
            return Task.FromResult(users);
        }
        
        public Task<IEnumerable<User>> GetStudentsBySchoolIdAsync(int schoolId)
        {
            _db.Open();

            string sql =
                "SELECT " +
                "u.UserId, u.Username, u.Email, u.Password, u.RoleId, u.StatusId, " +
                "r.Role AS RoleName, s.Status AS StatusName, " +
                "su.SchoolId AS SchoolId, " +
                "sg.GroupId AS GroupId " +
                "FROM User u " +
                "INNER JOIN SchoolUser su ON su.UserId = u.UserId " +
                "LEFT JOIN StudentGroup sg ON sg.UserId = u.UserId " +
                "LEFT JOIN Role r ON r.RoleId = u.RoleId " +
                "LEFT JOIN Status s ON s.StatusId = u.StatusId " +
                "WHERE su.SchoolId = @schoolId AND r.Role = 'Leerling' " +
                "ORDER BY u.UserId ASC;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@schoolId"] = schoolId
            };

            DataTable dt = _db.ReadQuery(sql, parameters);
            _db.Close();

            IEnumerable<User> users = dt.AsEnumerable().Select(MapUser);
            return Task.FromResult(users);
        }

        public Task<User?> GetByIdAsync(int userId)
        {
            User? user = GetUserById(userId);
            return Task.FromResult(user);
        }

        public Task SetActiveAsync(int userId, bool isActive)
        {
            _db.Open();

            int statusId = ResolveStatusId(isActive);

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["StatusId"] = statusId
            };

            int status = _db.Update("User", "UserId", userId.ToString(), data);

            _db.Close();

            if (status != 202 && status != 404)
            {
                throw new Exception("Failed to set active status in database.");
            }

            return Task.CompletedTask;
        }

        public Task CreateAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Gebruikersnaam is verplicht.");
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Wachtwoord is verplicht.");

            _db.Open();

            if (UsernameExists(user.Username))
            {
                _db.Close();
                throw new Exception("Deze gebruikersnaam is al in gebruik.");
            }

            int roleId = ResolveRoleId(user.UserType);
            int statusId = ResolveStatusId(user.IsActive);

            object emailValue =
                string.IsNullOrWhiteSpace(user.Email)
                    ? DBNull.Value
                    : user.Email.Trim();

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["Username"] = user.Username.Trim(),
                ["Email"] = emailValue,
                ["Password"] = user.Password,
                ["RoleId"] = roleId,
                ["StatusId"] = statusId
            };

            int newId = _db.CreateAndReturnId("User", data);
            if (newId <= 0)
            {
                _db.Close();
                throw new Exception("Kon de gebruiker niet aanmaken in de database.");
            }

            user.Id = newId;

            UpsertRelations(user);

            _db.Close();
            return Task.CompletedTask;
        }



        public Task UpdateAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.Id <= 0) throw new ArgumentException("Invalid user id.", nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required.", nameof(user));

            _db.Open();

            int roleId = ResolveRoleId(user.UserType);
            int statusId = ResolveStatusId(user.IsActive);

            object emailValue =
                string.IsNullOrWhiteSpace(user.Email)
                    ? DBNull.Value
                    : user.Email.Trim();

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["Username"] = user.Username.Trim(),
                ["Email"] = emailValue,
                ["Password"] = user.Password,
                ["RoleId"] = roleId,
                ["StatusId"] = statusId
            };

            int status = _db.Update("User", "UserId", user.Id.ToString(), data);
            if (status != 202 && status != 404)
            {
                _db.Close();
                throw new Exception("Failed to update user in database.");
            }

            UpsertRelations(user);

            _db.Close();
            return Task.CompletedTask;
        }


        public Task DeleteAsync(int userId)
        {
            _db.Open();

            string clearTeacherSql = "UPDATE SchoolGroup SET TeacherId = NULL WHERE TeacherId = @userId;";
            Dictionary<string, object> clearTeacherParams = new Dictionary<string, object>
            {
                ["@userId"] = userId
            };
            _db.ExecuteNonQuery(clearTeacherSql, clearTeacherParams);

            string delSchoolUser = "DELETE FROM SchoolUser WHERE UserId = @userId;";
            Dictionary<string, object> delSchoolUserParams = new Dictionary<string, object>
            {
                ["@userId"] = userId
            };
            _db.ExecuteNonQuery(delSchoolUser, delSchoolUserParams);

            string delStudentGroup = "DELETE FROM StudentGroup WHERE UserId = @userId;";
            Dictionary<string, object> delStudentGroupParams = new Dictionary<string, object>
            {
                ["@userId"] = userId
            };
            _db.ExecuteNonQuery(delStudentGroup, delStudentGroupParams);

            int status = _db.Delete("User", "UserId", userId);

            _db.Close();

            if (status != 202 && status != 404)
            {
                throw new Exception("Failed to delete user in database.");
            }

            return Task.CompletedTask;
        }

        private void UpsertRelations(User user)
        {
            string deleteSchoolUserSql = "DELETE FROM SchoolUser WHERE UserId = @userId;";
            Dictionary<string, object> deleteSchoolUserParams = new Dictionary<string, object>
            {
                ["@userId"] = user.Id
            };
            _db.ExecuteNonQuery(deleteSchoolUserSql, deleteSchoolUserParams);

            if (user.SchoolId > 0)
            {
                Dictionary<string, object> insertSchoolUser = new Dictionary<string, object>
                {
                    ["SchoolId"] = user.SchoolId,
                    ["UserId"] = user.Id
                };
                _db.Create("SchoolUser", insertSchoolUser);
            }

            string deleteStudentGroupSql = "DELETE FROM StudentGroup WHERE UserId = @userId;";
            Dictionary<string, object> deleteStudentGroupParams = new Dictionary<string, object>
            {
                ["@userId"] = user.Id
            };
            _db.ExecuteNonQuery(deleteStudentGroupSql, deleteStudentGroupParams);

            if (user.UserType == UserType.Leerling && user.GroupId > 0)
            {
                Dictionary<string, object> insertStudentGroup = new Dictionary<string, object>
                {
                    ["GroupId"] = user.GroupId,
                    ["UserId"] = user.Id,
                    ["Score"] = 0
                };
                _db.Create("StudentGroup", insertStudentGroup);
            }
        }


        private static User MapUser(DataRow row)
        {
            User user = new User();

            user.Id = Convert.ToInt32(row["UserId"]);
            user.Username = Convert.ToString(row["Username"]) ?? string.Empty;
            user.Email = Convert.ToString(row["Email"]) ?? string.Empty;
            user.Password = Convert.ToString(row["Password"]) ?? string.Empty;

            string roleName = Convert.ToString(row["RoleName"]) ?? string.Empty;
            user.UserType = MapRoleNameToUserType(roleName);

            int statusId = Convert.ToInt32(row["StatusId"]);
            user.IsActive = statusId == 1;

            if (row.Table.Columns.Contains("SchoolId") && row["SchoolId"] != DBNull.Value)
            {
                user.SchoolId = Convert.ToInt32(row["SchoolId"]);
            }

            if (row.Table.Columns.Contains("GroupId") && row["GroupId"] != DBNull.Value)
            {
                user.GroupId = Convert.ToInt32(row["GroupId"]);
            }

            return user;
        }


        private static UserType MapRoleNameToUserType(string roleName)
        {
            if (roleName == "Docent") return UserType.Docent;
            if (roleName == "Leerling") return UserType.Leerling;
            return UserType.Admin;
        }

        private static bool MapStatusNameToIsActive(string statusName)
        {
            if (statusName == "Inactive" || statusName == "Inactief" || statusName == "Disabled") return false;
            if (statusName == "Active" || statusName == "Actief" || statusName == "Enabled") return true;

            return true;
        }
        private bool UsernameExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM User WHERE Username = @username;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@username"] = username.Trim()
            };

            object? result = _db.ExecuteScalar(sql, parameters);
            int count = result == null ? 0 : Convert.ToInt32(result);

            return count > 0;
        }
        private int ResolveRoleId(UserType userType)
        {
            string role = "Admin";
            if (userType == UserType.Docent) role = "Docent";
            if (userType == UserType.Leerling) role = "Leerling";

            string sql = "SELECT RoleId FROM Role WHERE Role = @role LIMIT 1;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@role"] = role
            };

            object? result = _db.ExecuteScalar(sql, parameters);
            if (result == null) return 1;

            return Convert.ToInt32(result);
        }
        private static int ResolveStatusId(bool isActive)
        {
            return isActive ? 1 : 2;
        }
    }
}
