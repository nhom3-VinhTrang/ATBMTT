using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TripleDESEncryptionApp
{
    public class DecryptFileForm : Form
    {
        public DecryptFileForm()
        {
            this.Text = "Giải mã file";
            this.Width = 400;
            this.Height = 200;
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnSelectFile = new Button { Text = "Chọn file mã hóa", Top = 20, Left = 20, Width = 150 };
            var txtKey = new TextBox { Top = 60, Left = 20, Width = 340, PlaceholderText = "Khóa bí mật" };
            var btnDecrypt = new Button { Text = "Giải mã và lưu", Top = 100, Height = 45, Left = 20, Width = 150 };
            var btnBack = new Button { Text = "Trở về", Top = 100, Left = 180, Height = 45, Width = 100 };

            this.Controls.Add(btnSelectFile);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(btnBack);

            string encryptedFilePath = "";

            btnSelectFile.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Encrypted File|*.enc" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    encryptedFilePath = ofd.FileName;
                    MessageBox.Show("Đã chọn file mã hóa");
                }
            };

            btnDecrypt.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(encryptedFilePath) || string.IsNullOrWhiteSpace(txtKey.Text))
                {
                    MessageBox.Show("Chọn file và nhập khóa.");
                    return;
                }

                try
                {
                    byte[] data = File.ReadAllBytes(encryptedFilePath);
                    byte[] decrypted = DecryptFile(data, txtKey.Text);

                    using var sfd = new SaveFileDialog { Filter = "All Files|*.*" };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, decrypted);
                        MessageBox.Show("Đã giải mã và lưu file.");
                    }
                }
                catch
                {
                    MessageBox.Show("Giải mã thất bại. Kiểm tra khóa hoặc file.");
                }
            };

            btnBack.Click += (s, e) => { this.Hide(); new MainMenu().Show(); };
        }

        public static byte[] DecryptFile(byte[] encryptedData, string key)
        {
            using var tdes = TripleDES.Create();
            tdes.Key = DecryptForm.GenerateSecureKey(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            return tdes.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}
