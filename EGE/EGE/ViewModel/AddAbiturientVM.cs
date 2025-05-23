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
    public class AddAbiturientVM: INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private School _selectedShool;
        private Specialnosti _selectedSpecialnost;
        private bool _isEditMode;

        public Abiturient NewAbiturient { get; set; } = new Abiturient();
        public ObservableCollection<School> AllShools { get; set; }
        public ObservableCollection<Specialnosti> AllSpecialnosti { get; set; }
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public School SelectedShool
        {
            get => _selectedShool;
            set
            {
                _selectedShool = value;
                OnPropertyChanged();
            }
        }

        public Specialnosti SelectedSpecialnost
        {
            get => _selectedSpecialnost;
            set
            {
                _selectedSpecialnost = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public AddAbiturientVM()
        {
            _db = new AppDbContext();
            LoadData();
            SaveCommand = new RelayCommand(Save);

            // Инициализация выбранных значений при редактировании
            if (IsEditMode && NewAbiturient != null)
            {
                SelectedShool = AllShools.FirstOrDefault(s => s.IDSchool == NewAbiturient.IDSchool);
                SelectedSpecialnost = AllSpecialnosti.FirstOrDefault(sp => sp.IDSpecial == NewAbiturient.IDSpecial);
            }
        }
        
        private void Save()
        {
            
                if (SelectedShool == null || SelectedSpecialnost == null)
                {
                    MessageBox.Show("Выберите школу и специальность!");
                    return;
                }

                NewAbiturient.IDSchool = SelectedShool.IDSchool;
                NewAbiturient.IDSpecial = SelectedSpecialnost.IDSpecial;

                if (!IsEditMode)
                {
                    _db.Abiturient.Add(NewAbiturient);
                }

                _db.SaveChanges();
                
                MessageBox.Show(IsEditMode ? "Абитуриент обновлен!" : "Абитуриент добавлен!");

                if (IsEditMode)
                {
                    Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this)?
                        .DialogResult = true;
                }
                else
                {
                    NewAbiturient = new Abiturient();
                    OnPropertyChanged(nameof(NewAbiturient));
                }
            }
           
        

        private void LoadData()
        {
            _db.Shool.Load();
            _db.Specialnosti.Load();
            AllShools = _db.Shool.Local.ToObservableCollection();
            AllSpecialnosti = _db.Specialnosti.Local.ToObservableCollection();
            OnPropertyChanged(nameof(AllShools)); // Уведомление об изменении коллекций
            OnPropertyChanged(nameof(AllSpecialnosti));

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }

}

