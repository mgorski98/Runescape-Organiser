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

namespace RunescapeOrganiser {
    /// <summary>
    /// Interaction logic for Notes.xaml
    /// </summary>
    public partial class Notes : Window {

        public Notes() {
            InitializeComponent();
        }

        public string[] GetNotes() {
            string contents = NotesTextbox.Text;
            return contents.Split('\n');
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            var x = Application.Current.Windows.OfType<TaskAddWindow>().ElementAt(0);
            x.SetNotes(this.GetNotes());
            x.AddTask();
        }
    }
}
