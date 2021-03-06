﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SmartVocabulary.UI
{
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            txtAssemblyVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtIconLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void tbxMail_GotMouseCapture(object sender, MouseEventArgs e)
        {
            this.tbxMail.SelectAll();
        }

        private void btnViewReadme_Click(object sender, RoutedEventArgs e)
        {
            string path = String.Format("{0}\\README.rtf", AppDomain.CurrentDomain.BaseDirectory);
            Process.Start(path);
        }

        private void btnViewLicense_Click(object sender, RoutedEventArgs e)
        {
            string path = String.Format("{0}\\License.txt", AppDomain.CurrentDomain.BaseDirectory);
            Process.Start(path);
        }
    }
}