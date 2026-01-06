using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class GroupRepo : IGroupRepo
    {
        private readonly IDatabaseService _db;

        public GroupRepo(IDatabaseService db)
        {
            _db = db;
        }

        public Task<IEnumerable<Group>> GetAllAsync()
        {
            _db.Open();

            string sql =
                "SELECT g.GroupId, g.SchoolId, g.GroupName, g.TeacherId, u.Username AS TeacherName " +
                "FROM SchoolGroup g " +
                "LEFT JOIN User u ON u.UserId = g.TeacherId " +
                "ORDER BY g.GroupId ASC;";

            DataTable dt = _db.ReadQuery(sql);

            _db.Close();

            IEnumerable<Group> groups = dt.AsEnumerable().Select(MapGroup);
            return Task.FromResult(groups);
        }

        public Task<IEnumerable<Group>> GetBySchoolIdAsync(int schoolId)
        {
            _db.Open();

            string sql =
                "SELECT g.GroupId, g.SchoolId, g.GroupName, g.TeacherId, u.Username AS TeacherName " +
                "FROM SchoolGroup g " +
                "LEFT JOIN User u ON u.UserId = g.TeacherId " +
                "WHERE g.SchoolId = @schoolId " +
                "ORDER BY g.GroupId ASC;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@schoolId"] = schoolId
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            IEnumerable<Group> groups = dt.AsEnumerable().Select(MapGroup);
            return Task.FromResult(groups);
        }

        public Task<Group?> GetByIdAsync(int groupId)
        {
            _db.Open();

            string sql =
                "SELECT g.GroupId, g.SchoolId, g.GroupName, g.TeacherId, u.Username AS TeacherName " +
                "FROM SchoolGroup g " +
                "LEFT JOIN User u ON u.UserId = g.TeacherId " +
                "WHERE g.GroupId = @groupId " +
                "LIMIT 1;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@groupId"] = groupId
            };

            DataTable dt = _db.ReadQuery(sql, parameters);

            _db.Close();

            if (dt.Rows.Count == 0) return Task.FromResult<Group?>(null);

            Group group = MapGroup(dt.Rows[0]);
            return Task.FromResult<Group?>(group);
        }

        public Task CreateAsync(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (group.SchoolId <= 0) throw new ArgumentException("SchoolId is required.", nameof(group));
            if (string.IsNullOrWhiteSpace(group.Name)) throw new ArgumentException("Group name is required.", nameof(group));

            _db.Open();

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["SchoolId"] = group.SchoolId,
                ["GroupName"] = group.Name.Trim(),
                ["TeacherId"] = group.TeacherId <= 0 ? DBNull.Value : (object)group.TeacherId
            };

            int newId = _db.CreateAndReturnId("SchoolGroup", data);

            _db.Close();

            if (newId <= 0) throw new Exception("Failed to create group in database.");

            group.Id = newId;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            if (group.Id <= 0) throw new ArgumentException("Invalid group id.", nameof(group));

            _db.Open();

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["GroupName"] = group.Name.Trim(),
                ["TeacherId"] = group.TeacherId <= 0 ? DBNull.Value : (object)group.TeacherId
            };

            int status = _db.Update("SchoolGroup", "GroupId", group.Id.ToString(), data);

            _db.Close();

            if (status != 202 && status != 404) throw new Exception("Failed to update group in database.");

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int groupId)
        {
            _db.Open();

            string deleteStudentsSql = "DELETE FROM StudentGroup WHERE GroupId = @groupId;";
            Dictionary<string, object> deleteStudentsParams = new Dictionary<string, object>
            {
                ["@groupId"] = groupId
            };
            _db.ExecuteNonQuery(deleteStudentsSql, deleteStudentsParams);

            int status = _db.Delete("SchoolGroup", "GroupId", groupId);

            _db.Close();

            if (status != 202 && status != 404) throw new Exception("Failed to delete group in database.");

            return Task.CompletedTask;
        }

        private static Group MapGroup(DataRow row)
        {
            Group group = new Group();
            group.Id = Convert.ToInt32(row["GroupId"]);
            group.SchoolId = Convert.ToInt32(row["SchoolId"]);
            group.Name = Convert.ToString(row["GroupName"]) ?? string.Empty;

            if (row["TeacherId"] == DBNull.Value)
            {
                group.TeacherId = 0;
            }
            else
            {
                group.TeacherId = Convert.ToInt32(row["TeacherId"]);
            }

            group.TeacherName = Convert.ToString(row["TeacherName"]) ?? string.Empty;

            return group;
        }
    }
}
