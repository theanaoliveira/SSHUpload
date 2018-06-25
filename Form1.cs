using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SSHUpload
{
    public partial class Form1 : Form
    {
        static List<ConnectionSSH> ListConnections = new List<ConnectionSSH>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.txtFileName.Text = this.openFileDialog.FileName;
            }
        }

        private void btnCadastro_Click(object sender, EventArgs e)
        {
            if (txtHost.Text != "" &&
                txtPorta.Text != "" &&
                txtUser.Text != "" &&
                txtPassword.Text != "")
            {
                var loConnection = new ConnectionSSH()
                {
                    host = txtHost.Text,
                    port = int.Parse(txtPorta.Text),
                    user = txtUser.Text,
                    password = txtPassword.Text
                };

                ListConnections.Add(loConnection);
                listServidores.Items.Add(loConnection.host);
                listServidores.SelectedIndex = 0;

                txtHost.Text = "";
                txtPorta.Text = "22";
                txtUser.Text = "";
                txtPassword.Text = "";
            }
            else
            {
                MessageBox.Show("Preencha todos os campos");
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (this.txtFileName.Text != "" && txtPath.Text != "")
            {
                var file = new FileInfo(this.txtFileName.Text);
                var uploadfile = file.FullName;

                progressBar1.Maximum = ListConnections.Count;
                progressBar1.Value = 0;
                progressBar1.Step = 1;

                MessageBox.Show("Aguarde enquanto estamos enviado o arquivo! :)");

                if (!txtPath.Text.EndsWith("/"))
                    txtPath.Text += "/";

                for (var lintCont = 0; lintCont < ListConnections.Count; lintCont++)
                {
                    try
                    {
                        var client = new SftpClient(ListConnections[lintCont].host, ListConnections[lintCont].port, ListConnections[lintCont].user, ListConnections[lintCont].password);
                        client.Connect();

                        if (client.IsConnected)
                        {
                            var fileStream = new FileStream(uploadfile, FileMode.Open);

                            if (fileStream != null)
                            {
                                client.UploadFile(fileStream, $"{txtPath.Text}{file.Name}", null);
                                client.Disconnect();
                                client.Dispose();
                            }

                            fileStream.Dispose();
                        }
                    }
                    catch
                    {
                        MessageBox.Show($"Erro ao subir o arquivo no host: {ListConnections[lintCont].host}. Verifique as credenciais cadastradas e tente novamente.");
                    }

                    progressBar1.PerformStep();
                }

                MessageBox.Show("Arquivos enviados com sucesso.");
            }
            else
            {
                MessageBox.Show("Faça upload do arquivo");
            }
        }
    }
}
