using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Utils;

namespace RunescapeOrganiser {
    /// <summary>
    /// Interaction logic for TaskAddWindow.xaml
    /// </summary>
    public partial class TaskAddWindow : Window {
        private string[] monsterNames;
        private string[] bossMonsterNames;
        private MainWindow mainWindow;
        private SlayerPage slayerPage;
        private string[] notes;
        

        public TaskAddWindow() {
            InitializeComponent();
            this.monsterNames = this.GetMonsterNames().OrderBy(s => s).ToArray();
            this.bossMonsterNames = this.GetBossMonsterNames().OrderBy(s => s).ToArray();
            PopulateViews();
            mainWindow = Application.Current.Windows.OfType<MainWindow>().ElementAt(0);
            if (mainWindow.Tabs.Items[0] is TabItem t) {
                if (t.Content is Frame f) {
                    if (f.Content is SlayerPage p) {
                        slayerPage = p;
                    }
                }
            }
            BossListView.Items.Refresh();
            ContractCheckBox.Visibility = Visibility.Hidden;
        }

        private string[] GetMonsterNames() {
            return Enum.GetValues(typeof(SlayerMonsters)).Cast<SlayerMonsters>().Skip(1).Select(e => Slayer.SlayerLookUpTable[e].Key).ToArray();
        }

        private string[] GetBossMonsterNames() {
            return Enum.GetValues(typeof(BossSlayerMonsters)).Cast<BossSlayerMonsters>().Skip(1).Select(e => Slayer.BossSlayerLookUpTable[e].Key).ToArray();
        }

        private SlayerTask CreateSlayerTask() {
            SlayerTask s = null;
            string monsterName = (string)MonsterListView.SelectedItem;
            SlayerMonsters monsterType = Slayer.SlayerLookUpTable.Where(entry => entry.Value.Key == monsterName).ElementAt(0).Key;
            UInt32.TryParse(MonstersKilledTextBox.Text, out uint amount);
            bool killedBoss = BossCheckBox.IsChecked ?? false;
            uint? bossKills = null;
            BossSlayerMonsters? bType = null;
            if (killedBoss) {
                UInt32.TryParse(BossKillsAmountTextBox.Text, out uint bossAmount);
                bossKills = bossAmount;
                bType = Slayer.BossMonsterTypesLookUpTable.Where(entry => entry.Value == monsterType).ElementAt(0).Key;
            }

            bool iscancelled, isextended;
            iscancelled = CancelledCheckbox.IsChecked ?? false;
            isextended = ExtendedCheckBox.IsChecked ?? false;
            SlayerBonuses codexBonus;
            codexBonus = (SlayerBonuses)Enum.Parse(typeof(SlayerBonuses), (string)CodexBonuses.SelectedItem);
            
            s = new SlayerTask(monsterType, amount, killedBoss, bossKills, bType, iscancelled, isextended, notes);
            if (ContractCheckBox.IsChecked ?? false) {
                s.InitExp(codexBonus, SlayerBonuses.SlayerContract);
            } else {
                s.InitExp(codexBonus);
            }
            return s;
        }

        public void HideBossControls () {
            BossListView.Visibility = Visibility.Hidden;
            BossesLabel.Visibility = Visibility.Hidden;
            BossKillsAmountTextBox.Visibility = Visibility.Hidden;
            BossKillsLabel.Visibility = Visibility.Hidden;
        }

        public void ShowBossControls() {
            BossListView.Visibility = Visibility.Visible;
            BossesLabel.Visibility = Visibility.Visible;
            BossKillsAmountTextBox.Visibility = Visibility.Visible;
            BossKillsLabel.Visibility = Visibility.Visible;
        }

        private void PopulateViews() {
            BossListView.ItemsSource = this.bossMonsterNames;
            MonsterListView.ItemsSource = this.monsterNames;
            CodexBonuses.ItemsSource = Enum.GetNames(typeof(SlayerBonuses)).Reverse().Skip(1).OrderBy(s => s);
            CodexBonuses.SelectedItem = CodexBonuses.Items.GetItemAt(0);
        }

        private void FindMatchingBosses() {
            string monsterName = MonsterListView.SelectedItem as string;
            SlayerMonsters monsterType = Slayer.SlayerLookUpTable.Where(entry => entry.Value.Key == monsterName).ToArray()[0].Key;
            var bossTypes = Slayer.BossMonsterGroups.Where(entry => entry.Value.Contains(monsterType)).Select(en => en.Key);
            string[] bossNames = bossTypes.Select(entry => Slayer.BossSlayerLookUpTable[entry].Key).ToArray();
            BossListView.ItemsSource = bossNames;
            BossListView.UpdateLayout();
            BossListView.Items.Refresh();
        }

        public void SetNotes(string[] notes) {
            this.notes = notes;
        }

        public void AddTask() {
            SlayerTask task = CreateSlayerTask();
            DailySlayerTaskList parent = slayerPage.AddDaily();
            parent.Add(task);
            slayerPage.SlayerTasksView.UpdateLayout();
            slayerPage.UpdateTaskInfoContents();
            this.Close();
        }

        private void ValidateContract() {
            string monsterName = MonsterListView.SelectedItem as string;
            SlayerMonsters monsterType = Slayer.SlayerLookUpTable.Where(entry => entry.Value.Key == monsterName).ElementAt(0).Key;
            if (Slayer.SlayerMonstersWithContractAvailableLookUpTable.Contains(monsterType)) {
                ContractCheckBox.Visibility = Visibility.Visible;
            } else {
                ContractCheckBox.Visibility = Visibility.Hidden;
                ContractCheckBox.IsChecked = false;
            }
        }



        //event handlers

        private void BossCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            HideBossControls();
        }

        private void MonsterSearch(object sender, TextChangedEventArgs e) {
            MonsterListView.ItemsSource = monsterNames.Where(name => name.ToLower().Contains(FindMonstersTextBox.Text.ToLower())).OrderBy(s => s);
        }

        private void CancelledCheckbox_Checked(object sender, RoutedEventArgs e) {
            ExtendedCheckBox.IsChecked = false;
        }

        private void ExtendedCheckBox_Checked(object sender, RoutedEventArgs e) {
            CancelledCheckbox.IsChecked = false;
        }

        private void ListViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (MonsterListView.SelectedItem == null) return;
            FindMatchingBosses();
            ValidateContract();
        }

        private void TextBoxNumberValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !StringUtils.IsNumeric(e.Text);
        }

        private void AddTaskEvent(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Do you have any additional notes?", "Notes", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                try {
                    var t = Application.Current.Windows.OfType<Notes>().ElementAt(0);
                } catch (ArgumentOutOfRangeException) {
                    Notes n = new Notes();
                    n.Show();
                }
            } else AddTask();
        }

        private void BossCheckBox_Checked(object sender, RoutedEventArgs e) {
            ShowBossControls();
            FindMatchingBosses();
        }
    }
}
