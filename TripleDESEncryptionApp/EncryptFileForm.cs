using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TripleDESEncryptionApp
{
    public class EncryptFileForm : Form
    {
        public EncryptFileForm()
        {
            this.Text = "Mã hóa file";
            this.Width = 400;
            this.Height = 200;
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnSelectFile = new Button { Text = "Chọn file", Top = 20, Left = 20, Width = 100 };
            var txtKey = new TextBox { Top = 60, Left = 20, Width = 340, PlaceholderText = "Khóa bí mật" };
            var btnEncrypt = new Button { Text = "Mã hóa và lưu", Top = 100, Height = 45, Left = 20, Width = 150 };
            var btnBack = new Button { Text = "Trở về", Top = 100, Height = 45, Left = 180, Width = 100 };

            this.Controls.Add(btnSelectFile);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnEncrypt);
            this.Controls.Add(btnBack);

            string filePath = "";

            btnSelectFile.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filePath = ofd.FileName;
                    MessageBox.Show("Đã chọn: " + Path.GetFileName(filePath));
                }
            };

            btnEncrypt.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(txtKey.Text))
                {
                    MessageBox.Show("Vui lòng chọn file và nhập khóa.");
                    return;
                }

                byte[] fileData = File.ReadAllBytes(filePath);
                byte[] encrypted = EncryptFile(fileData, txtKey.Text);

                using var sfd = new SaveFileDialog { Filter = "Encrypted File|*.enc" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, encrypted);
                    MessageBox.Show("Đã mã hóa file.");
                }
            };

            btnBack.Click += (s, e) => { this.Hide(); new MainMenu().Show(); };
        }

        public static byte[] EncryptFile(byte[] data, string key)
        {
            using var tdes = TripleDES.Create();
            tdes.Key = DecryptForm.GenerateSecureKey(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            return tdes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
        }
    }
}
