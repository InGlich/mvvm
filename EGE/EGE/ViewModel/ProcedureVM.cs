using EGE.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace EGE.ViewModel
{
    public class ProcedureVM : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private Specialnosti _selectedSpecial;

        public ObservableCollection<Specialnosti> Specialnosti { get; set; }
        public ObservableCollection<Abiturient> Applicants { get; set; }
        public ICommand ExecuteProcedureCommand { get; }

        public Specialnosti SelectedSpecial
        {
            get => _selectedSpecial;
            set
            {
                _selectedSpecial = value;
                OnPropertyChanged(nameof(SelectedSpecial));
            }
        }

        public ProcedureVM()
        {
            _db = new AppDbContext();
            ExecuteProcedureCommand = new RelayCommand(ExecuteProcedure);
            LoadSpecialnosti();
        }

        private void LoadSpecialnosti()
        {
            Specialnosti = new ObservableCollection<Specialnosti>(_db.Specialnosti.ToList());
        }

        private void ExecuteProcedure()
        {
            if (SelectedSpecial == null) return;

           
            var count = _db.Database.ExecuteSqlRaw(
                "EXEC CountApplicantsBySpecialnost @SpecialnostId",
                new SqlParameter("@SpecialnostId", SelectedSpecial.IDSpecial)
            );

            Applicants = new ObservableCollection<Abiturient>(
                _db.Abiturient.Where(a => a.IDSpecial == SelectedSpecial.IDSpecial).ToList()
            );

            MessageBox.Show($"На специальности '{SelectedSpecial.NameSpecial}': {count} абитуриентов");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}