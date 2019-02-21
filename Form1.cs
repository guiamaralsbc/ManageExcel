using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace ManageExcel.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnArquivo_Click(object sender, EventArgs e)
        {

            if (ofdDestino.ShowDialog() == DialogResult.OK)
            {
                txtOrigem.Text = ofdDestino.FileName;
            }
        }

        private void btnPasta_Click(object sender, EventArgs e)
        {
            if (fbdOrigem.ShowDialog() == DialogResult.OK)
            {
                txtDestino.Text = fbdOrigem.SelectedPath;
            }

        }

        private bool Validar()
        {
            string origem = txtOrigem.Text;
            string destino = txtDestino.Text;
            string planilha = txtPlanilha.Text;

            if (String.IsNullOrWhiteSpace(origem) || String.IsNullOrWhiteSpace(destino) || String.IsNullOrWhiteSpace(planilha))
            {
                MessageBox.Show("Não deixe nenhum campo em branco!");
                return false;
            }
            return true;
        }

        private void btnGerar_Click(object sender, EventArgs e)
        {
            if (Validar())
            {
                string pathFrom = txtOrigem.Text;
                string pathTo = String.Format(@"{0}\teste.xlsx", txtDestino.Text);
                string sheetName = txtPlanilha.Text;
                DataTable dt = ReadExcel(pathFrom, sheetName);
                WriteExcel(dt, pathTo, sheetName);
            }
        }

        private void WriteExcel(DataTable table, string path, string sheetName)
        {
            string connectionString = String.Empty;
            try
            {
                connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0 Xml; HDR=YES\";";
                string fileName = Path.GetFileName(path);

                CriarExcel criar = new CriarExcel();
                CriarExcel.ExportToExcelOleDb(table, connectionString, fileName, true, sheetName);
                MessageBox.Show("Planilha Gerada com Sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problemas na Geração do Arquivo");
            }
        }


        private DataTable ReadExcel(string path, string sheetName)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection())
                {
                    DataTable dt = new DataTable();
                    conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'";
                    using (OleDbCommand comm = new OleDbCommand())
                    {
                        comm.CommandText = "Select * from [" + sheetName + "$]";
                        comm.Connection = conn;
                        conn.Open();
                        using (OleDbDataAdapter da = new OleDbDataAdapter())
                        {
                            da.SelectCommand = comm;
                            da.Fill(dt);
                            conn.Close();
                            return dt;
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problemas na Leitura do Arquivo");
                return new DataTable();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}

