using System;
using System.Windows.Forms;

namespace ReconKitApp
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAdmins_Click(object sender, EventArgs e)
        {
            int level = GetSelectedLevel();
            var f = new AdminsForm(level);
            f.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            int level = GetSelectedLevel();
            var f = new UsersForm(level);
            f.ShowDialog();
        }

        private int GetSelectedLevel()
        {
            if (cmbLevel.SelectedIndex == 0) return 3;
            if (cmbLevel.SelectedIndex == 1) return 2;
            return 1;
        }
    }
}
