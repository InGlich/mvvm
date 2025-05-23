using EGE.Model;
using EGE.View;
using GalaSoft.MvvmLight.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;
namespace EGE.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly AppDbContext _db;

        // Основные коллекции данных
        private ObservableCollection<Abiturient> _allAbiturients;
        private ObservableCollection<School> _allShools;
        private ObservableCollection<Otdelenie> _allOtdelenie;
        private ObservableCollection<Specialnosti> _allSpecialnosti;
        private ObservableCollection<ResultExem> _allResults;

        // Для поиска
        private string _searchLastName;
        private ObservableCollection<Abiturient> _filteredAbiturients;

        // Для процедуры
        private Specialnosti _selectedSpecialForProcedure;
        private ObservableCollection<Specialnosti> _specialnostiForProcedure;
        private ObservableCollection<Abiturient> _applicantsFromProcedure;

        // Выбранные элементы
        private Abiturient _selectedAbiturient;
        private School _selectedShool;
        private Otdelenie _selectedOtdelenie;
        private Specialnosti _selectedSpecialnost;
        private ResultExem _selectedResult;


        public MainViewModel()
        {
            _db = new AppDbContext();
            InitializeCommands();
            LoadData();
        }

        private void InitializeCommands()
        {
            // Команды добавления
            AddAbiturientCommand = new RelayCommand(AddAbiturient);
            AddShoolCommand = new RelayCommand(AddShool);
            AddOtdelenieCommand = new RelayCommand(AddOtdelenie);
            AddSpecialnostCommand = new RelayCommand(AddSpecialnost);
            AddResultCommand = new RelayCommand(AddResult);
            ExportCommand = new RelayCommand(ExportToExcel);
            ExecuteProcedureCommand = new RelayCommand(ExecuteProcedure);

            // Команды удаления
            DeleteAbiturientCommand = new RelayCommand(DeleteAbiturient, CanDeleteAbiturient);
            DeleteShoolCommand = new RelayCommand(DeleteShool, CanDeleteShool);
            DeleteOtdelenieCommand = new RelayCommand(DeleteOtdelenie, CanDeleteOtdelenie);
            DeleteSpecialnostCommand = new RelayCommand(DeleteSpecialnost, CanDeleteSpecialnost);
            DeleteResultCommand = new RelayCommand(DeleteResult, CanDeleteResult);

            // Команды редактирования
            EditAbiturientCommand = new RelayCommand(EditAbiturient, () => SelectedAbiturient != null);
            EditShoolCommand = new RelayCommand(EditShool, () => SelectedShool != null);
            EditOtdelenieCommand = new RelayCommand(EditOtdelenie, () => SelectedOtdelenie != null);
            EditSpecialnostCommand = new RelayCommand(EditSpecialnost, () => SelectedSpecialnost != null);
            EditResultCommand = new RelayCommand(EditResult, () => SelectedResult != null);
        }



        // Основные коллекции
        public ObservableCollection<Abiturient> AllAbiturients
        {
            get => _allAbiturients;
            set { _allAbiturients = value; OnPropertyChanged(); }
        }

        public ObservableCollection<School> AllShools
        {
            get => _allShools;
            set { _allShools = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Otdelenie> AllOtdelenie
        {
            get => _allOtdelenie;
            set { _allOtdelenie = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Specialnosti> AllSpecialnosti
        {
            get => _allSpecialnosti;
            set { _allSpecialnosti = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ResultExem> AllResults
        {
            get => _allResults;
            set { _allResults = value; OnPropertyChanged(); }
        }

        // Для поиска
        public string SearchLastName
        {
            get => _searchLastName;
            set
            {
                _searchLastName = value;
                OnPropertyChanged();
                FilterAbiturients();
            }
        }

        public ObservableCollection<Abiturient> FilteredAbiturients
        {
            get => _filteredAbiturients;
            set
            {
                _filteredAbiturients = value;
                OnPropertyChanged();
            }
        }

        // Для процедуры
        public Specialnosti SelectedSpecialForProcedure
        {
            get => _selectedSpecialForProcedure;
            set
            {
                _selectedSpecialForProcedure = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Specialnosti> SpecialnostiForProcedure
        {
            get => _specialnostiForProcedure;
            set
            {
                _specialnostiForProcedure = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Abiturient> ApplicantsFromProcedure
        {
            get => _applicantsFromProcedure;
            set
            {
                _applicantsFromProcedure = value;
                OnPropertyChanged();
            }
        }

        public Abiturient SelectedAbiturient
        {
            get => _selectedAbiturient;
            set { _selectedAbiturient = value; OnPropertyChanged(); }
        }

        public School SelectedShool
        {
            get => _selectedShool;
            set { _selectedShool = value; OnPropertyChanged(); }
        }

        public Otdelenie SelectedOtdelenie
        {
            get => _selectedOtdelenie;
            set { _selectedOtdelenie = value; OnPropertyChanged(); }
        }

        public Specialnosti SelectedSpecialnost
        {
            get => _selectedSpecialnost;
            set { _selectedSpecialnost = value; OnPropertyChanged(); }
        }

        public ResultExem SelectedResult
        {
            get => _selectedResult;
            set { _selectedResult = value; OnPropertyChanged(); }
        }


        // Команды добавления
        public ICommand AddAbiturientCommand { get; private set; }
        public ICommand AddShoolCommand { get; private set; }
        public ICommand AddOtdelenieCommand { get; private set; }
        public ICommand AddSpecialnostCommand { get; private set; }
        public ICommand AddResultCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand ExecuteProcedureCommand { get; private set; }

        // Команды удаления
        public ICommand DeleteAbiturientCommand { get; private set; }
        public ICommand DeleteShoolCommand { get; private set; }
        public ICommand DeleteOtdelenieCommand { get; private set; }
        public ICommand DeleteSpecialnostCommand { get; private set; }
        public ICommand DeleteResultCommand { get; private set; }

        // Команды редактирования
        public ICommand EditAbiturientCommand { get; private set; }
        public ICommand EditShoolCommand { get; private set; }
        public ICommand EditOtdelenieCommand { get; private set; }
        public ICommand EditSpecialnostCommand { get; private set; }
        public ICommand EditResultCommand { get; private set; }


        private void LoadData()
        {
            _db.ChangeTracker.Entries().ToList().ForEach(e => e.Reload());

            _db.Abiturient
                .Include(a => a.Shool)
                .Include(a => a.Specialnosti)
                .Load();

            _db.Specialnosti
                .Include(s => s.Otdelenie)
                .Load();

            _db.ResultExem
                .Include(r => r.Abiturient)
                .Load();

            _db.Otdelenie.Load();
            _db.Shool.Load();

            AllAbiturients = new ObservableCollection<Abiturient>(_db.Abiturient.Local);
            AllShools = new ObservableCollection<School>(_db.Shool.Local);
            AllOtdelenie = new ObservableCollection<Otdelenie>(_db.Otdelenie.Local);
            AllSpecialnosti = new ObservableCollection<Specialnosti>(_db.Specialnosti.Local);
            AllResults = new ObservableCollection<ResultExem>(_db.ResultExem.Local);

            SpecialnostiForProcedure = new ObservableCollection<Specialnosti>(_db.Specialnosti.Local);
            ApplicantsFromProcedure = new ObservableCollection<Abiturient>();

            FilterAbiturients();
        }

        private void FilterAbiturients()
        {
            if (AllAbiturients == null) return;

            if (string.IsNullOrWhiteSpace(SearchLastName))
            {
                FilteredAbiturients = new ObservableCollection<Abiturient>(AllAbiturients);
            }
            else
            {
                var filtered = AllAbiturients
                    .Where(a => a.LastName?.Contains(SearchLastName, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();

                FilteredAbiturients = new ObservableCollection<Abiturient>(filtered);
            }
        }

       

        // Добавление
        private void AddAbiturient()
        {
            var window = new AddAbiturientWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddAbiturientVM()
            };
            if (window.ShowDialog() == true)
            {
                LoadData();
                _db.ChangeTracker.Entries().ToList().ForEach(e => e.Reload());
                FilterAbiturients();
            }
        }

        private void AddShool()
        {
            var window = new AddShoolWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddShoolVM()
            };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void AddOtdelenie()
        {
            var window = new AddOtdelenieWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddOtdelenieVM()
            };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void AddSpecialnost()
        {
            var window = new AddSpecialnostiWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddSpecialnostiVM()
            };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void AddResult()
        {
            var window = new AddResultExemWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = new AddResultExemVM()
            };
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void ExportToExcel()
        {
            try
            {
               
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Add();
                var worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet;

                
                ExportAbiturientsToExcel(worksheet);

                
                var schoolsWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.Add();
                schoolsWorksheet.Name = "Школы";
                ExportSchoolsToExcel(schoolsWorksheet);

               
                var specialsWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.Add();
                specialsWorksheet.Name = "Специальности";
                ExportSpecialnostiToExcel(specialsWorksheet);

                
                foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in workbook.Worksheets)
                {
                    sheet.Columns.AutoFit();
                }

                string fileName = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"Отчет_ЕГЭ_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                workbook.SaveAs(fileName);
                excelApp.Visible = true;

                MessageBox.Show($"Отчет успешно сохранен: {fileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}");
            }
        }

        private void ExportAbiturientsToExcel(Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            worksheet.Name = "Абитуриенты";

            worksheet.Cells[1, 1] = "№";
            worksheet.Cells[1, 2] = "Фамилия";
            worksheet.Cells[1, 3] = "Школа";
            worksheet.Cells[1, 4] = "Специальность";
            worksheet.Cells[1, 5] = "Балл аттестата";
            worksheet.Cells[1, 6] = "Телефон";

            var abiturients = _db.Abiturient
                .Include(a => a.Shool)
                .Include(a => a.Specialnosti)
                .ToList();

            for (int i = 0; i < abiturients.Count; i++)
            {
                var abiturient = abiturients[i];
                worksheet.Cells[i + 2, 1] = i + 1;
                worksheet.Cells[i + 2, 2] = abiturient.LastName;
                worksheet.Cells[i + 2, 3] = abiturient.Shool?.NameOrganiz;
                worksheet.Cells[i + 2, 4] = abiturient.Specialnosti?.NameSpecial;
                worksheet.Cells[i + 2, 5] = abiturient.CredBal;
                worksheet.Cells[i + 2, 6] = abiturient.Phone;
            }

            worksheet.Cells[abiturients.Count + 2, 1] = "Итого абитуриентов:";
            worksheet.Cells[abiturients.Count + 2, 2] = abiturients.Count;

            FormatExcelWorksheet(worksheet, "A1:F1");
        }

        private void ExportSchoolsToExcel(Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            worksheet.Cells[1, 1] = "№";
            worksheet.Cells[1, 2] = "Название школы";
            worksheet.Cells[1, 3] = "Адрес";
            worksheet.Cells[1, 4] = "Кол-во абитуриентов";

            
            var schools = _db.Shool.Include(s => s.Abiturients).ToList();

            for (int i = 0; i < schools.Count; i++)
            {
                var school = schools[i];
                worksheet.Cells[i + 2, 1] = i + 1;
                worksheet.Cells[i + 2, 2] = school.NameOrganiz;
                worksheet.Cells[i + 2, 3] = school.Adres;
                worksheet.Cells[i + 2, 4] = school.Abiturients?.Count ?? 0;
            }

            
            worksheet.Cells[schools.Count + 2, 1] = "Итого школ:";
            worksheet.Cells[schools.Count + 2, 2] = schools.Count;

            
            FormatExcelWorksheet(worksheet, "A1:D1");
        }

        private void ExportSpecialnostiToExcel(Microsoft.Office.Interop.Excel.Worksheet worksheet)
        {
            worksheet.Cells[1, 1] = "№";
            worksheet.Cells[1, 2] = "Шифр";
            worksheet.Cells[1, 3] = "Название";
            worksheet.Cells[1, 4] = "Отделение";
            worksheet.Cells[1, 5] = "Форма обучения";
            worksheet.Cells[1, 6] = "Бюджет";
            worksheet.Cells[1, 7] = "Кол-во мест";
            worksheet.Cells[1, 8] = "Кол-во абитуриентов";

            var specials = _db.Specialnosti
                .Include(s => s.Otdelenie)
                .Include(s => s.Abiturients)
                .ToList();

            for (int i = 0; i < specials.Count; i++)
            {
                var spec = specials[i];
                worksheet.Cells[i + 2, 1] = i + 1;
                worksheet.Cells[i + 2, 2] = spec.Shifr;
                worksheet.Cells[i + 2, 3] = spec.NameSpecial;
                worksheet.Cells[i + 2, 4] = spec.Otdelenie?.NameOtdel;
                worksheet.Cells[i + 2, 5] = spec.FormaObychen;
                worksheet.Cells[i + 2, 6] = spec.Bydzhet ? "Да" : "Нет";
                worksheet.Cells[i + 2, 7] = spec.CountMest;
                worksheet.Cells[i + 2, 8] = spec.Abiturients?.Count ?? 0;
            }

            worksheet.Cells[specials.Count + 2, 1] = "Итого специальностей:";
            worksheet.Cells[specials.Count + 2, 2] = specials.Count;

            FormatExcelWorksheet(worksheet, "A1:H1");
        }

        private void FormatExcelWorksheet(Microsoft.Office.Interop.Excel.Worksheet worksheet, string headerRange)
        {
            // Форматирование заголовков
            var range = worksheet.Range[headerRange];
            range.Font.Bold = true;
            range.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightGray;
            range.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

            // Добавляем границы для данных
            var dataRange = worksheet.UsedRange;
            dataRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;
        }

        // Удаление
        private bool CanDeleteAbiturient() => SelectedAbiturient != null;
        private bool CanDeleteShool() => SelectedShool != null;
        private bool CanDeleteOtdelenie() => SelectedOtdelenie != null;
        private bool CanDeleteSpecialnost() => SelectedSpecialnost != null;
        private bool CanDeleteResult() => SelectedResult != null;

        private void DeleteAbiturient()
        {
            if (SelectedAbiturient == null) return;

            try
            {
                _db.Abiturient.Remove(SelectedAbiturient);
                _db.SaveChanges();
                LoadData();
                MessageBox.Show("Абитуриент успешно удален");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void DeleteShool()
        {
            if (SelectedShool == null) return;

            try
            {
                _db.Shool.Remove(SelectedShool);
                _db.SaveChanges();
                LoadData();
                MessageBox.Show("Школа успешно удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void DeleteOtdelenie()
        {
            if (SelectedOtdelenie == null) return;

            try
            {
                _db.Otdelenie.Remove(SelectedOtdelenie);
                _db.SaveChanges();
                LoadData();
                MessageBox.Show("Отделение успешно удалено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void DeleteSpecialnost()
        {
            if (SelectedSpecialnost == null) return;

            try
            {
                _db.Specialnosti.Remove(SelectedSpecialnost);
                _db.SaveChanges();
                LoadData();
                MessageBox.Show("Специальность успешно удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void DeleteResult()
        {
            if (SelectedResult == null) return;

            try
            {
                _db.ResultExem.Remove(SelectedResult);
                _db.SaveChanges();
                LoadData();
                MessageBox.Show("Результат ЕГЭ успешно удален");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        // Редактирование
        private void EditAbiturient()
        {
            var vm = new AddAbiturientVM
            {
                NewAbiturient = SelectedAbiturient,
                SelectedShool = SelectedAbiturient.Shool,
                SelectedSpecialnost = SelectedAbiturient.Specialnosti,
                IsEditMode = true
            };

            var window = new AddAbiturientWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (window.ShowDialog() == true)
            {
                _db.SaveChanges();
                LoadData();
            }
        }

        private void EditShool()
        {
            var vm = new AddShoolVM
            {
                NewShool = SelectedShool,
                IsEditMode = true
            };

            var window = new AddShoolWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (window.ShowDialog() == true)
            {
                _db.SaveChanges();
                LoadData();
            }
        }

        private void EditOtdelenie()
        {
            var vm = new AddOtdelenieVM
            {
                NewOtdelenie = SelectedOtdelenie,
                IsEditMode = true
            };

            var window = new AddOtdelenieWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (window.ShowDialog() == true)
            {
                _db.SaveChanges();
                LoadData();
            }
        }

        private void EditSpecialnost()
        {
            var vm = new AddSpecialnostiVM
            {
                NewSpecialnost = SelectedSpecialnost,
                SelectedOtdelenie = SelectedSpecialnost.Otdelenie,
                IsEditMode = true
            };

            var window = new AddSpecialnostiWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (window.ShowDialog() == true)
            {
                _db.SaveChanges();
                LoadData();
            }
        }

        private void EditResult()
        {
            var vm = new AddResultExemVM
            {
                NewResult = SelectedResult,
                SelectedAbiturient = SelectedResult.Abiturient,
                IsEditMode = true
            };

            var window = new AddResultExemWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (window.ShowDialog() == true)
            {
                _db.SaveChanges();
                LoadData();
            }
        }

        // Процедура
        private void ExecuteProcedure()
        {
            if (SelectedSpecialForProcedure == null)
            {
                MessageBox.Show("Выберите специальность");
                return;
            }

            var specialnostId = SelectedSpecialForProcedure.IDSpecial;

            var count = _db.Database.ExecuteSqlRaw(
                "EXEC CountApplicantsBySpecialnost {0}",
                specialnostId
            );

            ApplicantsFromProcedure = new ObservableCollection<Abiturient>(
                _db.Abiturient
                    .Include(a => a.Shool)
                    .Where(a => a.IDSpecial == specialnostId)
                    .ToList()
            );

            MessageBox.Show($"На специальности '{SelectedSpecialForProcedure.NameSpecial}': {ApplicantsFromProcedure.Count} абитуриентов");
        }

       

        //INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}