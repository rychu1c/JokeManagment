namespace JokeManagment.Server
{
    public class CurrentUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public LearningLevel LearningStatus { get; set; }
        public int LocationId { get; set; }
        public AccessLevel LevelOfAccess { get; set; } = AccessLevel.User;


        public enum AccessLevel
        {
            User,
            Admin
        }

        public enum LearningLevel
        {
            Student = 1,
            Teacher = 2
        }

        public CurrentUser(string login, string password, string name, string surname, LearningLevel status, int location)
        {
            Login = login;
            Password = password;
            Name = name;
            Surname = surname;
            LearningStatus = status;
            LocationId = location;
        }
        public CurrentUser()
        {

        }
    }
}
