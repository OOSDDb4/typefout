using System.Data;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class ExerciseRepo : IExerciseRepo
    {
        private readonly IDatabaseService _db;

        public ExerciseRepo(IDatabaseService db)
        {
            _db = db;
        }

        public Task<List<Exercise>> GetAllAsync()
        {
            _db.Open();

            DataTable table = _db.Read(
                table: "Exercise",
                columns: new List<string> { "ExerciseId", "Exercise", "ExerciseActive" },
                orderBy: "ExerciseId"
            );

            _db.Close();

            List<Exercise> exercises = new List<Exercise>();

            foreach (DataRow row in table.Rows)
            {
                Exercise exercise = new Exercise
                {
                    ExerciseId = Convert.ToInt32(row["ExerciseId"]),
                    ExerciseName = Convert.ToString(row["Exercise"]) ?? string.Empty,
                    ExerciseActive = Convert.ToInt32(row["ExerciseActive"]) == 1
                };

                exercises.Add(exercise);
            }

            return Task.FromResult(exercises);
        }

        public Task SetGlobalActiveAsync(int exerciseId, bool active)
        {
            _db.Open();

            string sql = "UPDATE Exercise SET ExerciseActive = @active WHERE ExerciseId = @id;";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@active", active ? 1 : 0 },
                { "@id", exerciseId }
            };

            _db.ExecuteNonQuery(sql, parameters);
            _db.Close();

            return Task.CompletedTask;
        }
    }
}