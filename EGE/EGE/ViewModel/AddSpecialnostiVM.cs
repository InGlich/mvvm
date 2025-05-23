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
    public class AddSpecialnostiVM : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private Otdelenie _selectedOtdelenie;
        private bool _isEditMode;

        public Specialnosti NewSpecialnost { get; set; } = new Specialnosti();
        public ObservableCollection<Otdelenie> AllOtdelenie { get; set; }
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public Otdelenie SelectedOtdelenie
        {
            get => _selectedOtdelenie;
            set
            {
                _selectedOtdelenie = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        public AddSpecialnostiVM()
        {
            _db = new AppDbContext();
            LoadData();
            SaveCommand = new RelayCommand(Save);
            if (IsEditMode && NewSpecialnost != null)
            {
                SelectedOtdelenie = AllOtdelenie.FirstOrDefault(o => o.IDOtdel == NewSpecialnost.IDOtdel);
            }
        }

        private void Save()
        {
            
                if (SelectedOtdelenie == null)
                {
                    MessageBox.Show("Выберите отделение!");
                    return;
                }

                NewSpecialnost.IDOtdel = SelectedOtdelenie.IDOtdel;

                if (!IsEditMode)
                {
                    _db.Specialnosti.Add(NewSpecialnost);
                }

                _db.SaveChanges();

                MessageBox.Show(IsEditMode ? "Специальность обновлена!" : "Специальность добавлена!");

                if (IsEditMode)
                {
                    Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.DataContext == this)?
                        .DialogResult = true;
                }
                else
                {
                    NewSpecialnost = new Specialnosti();
                    OnPropertyChanged(nameof(NewSpecialnost));
                }
            }
           
        

        private void LoadData()
        {
            _db.Otdelenie.Load();
            AllOtdelenie = _db.Otdelenie.Local.ToObservableCollection();
            OnPropertyChanged(nameof(AllOtdelenie));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
