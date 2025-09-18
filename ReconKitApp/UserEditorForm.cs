using System;
using System.Windows.Forms;

namespace ReconKitApp
{
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class UserEditorForm : Form
    {
        private long editId;
        public UserEditorForm(long id = 0)
        {
            InitializeComponent();
            editId = id;
        }

        private void UserEditorForm_Load(object sender, EventArgs e)
        {
            // Evitar accesos runtime en design-time
            if (DesignMode) return;
            if (editId != 0) LoadData();
        }

        private void btnGenFamilia_Click(object sender, EventArgs e) => GenerateFamilyId();

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var dni = txtDni.Text.Trim();
            var apellido = txtApellido.Text.Trim();
            var nombre = txtNombre.Text.Trim();
            var idfamilia = txtIdFamilia.Text.Trim();
            var fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
            var direccion = txtDireccion.Text.Trim();
            var provincia = txtProvincia.Text.Trim();
            var localidad = txtLocalidad.Text.Trim();
            var integrantes = txtIntegrantes.Text.Trim();
            var notas = txtNotas.Text.Trim();

            using (var c = new System.Data.SQLite.SQLiteConnection(DataAccess.ConnectionString))
            {
                c.Open();
                using (var cmd = c.CreateCommand())
                {
                    if (editId == 0)
                    {
                        cmd.CommandText = @"
INSERT INTO users (dni, apellido, nombre, id_familia, fecha_nac, direccion, provincia, localidad, integrantes, notas) 
VALUES (@dni,@ape,@nom,@fam,@fec,@dir,@prov,@loc,@intg,@not)";
                    }
                    else
                    {
                        cmd.CommandText = @"
UPDATE users SET dni=@dni, apellido=@ape, nombre=@nom, id_familia=@fam, fecha_nac=@fec, direccion=@dir, provincia=@prov, localidad=@loc, integrantes=@intg, notas=@not WHERE id=@id";
                        cmd.Parameters.AddWithValue("@id", editId);
                    }

                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@ape", apellido);
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@fam", idfamilia);
                    cmd.Parameters.AddWithValue("@fec", fecha);
                    cmd.Parameters.AddWithValue("@dir", direccion);
                    cmd.Parameters.AddWithValue("@prov", provincia);
                    cmd.Parameters.AddWithValue("@loc", localidad);
                    cmd.Parameters.AddWithValue("@intg", integrantes);
                    cmd.Parameters.AddWithValue("@not", notas);
                    cmd.ExecuteNonQuery();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadData()
        {
            try
            {
                using (var c = new System.Data.SQLite.SQLiteConnection(DataAccess.ConnectionString))
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = "SELECT dni, apellido, nombre, id_familia, fecha_nac, direccion, provincia, localidad, integrantes, notas FROM users WHERE id=@id";
                        cmd.Parameters.AddWithValue("@id", editId);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                txtDni.Text = r.IsDBNull(0) ? "" : r.GetString(0);
                                txtApellido.Text = r.IsDBNull(1) ? "" : r.GetString(1);
                                txtNombre.Text = r.IsDBNull(2) ? "" : r.GetString(2);
                                txtIdFamilia.Text = r.IsDBNull(3) ? "" : r.GetString(3);
                                if (!r.IsDBNull(4)) DateTime.TryParse(r.GetString(4), out DateTime dt);
                                txtDireccion.Text = r.IsDBNull(5) ? "" : r.GetString(5);
                                txtProvincia.Text = r.IsDBNull(6) ? "" : r.GetString(6);
                                txtLocalidad.Text = r.IsDBNull(7) ? "" : r.GetString(7);
                                txtIntegrantes.Text = r.IsDBNull(8) ? "" : r.GetString(8);
                                txtNotas.Text = r.IsDBNull(9) ? "" : r.GetString(9);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadData error: " + ex.Message);
            }
        }

        private void GenerateFamilyId()
        {
            try
            {
                var apellido = (txtApellido.Text ?? "").Trim().ToUpper();
                var dni = (txtDni.Text ?? "").Trim();
                if (apellido.Length == 0 || dni.Length == 0) { MessageBox.Show("Apellido y DNI requeridos para generar ID de familia"); return; }
                var key = apellido;
                if (key.Length > 5) key = key.Substring(0, 5);
                string baseFam = key + "+" + dni + "+";
                int nextNum = 1;
                using (var c = new System.Data.SQLite.SQLiteConnection(DataAccess.ConnectionString))
                {
                    c.Open();
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = "SELECT id_familia FROM users WHERE id_familia LIKE @b";
                        cmd.Parameters.AddWithValue("@b", baseFam + "%");
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                var v = r.IsDBNull(0) ? "" : r.GetString(0);
                                if (v.StartsWith(baseFam))
                                {
                                    var tail = v.Substring(baseFam.Length);
                                    if (int.TryParse(tail, out int n)) if (n >= nextNum) nextNum = n + 1;
                                }
                            }
                        }
                    }
                }
                var famid = baseFam + nextNum.ToString();
                txtIdFamilia.Text = famid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GenerateFamilyId error: " + ex.Message);
            }
        }
    }
}
