using Dapper;
using JokeManagment.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JokeManagment.Client
{
    public class LessonMenu
    {
        CurrentUser currentUser { get; set; }

        public LessonMenu(CurrentUser _currentUser)
        {
            currentUser = _currentUser;
        }

        public void Menu()
        {
            if (((int)currentUser.LearningStatus) == 2)
            {
                StudentMenu();
            }
            else if(((int)currentUser.LearningStatus) == 1)
            {
                TeacherMenu();
            }
        }

        private void StudentMenu()
        {
            bool isUserQuittedmenu = false;
            while (!isUserQuittedmenu)
            {
                List<string> messages = new List<string>();
                messages.Add("0.Wróć");
                messages.Add("1.Zapisz się na lekcje");
                messages.Add("2.Sprawdz gdzie jesteś zapisany");
                messages.Add("3.Wypisz się z zajęć");

                foreach (string m in messages)
                {
                    Console.WriteLine(m);
                }

                string inputUser = Console.ReadLine();
                bool isValid = int.TryParse(inputUser, out int inputUserInt);
                if (!isValid) { continue; }

                if (messages.Count+1 < inputUserInt || 0 > inputUserInt)
                {
                    Console.WriteLine("Błędna wpisana wartość");
                    continue;
                }
                switch (inputUserInt)
                {
                    case 1:
                        SignForLesson();
                        break;
                    case 2:
                        
                        break;
                    case 3:
                        
                        break;
                    default:
                        return;
                }
            }
        }

        private void TeacherMenu()
        {

            Console.WriteLine("1.Zapisz się na lekcje");
            Console.WriteLine("2.Sprawdz gdzie jesteś zapisany");
            Console.WriteLine("3.Wypisz się z zajęć");
            Console.WriteLine("4.Dodaj nowy przedmiot który uczysz");
            Console.WriteLine("5.Usuń swoje zajecia");
            Console.WriteLine("0.Wróć");
        }

        private void SignForLesson()
        {
            //Check for available subject
            string Sqlstringgetsubjects = $"SELECT * FROM SchoolSubjects;";
            List<SchoolSubjects> ListSubjects = new List<SchoolSubjects>();

            ListSubjects = GetListFromDB<SchoolSubjects>(Sqlstringgetsubjects);
            if (ListSubjects == null) return;

            Console.WriteLine("Wybier jakiego przedmiotu chcesz się uczyć z nizej wymienionych:");
            foreach (SchoolSubjects subjects in ListSubjects)
            {
                Console.WriteLine($"{subjects.id_subject}. {subjects.subject_name}");
            }

            string inputUser = Console.ReadLine();
            bool isValid = int.TryParse(inputUser, out int inputUserInt);
            if (!isValid) 
            {
                Console.WriteLine("Wpisana niepoprawna wartość");
                return; 
            }

            SchoolSubjects pickedsubject = ListSubjects.FirstOrDefault<SchoolSubjects>(sub => sub.id_subject == inputUserInt);
            if (pickedsubject == null)
            {
                Console.WriteLine("Nie udało się pobrać wybranego przedmiotu. Powrót");
                Console.ReadLine();
                return;
            }

            //string Sqlstringgetteacher = $"SELECT * FROM Teachers WHERE id_subject = {pickedsubject.id_subject};";
            string Sqlstringavailableteachersubject = $"SELECT user_id, users.login, users.password, users.name, users.surname, users.learningstatus, users.location_id, users.levelofaccess FROM Users RIGHT JOIN (SELECT Foo.teacher_id FROM (SELECT teacher_id, COUNT(student_id) AS StudentCount FROM Teachers WHERE Teachers.id_subject = {pickedsubject.id_subject} GROUP BY teacher_id HAVING COUNT(student_id) < 3 ORDER BY teacher_id) AS Foo) AS Faa ON user_id = Faa.teacher_id;";
            List<CurrentUser> teachersList = GetListFromDB<CurrentUser>(Sqlstringavailableteachersubject);
            if (teachersList == null)
            {
                Console.WriteLine("Bład pobrania listy uczniów. Powrót");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Wybierz nauczyciela z którym chcesz uczyć się przdmiotu {pickedsubject.id_subject}:");
            foreach (CurrentUser teacher in teachersList)
            {
                Console.WriteLine($"{teachersList.IndexOf(teacher)+1}.{teacher.Name} {teacher.Surname}");
            }

            string userInput = Console.ReadLine();
            bool isValid3 = int.TryParse(userInput, out int userInputInt);
            if (!isValid3 || userInputInt > teachersList.Count || userInputInt < 0)
            {
                Console.WriteLine("Nie wybrano nauczyciela");
                return;
            }
            CurrentUser PickedTeacher = teachersList.ElementAt(userInputInt-1);
            Console.WriteLine($" picked teacher is {PickedTeacher.Name} {PickedTeacher.Surname}");
            string Sqlstringinsertstudent = $"INSERT INTO TEACHER VALUES ({PickedTeacher.Id}, {currentUser.Id}, {pickedsubject.id_subject});";
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{Sqlstringinsertstudent}");
                }
                catch
                {
                    Console.WriteLine("Nie udało się zapisać. Spróbuj ponownie");
                    Console.ReadLine();
                    Console.Clear();
                    return;
                }
                Console.WriteLine("Zapis powiodł się!");
                Console.ReadLine();
                Console.Clear();
                return;
            }
        }

        private List<T> GetListFromDB<T>(string SQLCommand)
        {
            var list = new List<T>();
            using (var loginConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    list = loginConnection.Query<T>($"{SQLCommand}").ToList();//Stored Procedure
                }
                catch
                {
                    Console.WriteLine("Błąd pobrania listy.");
                    return list = null;
                }
            }
            return list;
        }
    }
}
