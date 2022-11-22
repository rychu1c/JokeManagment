namespace JokeManagment.Server
{
    public class CurrentUser
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public LearningLevel Status { get; set; }
        public string Location { get; set; }
        public AccessLevel LevelOfAccess { get; set; } = AccessLevel.User;

        public enum AccessLevel
        {
            User,
            Admin
        }

        public enum LearningLevel
        {
            Student,
            Teacher
        }

        public CurrentUser(string login, string password, string name, string surname, LearningLevel status, string location)
        {
            Login = login;
            Password = password;
            Name = name;
            Surname = surname;
            Status = status;
            Location = location;
        }
        public CurrentUser()
        {

        }
    }
}
