using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVVMSample
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private Emp selectedEmp;
        public Emp SelectedEmp
        {
            get
            {
                return selectedEmp;
            }
            set
            {
                selectedEmp = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand AddEmpCommand { get; set; }

        public ObservableCollection<Emp> Emps { get; set; }

        public UserViewModel()
        {
            Emps = new ObservableCollection<Emp>();
            Emps.Add(new Emp { Name = "홍길동", Job = "Salesman" });
            Emps.Add(new Emp { Name = "홍길동", Job = "Salesman" });
            Emps.Add(new Emp { Name = "홍길동", Job = "Salesman" });
            Emps.Add(new Emp { Name = "홍길동", Job = "Salesman" });
            Emps.Add(new Emp { Name = "홍길동", Job = "Salesman" });

            AddEmpCommand = new RelayCommand(AddEmp);
        }

        private void AddEmp(object param)
        {
            Emps.Add(new Emp { Name = param.ToString(), Job = "New Job" });
        }
    }
}
