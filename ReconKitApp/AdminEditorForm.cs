using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ReconKitApp
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class AdminEditorForm : Form
    {
        private long editId;
        public AdminEditorForm(long id = 0)
        {
            InitializeComponent();
            editId = id;
        }

        private void AdminEditorForm_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            if (editId != 0) LoadData();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var user = txtUser.Text.Trim();
            var pass = txtPass.Text.Trim();
            var level = (int)numLevel.Value;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) { MessageBox.Show("Usuario y contraseña requeridos"); return; }

            using (var c = new SQLiteConnection(DataAccess.ConnectionString))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    if (editId == 0)
                    {
                        cmd.CommandText = "INSERT INTO admins (username, password_hash, level) VALUES (@u,@p,@l)";
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE admins SET username=@u, password_hash=@p, level=@l WHERE id=@id";
                        cmd.Parameters.AddWithValue("@id", editId);
                    }
                    cmd.Parameters.AddWithValue("@u", user);
                    cmd.Parameters.AddWithValue("@p", pass);
                    cmd.Parameters.AddWithValue("@l", level);
                    cmd.ExecuteNonQuery();
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadData()
        {
            using (var c = new SQLiteConnection(DataAccess.ConnectionString))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT username, password_hash, level FROM admins WHERE id=@id";
                    cmd.Parameters.AddWithValue("@id", editId);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            txtUser.Text = r.IsDBNull(0) ? "" : r.GetString(0);
                            txtPass.Text = r.IsDBNull(1) ? "" : r.GetString(1);
                            numLevel.Value = r.IsDBNull(2) ? 1 : r.GetInt32(2);
                        }
                    }
                }
            }
        }
    }
}
