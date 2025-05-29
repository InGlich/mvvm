using EGE.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace EGE.ViewModel
{
    public class AddOtdelenieVM : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private Otdelenie _newOtdelenie = new Otdelenie();
        private bool _isEditMode;

        public Otdelenie NewOtdelenie
        {
            get => _newOtdelenie;
            set
            {
                _newOtdelenie = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public AddOtdelenieVM()
        {
            _db = new AppDbContext();
            SaveCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(NewOtdelenie.NameOtdel))
            {
                MessageBox.Show("Введите название отделения!");
                return;
            }

            if (!IsEditMode)
            {
                if (_db.Otdelenie.Any(o => o.NameOtdel == NewOtdelenie.NameOtdel))
                {
                    MessageBox.Show("Отделение с таким названием уже существует!");
                    return;
                }

    

                _db.Otdelenie.Add(NewOtdelenie);
            }

            _db.SaveChanges();

            MessageBox.Show(IsEditMode ? "Отделение успешно обновлено!" : "Отделение успешно добавлено!");

            if (IsEditMode)
            {
                Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this)?
                    .DialogResult = true;
            }
            else
            {
                NewOtdelenie = new Otdelenie(); // Сбрасываем форму
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

