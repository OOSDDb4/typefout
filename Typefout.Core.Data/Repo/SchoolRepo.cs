using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class SchoolRepo : ISchoolRepo
    {
        private readonly IDatabaseService _db;

        public SchoolRepo(IDatabaseService db)
        {
            _db = db;
        }

        public Task<IEnumerable<School>> GetAllAsync()
        {
            _db.Open();

            DataTable dt = _db.Read(
                table: "School",
                columns: new List<string> { "SchoolId", "SchoolName" },
                orderBy: "SchoolId ASC"
            );

            _db.Close();

            IEnumerable<School> schools = dt.AsEnumerable().Select(MapSchool);
            return Task.FromResult(schools);
        }

        public Task<School?> GetByIdAsync(int schoolId)
        {
            _db.Open();

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                ["@schoolId"] = schoolId
            };

            DataTable dt = _db.Read(
                table: "School",
                columns: new List<string> { "SchoolId", "SchoolName" },
                where: "SchoolId = @schoolId",
                parameters: parameters,
                limit: 1
            );

            _db.Close();

            if (dt.Rows.Count == 0) return Task.FromResult<School?>(null);

            School school = MapSchool(dt.Rows[0]);
            return Task.FromResult<School?>(school);
        }

        public Task CreateAsync(School school)
        {
            if (school == null) throw new ArgumentNullException(nameof(school));
            if (string.IsNullOrWhiteSpace(school.Name)) throw new ArgumentException("School name is required.", nameof(school));

            _db.Open();

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["SchoolName"] = school.Name.Trim()
            };

            int newId = _db.CreateAndReturnId("School", data);

            _db.Close();

            if (newId <= 0) throw new Exception("Failed to create school in database.");

            school.Id = newId;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(School school)
        {
            if (school == null) throw new ArgumentNullException(nameof(school));
            if (school.Id <= 0) throw new ArgumentException("Invalid school id.", nameof(school));

            _db.Open();

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["SchoolName"] = school.Name.Trim()
            };

            int status = _db.Update("School", "SchoolId", school.Id.ToString(), data);

            _db.Close();

            if (status != 202 && status != 404) throw new Exception("Failed to update school in database.");

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int schoolId)
        {
            _db.Open();

            int status = _db.Delete("School", "SchoolId", schoolId);

            _db.Close();

            if (status != 202 && status != 404) throw new Exception("Failed to delete school in database.");

            return Task.CompletedTask;
        }

        private static School MapSchool(DataRow row)
        {
            School school = new School();
            school.Id = Convert.ToInt32(row["SchoolId"]);
            school.Name = Convert.ToString(row["SchoolName"]) ?? string.Empty;
            return school;
        }
        public Task DeleteCascadeAsync(int schoolId)
        {
            _db.Open();

            string getGroupsSql = "SELECT GroupId FROM SchoolGroup WHERE SchoolId = @schoolId;";
            Dictionary<string, object> getGroupsParams = new Dictionary<string, object>
            {
                ["@schoolId"] = schoolId
            };

            DataTable groupsTable = _db.ReadQuery(getGroupsSql, getGroupsParams);

            foreach (DataRow row in groupsTable.Rows)
            {
                int groupId = Convert.ToInt32(row["GroupId"]);

                string deleteStudentsSql = "DELETE FROM StudentGroup WHERE GroupId = @groupId;";
                Dictionary<string, object> deleteStudentsParams = new Dictionary<string, object>
                {
                    ["@groupId"] = groupId
                };

                _db.ExecuteNonQuery(deleteStudentsSql, deleteStudentsParams);
            }
            string deleteGroupsSql = "DELETE FROM SchoolGroup WHERE SchoolId = @schoolId;";
            _db.ExecuteNonQuery(deleteGroupsSql, getGroupsParams);

            string deleteSchoolUserSql = "DELETE FROM SchoolUser WHERE SchoolId = @schoolId;";
            _db.ExecuteNonQuery(deleteSchoolUserSql, getGroupsParams);

            int status = _db.Delete("School", "SchoolId", schoolId);

            _db.Close();

            if (status != 202 && status != 404)
            {
                throw new Exception("Failed to delete school in database.");
            }

            return Task.CompletedTask;
        }

    }
}
