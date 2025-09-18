using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace ReconKitApp
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class UsersForm : Form
    {
        private int currentLevel;
        public UsersForm(int level)
        {
            InitializeComponent();
            currentLevel = level;
        }

        private void UsersForm_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            DataAccess.EnsureDatabase();
            RefreshGrid();
            ApplyPermissions();
        }

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshGrid();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (currentLevel>=2) ShowEditor();
            else MessageBox.Show("No autorizado: nivel 2 requerido para agregar");
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currentLevel>=2) EditSelected();
            else MessageBox.Show("No autorizado: nivel 2 requerido para modificar");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentLevel==3) DeleteSelected();
            else MessageBox.Show("No autorizado: nivel 3 requerido para eliminar");
        }

        private void ApplyPermissions()
        {
            btnAdd.Enabled = (currentLevel>=2);
            btnEdit.Enabled = (currentLevel>=2);
            btnDelete.Enabled = (currentLevel==3);
        }

        private void RefreshGrid()
        {
            using (var c = new SQLiteConnection(DataAccess.ConnectionString))
            {
                c.Open();
                using (var da = new SQLiteDataAdapter("SELECT id, dni, apellido, nombre, id_familia, fecha_nac, direccion, provincia, localidad FROM users", c))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
        }

        private void ShowEditor(long id = 0)
        {
            var dlg = new UserEditorForm(id);
            if (dlg.ShowDialog()==DialogResult.OK) RefreshGrid();
        }

        private void EditSelected()
        {
            if (dgv.SelectedRows.Count==0) { MessageBox.Show("Seleccione una fila"); return; }
            var id = Convert.ToInt64(dgv.SelectedRows[0].Cells["id"].Value);
            ShowEditor(id);
        }

        private void DeleteSelected()
        {
            if (dgv.SelectedRows.Count==0) { MessageBox.Show("Seleccione una fila"); return; }
            var id = Convert.ToInt64(dgv.SelectedRows[0].Cells["id"].Value);
            if (MessageBox.Show("Eliminar usuario ID " + id + "?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var c = new SQLiteConnection(DataAccess.ConnectionString))
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM users WHERE id=@id";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                RefreshGrid();
            }
        }
    }
}
