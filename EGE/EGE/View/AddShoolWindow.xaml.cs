﻿using EGE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EGE.View
{
    /// <summary>
    /// Логика взаимодействия для AddShoolWindow.xaml
    /// </summary>
    public partial class AddShoolWindow : Window
    {
        public AddShoolWindow()
        {
            InitializeComponent();
            DataContext = new AddShoolVM();

        }
    }
}
