using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_TEST.User
{
    public class UserSessionSet
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
    }

    public class UserSession : INotifyPropertyChanged
    {
        private int _id;
        private string _email = "";

        public int Id
        {
            get => _id;
            set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        public string Email
        {
            get => _email;
            set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }

        private static UserSession? _instance;
        public static UserSession Instance => _instance ??= new UserSession();

        public void Set(UserSessionSet uss)
        {
            if (uss == null)
            {
                Clear();
                return;
            }
            Id = uss.Id;
            Email = uss.Email;
        }

        public void Clear()
        {
            Id = 0;
            Email = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
