using Dapper;
using JokeManagment.Server;

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
            TeachingMenu();
        }

        private void TeachingMenu()
        {
            bool isUserQuittedmenu = false;
            while (!isUserQuittedmenu)
            {
                List<string> messages = new List<string>();
                messages.Add("0.Wróć");
                messages.Add("1.Zapisz się na lekcje");
                messages.Add("2.Sprawdz gdzie jesteś zapisany");
                messages.Add("3.Wypisz się z zajęć");
                messages.Add("4.Zostań nauczucielem");

                if (((int)currentUser.LearningStatus) == 2)
                {
                    messages.Add("5.Dodaj nowy przedmiot który uczysz");
                }

                foreach (string m in messages)
                {
                    Console.WriteLine(m);
                }

                string? inputUser = Console.ReadLine();
                bool isValid = int.TryParse(inputUser, out int inputUserInt);
                if (!isValid || messages.Count - 1 < inputUserInt || 0 > inputUserInt)
                {
                    Console.WriteLine("Błędna wpisana wartość. Wciaśnij dowolny przycisk by kontynuować");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                if (0 == inputUserInt)
                {
                    Console.Clear();
                    return;
                }

                Console.Clear();

                switch (inputUserInt)
                {
                    case 1:
                        SignForLesson();
                        break;
                    case 2:
                        CheckYourAssigment();
                        break;
                    case 3:
                        SignOutLesson();
                        break;
                    case 4:
                        if (((int)currentUser.LearningStatus) == 2)
                        {
                            BecomeTeacher();
                            break;
                        }
                        Console.WriteLine("Nie odpowiedznie uprawnienia. Wciśnij dowolny klawisz by kontynuować.");
                        break;
                    case 5:
                        if (((int)currentUser.LearningStatus) == 2)
                        {
                            AddYourSubject();
                            break;
                        }
                        Console.WriteLine("Nie odpowiedznie uprawnienia. Wciśnij dowolny klawisz by kontynuować.");
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void SignForLesson()
        {
            //Check for available subject
            string Sqlstringgetsubjects = $"SELECT * FROM SchoolSubjects;";

            List<SchoolSubjects>? ListSubjects = GetListFromDB<SchoolSubjects>(Sqlstringgetsubjects);
            if (ListSubjects == null)
            {
                Console.WriteLine("Błąd pobrania listy. Wciaśnij dowolny przycisk by kontynuować");
                return;
            }
            if (ListSubjects.Count == 0)
            {
                Console.WriteLine("Nie ma aktualnie żadnego przedmiotu do wybrania. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }


            Console.WriteLine("Wybier jakiego przedmiotu chcesz się uczyć z niżej wymienionych:");
            foreach (SchoolSubjects subjects in ListSubjects)
            {
                Console.WriteLine($"{subjects.id_subject}. {subjects.subject_name}");
            }

            string? inputUser = Console.ReadLine();
            bool isValid = int.TryParse(inputUser, out int inputUserInt);
            if (!isValid)
            {
                Console.WriteLine("Wpisana niepoprawna wartość. Wiciśnij dowolny klawisz by kontynuować.");
                return;
            }

            SchoolSubjects? pickedsubject = ListSubjects.FirstOrDefault<SchoolSubjects>(sub => sub.id_subject == inputUserInt);
            if (pickedsubject == null)
            {
                Console.WriteLine("Nie udało się pobrać wybranego przedmiotu. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            string Sqlstringavailableteachersubject = $"SELECT users.user_id, users.login, users.password, users.name, users.surname, users.learningstatus, users.location_id, users.levelofaccess FROM Users RIGHT JOIN (SELECT Foo.teacher_id FROM (SELECT teacher_id, COUNT(student_id) AS StudentCount FROM Teachers WHERE Teachers.id_subject = {pickedsubject.id_subject} GROUP BY teacher_id HAVING COUNT(student_id) < 3 ORDER BY teacher_id) AS Foo) AS Faa ON user_id = Faa.teacher_id;";
            List<CurrentUser>? teachersList = GetListFromDB<CurrentUser>(Sqlstringavailableteachersubject);
            if (teachersList == null)
            {
                Console.WriteLine("Bład pobrania listy. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
            if (teachersList.Count == 0)
            {
                Console.WriteLine("Nie ma aktualnie wolnego korepetytora dla tego przedmiotu. Wciśnij dowolny klawisz by kontynuować");
            }

            Console.WriteLine($"Wybierz nauczyciela z którym chcesz uczyć się przdmiotu {pickedsubject.subject_name}:");
            foreach (CurrentUser teacher in teachersList)
            {
                Console.WriteLine($"{teachersList.IndexOf(teacher) + 1}.{teacher.Name} {teacher.Surname}");
            }

            string? userInput = Console.ReadLine();
            bool isValid3 = int.TryParse(userInput, out int userInputInt);
            if (!isValid3 || userInputInt > teachersList.Count || userInputInt <= 0)
            {
                Console.WriteLine("Nie wybrano nauczyciela. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
            CurrentUser PickedTeacher = teachersList.ElementAt(userInputInt - 1);
            Console.WriteLine($"Wybrany nauczyciel to {PickedTeacher.Name} {PickedTeacher.Surname}");
            string Sqlstringinsertstudent = $"INSERT INTO Teachers VALUES ({PickedTeacher.user_id}, {currentUser.user_id}, {pickedsubject.id_subject});";
            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{Sqlstringinsertstudent}");
                }
                catch
                {
                    Console.WriteLine("Nie udało się zapisać. Wciśnij dowolny klawisz by kontynuować.");
                    return;
                }
                Console.WriteLine("Zapis powiodło się. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
        }

        private void CheckYourAssigment()
        {
            string Sqlstringcheckyourlesson = $"SELECT user_id, name, surname, SchoolSubjects.subject_name, SchoolSubjects.id_subject FROM Users JOIN Teachers ON Teachers.teacher_id = users.user_id JOIN SchoolSubjects ON SchoolSubjects.id_subject = Teachers.id_subject WHERE Teachers.student_id = {currentUser.user_id};";


            List<StudentTeachers>? StudentTeachers = GetListFromDB<StudentTeachers>(Sqlstringcheckyourlesson);
            if (StudentTeachers == null || StudentTeachers.Count == 0)
            {
                Console.WriteLine("Jeszcze nigdzie nie jesteś zapisany. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            foreach (StudentTeachers teacher in StudentTeachers)
            {
                Console.WriteLine($"{teacher.name} {teacher.surname} uczy cię przedmiotu {teacher.subject_name}");
            }
        }

        private void SignOutLesson()
        {
            string Sqlstringgetyourlesson = $"SELECT user_id, name, surname, SchoolSubjects.subject_name, SchoolSubjects.id_subject FROM Users JOIN Teachers ON Teachers.teacher_id = users.user_id JOIN SchoolSubjects ON SchoolSubjects.id_subject = Teachers.id_subject WHERE Teachers.student_id = {currentUser.user_id};";

            List<StudentTeachers>? TeachersOfStudent = GetListFromDB<StudentTeachers>(Sqlstringgetyourlesson);
            if (TeachersOfStudent == null || TeachersOfStudent.Count == 0)
            {
                Console.WriteLine("Błąd pobrania listy z serwera. Wciśnij dowolny klawisz by kontynuować");
                return;
            }

            Console.WriteLine("Wybierz które zajęcia chcesz usunąć");
            Console.WriteLine("0. Anuluj kasowanie");
            foreach (StudentTeachers Teacher in TeachersOfStudent)
            {
                Console.WriteLine($"{TeachersOfStudent.IndexOf(Teacher) + 1}. Korepetytor to {Teacher.name} {Teacher.surname}, uczy cię {Teacher.subject_name}");
            }

            string? userInput = Console.ReadLine();
            bool isValid = int.TryParse(userInput, out int userInputInt);
            if (!isValid || userInputInt > TeachersOfStudent.Count || userInputInt <= 0)
            {
                Console.WriteLine("Błędna wartość. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
            StudentTeachers PickedStudent = TeachersOfStudent.ElementAt(userInputInt - 1);

            string Sqlstringdelitelesson = $"UPDATE Teachers SET student_id = NULL WHERE teacher_id = {PickedStudent.user_id} AND student_id = {currentUser.user_id} AND id_subject = {PickedStudent.id_subject};";

            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{Sqlstringdelitelesson}");
                }
                catch
                {
                    Console.WriteLine("Nie udało się usunąć. Wciśnij dowolny klawisz by kontynuować.");
                    return;
                }
                Console.WriteLine("Usunięcie powiodło się. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
        }

        private void AddYourSubject()
        {
            string Sqlstringgetsubjects = "SELECT * FROM SchoolSubjects";
            List<SchoolSubjects>? schoolSubjects = GetListFromDB<SchoolSubjects>(Sqlstringgetsubjects);
            if (schoolSubjects == null || schoolSubjects.Count == 0)
            {
                Console.WriteLine("Błąd pobrania listy przedmiotów z serwera. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }

            Console.WriteLine("Który przedmiot chcesz uczyć?");
            foreach (SchoolSubjects subject in schoolSubjects)
            {
                Console.WriteLine($"{schoolSubjects.IndexOf(subject) + 1}. {subject.subject_name}");
            }

            string? userInput = Console.ReadLine();
            bool isVaild = int.TryParse(userInput, out int userInputInt);
            if (!isVaild || userInputInt > schoolSubjects.Count() || userInputInt <= 0)
            {
                Console.WriteLine("Wpisana wartość jest nie poprawna. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
            Console.WriteLine("Wciśnij dowolny klawisz by kontynuować");

            SchoolSubjects subjects = schoolSubjects.ElementAt(userInputInt - 1);
            string Sqlstringaddsubject = $"INSERT INTO Teachers VALUES({currentUser.user_id}, NULL, {subjects.id_subject})";

            using (var RegistrationConnection = ConnectionSQL.EstablishConnection())
            {
                try
                {
                    RegistrationConnection.Execute($"{Sqlstringaddsubject}");
                }
                catch
                {
                    Console.WriteLine("Dodanie przedmiotu nie powioło się. Wciśnij dowolny klawisz by kontynuować.");
                    return;
                }
                Console.WriteLine("Dodanie przedmiotu powiodło się. Wciśnij dowolny klawisz by kontynuować.");
                return;
            }
        }

        private void BecomeTeacher()
        {
            string Sqlstringstatuschange = $"UPDATE Users SET learningstatus = 2 WHERE user_id = {currentUser.user_id};";
            StaticMethods.isExecutSqlString(Sqlstringstatuschange);
        }

        private List<T>? GetListFromDB<T>(string SQLCommand)
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
                    return list = null;
                }
            }
            Console.Clear();
            return list;
        }
    }
}
