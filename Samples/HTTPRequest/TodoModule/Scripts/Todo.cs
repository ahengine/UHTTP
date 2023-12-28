using Newtonsoft.Json;

namespace UHTTP.Sample.TodoModule
{
    public class Todo
    {
        public int userId { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public bool completed { get; set; }

        [JsonConstructor]
        public Todo(int userId, int id, string title, bool completed) {
            this.userId = userId;
            this.id = id;
            this.title = title;
            this.completed = completed;
        }

        public override string ToString() =>
            "[userId: "+userId+"] [Id: "+id+"] "+title+" is "+completed;
    }
}