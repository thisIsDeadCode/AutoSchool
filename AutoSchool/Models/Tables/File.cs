namespace AutoSchool.Models.Tables
{
    public class File
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}
