using System.Data;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class SchoolExerciseRepo : ISchoolExerciseRepo
    {
        private readonly IDatabaseService _db;

        public SchoolExerciseRepo(IDatabaseService db)
        {
            _db = db;
        }

        public Task<List<SchoolExerciseStatus>> GetStatusBySchoolAsync(int schoolId)
        {
            _db.Open();

            string sql = @"
SELECT
    e.ExerciseId,
    e.Exercise AS ExerciseName,
    e.ExerciseActive AS GlobalActive,
    CASE WHEN se.SchoolId IS NULL THEN 0 ELSE 1 END AS Linked,
    COALESCE(se.IsActive, 0) AS SchoolActive
FROM Exercise e
LEFT JOIN SchoolExercise se
    ON se.ExerciseId = e.ExerciseId
   AND se.SchoolId = @schoolId
ORDER BY e.ExerciseId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@schoolId", schoolId }
            };

            DataTable table = _db.ReadQuery(sql, parameters);
            _db.Close();

            List<SchoolExerciseStatus> result = new List<SchoolExerciseStatus>();

            foreach (DataRow row in table.Rows)
            {
                SchoolExerciseStatus status = new SchoolExerciseStatus
                {
                    SchoolId = schoolId,
                    ExerciseId = Convert.ToInt32(row["ExerciseId"]),
                    ExerciseName = Convert.ToString(row["ExerciseName"]) ?? string.Empty,
                    GlobalActive = Convert.ToInt32(row["GlobalActive"]) == 1,
                    Linked = Convert.ToInt32(row["Linked"]) == 1,
                    SchoolActive = Convert.ToInt32(row["SchoolActive"]) == 1
                };

                result.Add(status);
            }

            return Task.FromResult(result);
        }

        public Task LinkAsync(int schoolId, int exerciseId, bool schoolActive)
        {
            _db.Open();

            string sql = @"
INSERT INTO SchoolExercise (SchoolId, ExerciseId, IsActive)
VALUES (@schoolId, @exerciseId, @active)
ON DUPLICATE KEY UPDATE IsActive = VALUES(IsActive);";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@schoolId", schoolId },
                { "@exerciseId", exerciseId },
                { "@active", schoolActive ? 1 : 0 }
            };

            _db.ExecuteNonQuery(sql, parameters);
            _db.Close();

            return Task.CompletedTask;
        }

        public Task UnlinkAsync(int schoolId, int exerciseId)
        {
            _db.Open();

            string sql = "DELETE FROM SchoolExercise WHERE SchoolId = @schoolId AND ExerciseId = @exerciseId;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@schoolId", schoolId },
                { "@exerciseId", exerciseId }
            };

            _db.ExecuteNonQuery(sql, parameters);
            _db.Close();

            return Task.CompletedTask;
        }

        public Task SetSchoolActiveAsync(int schoolId, int exerciseId, bool active)
        {
            _db.Open();

            string sql = @"
UPDATE SchoolExercise
SET IsActive = @active
WHERE SchoolId = @schoolId AND ExerciseId = @exerciseId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@active", active ? 1 : 0 },
                { "@schoolId", schoolId },
                { "@exerciseId", exerciseId }
            };

            _db.ExecuteNonQuery(sql, parameters);
            _db.Close();

            return Task.CompletedTask;
        }
        public Task<List<Exercise>> GetAvailableExercisesForSchoolAsync(int schoolId)
        {
            _db.Open();

            string sql = @"
SELECT e.ExerciseId, e.Exercise, e.ExerciseActive
FROM Exercise e
JOIN SchoolExercise se ON se.ExerciseId = e.ExerciseId
WHERE se.SchoolId = @schoolId
  AND se.IsActive = 1
  AND e.ExerciseActive = 1
ORDER BY e.ExerciseId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@schoolId", schoolId }
            };

            DataTable table = _db.ReadQuery(sql, parameters);
            _db.Close();

            List<Exercise> result = new List<Exercise>();

            foreach (DataRow row in table.Rows)
            {
                Exercise exercise = new Exercise
                {
                    ExerciseId = Convert.ToInt32(row["ExerciseId"]),
                    ExerciseName = Convert.ToString(row["Exercise"]) ?? string.Empty,
                    ExerciseActive = Convert.ToInt32(row["ExerciseActive"]) == 1
                };

                result.Add(exercise);
            }

            return Task.FromResult(result);
        }
    }
}


