using EGE.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EGE.ViewModel
{
    public class AddShoolVM : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private School _newShool = new School();
        private bool _isEditMode;

        public School NewShool
        {
            get => _newShool;
            set
            {
                _newShool = value;
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

        public AddShoolVM()
        {
            _db = new AppDbContext();
            SaveCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewShool.NameOrganiz))
                {
                    MessageBox.Show("Введите название школы!");
                    return;
                }

                if (!IsEditMode)
                {
                    if (_db.Shool.Any(s => s.NameOrganiz == NewShool.NameOrganiz))
                    {
                        MessageBox.Show("Школа с таким названием уже существует!");
                        return;
                    }
                    _db.Shool.Add(NewShool);
                }

                _db.SaveChanges();

                MessageBox.Show(IsEditMode ? "Школа обновлена!" : "Школа добавлена!");

                if (IsEditMode)
                {
                    Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this)?
                        .DialogResult = true;
                }
                else
                {
                    NewShool = new School();
                    OnPropertyChanged(nameof(NewShool));
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
