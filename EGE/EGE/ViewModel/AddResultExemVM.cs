using EGE.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EGE.ViewModel
{
    public class AddResultExemVM : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private Abiturient _selectedAbiturient;
        private bool _isEditMode;

        public ResultExem NewResult { get; set; } = new ResultExem();
        public ObservableCollection<Abiturient> AllAbiturients { get; set; }
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public Abiturient SelectedAbiturient
        {
            get => _selectedAbiturient;
            set
            {
                _selectedAbiturient = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public AddResultExemVM()
        {
            _db = new AppDbContext();
            LoadData();
            SaveCommand = new RelayCommand(Save);
            if (IsEditMode && NewResult != null)
            {
                SelectedAbiturient = AllAbiturients.FirstOrDefault(a => a.IDAbitur == NewResult.IDAbitur);
            }
        }

        private void Save()
        {
           
                if (SelectedAbiturient == null)
                {
                    MessageBox.Show("Выберите абитуриента!");
                    return;
                }

                NewResult.IDAbitur = SelectedAbiturient.IDAbitur;

                if (!IsEditMode)
                {
                    _db.ResultExem.Add(NewResult);
                }

                _db.SaveChanges();

                MessageBox.Show(IsEditMode ? "Результат обновлен!" : "Результат добавлен!");

                if (IsEditMode)
                {
                    Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this)?
                        .DialogResult = true;
                }
                else
                {
                    NewResult = new ResultExem();
                    OnPropertyChanged(nameof(NewResult));
                }
            }
           
        

        private void LoadData()
        {
            _db.Abiturient.Load();
            AllAbiturients = _db.Abiturient.Local.ToObservableCollection();
            OnPropertyChanged(nameof(AllAbiturients));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
