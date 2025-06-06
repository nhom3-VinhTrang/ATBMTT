using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace TripleDESEncryptionApp
{
    public partial class DecryptForm : Form
    {
        public DecryptForm()
        {
            this.Text = "Giải mã ảnh";
            this.Width = 600;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnSelectFile = new Button { Text = "Chọn file mã hóa", Top = 10, Left = 10, Width = 150 };
            var txtKey = new TextBox { Top = 50, Left = 10, Width = 400, PlaceholderText = "Khóa bí mật" };
            var btnDecrypt = new Button { Text = "Giải mã", Top = 80, Height = 45, Left = 10, Width = 100 };
            var btnSaveImage = new Button { Text = "Tải ảnh xuống", Top = 80, Height = 45, Left = 120, Width = 150 };

            var txtMessage = new TextBox
            {
                Top = 120,
                Left = 10,
                Width = 550,
                Height = 60,
                Multiline = true,
                ReadOnly = true
            };

            var pictureBox = new PictureBox
            {
                Top = 10,
                Left = 10,
                Width = 480,
                Height = 320,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            var panel = new Panel
            {
                Top = 190,
                Left = 10,
                Width = 550,
                Height = 350,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(pictureBox);

            this.Controls.Add(btnSelectFile);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(btnSaveImage);
            this.Controls.Add(txtMessage);
            this.Controls.Add(panel);

            var btnBack = new Button { Text = "Trở về", Top = 560, Height = 45, Left = 10, Width = 100 };
            btnBack.Click += (s, e) =>
            {
                this.Hide();
                new MainMenu().Show();
            };
            this.Controls.Add(btnBack);

            string encryptedText = "";

            btnSelectFile.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Text File|*.txt" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    encryptedText = File.ReadAllText(ofd.FileName);
                    MessageBox.Show("Đã nạp nội dung mã hóa");
                }
            };

            btnDecrypt.Click += (s, e) =>
            {
                string key = txtKey.Text;
                if (string.IsNullOrWhiteSpace(encryptedText) || string.IsNullOrWhiteSpace(key))
                {
                    MessageBox.Show("Chọn file và nhập khóa");
                    return;
                }

                try
                {
                    var decryptedJson = DecryptTripleDES(encryptedText, key);
                    var doc = JsonDocument.Parse(decryptedJson);
                    string message = doc.RootElement.GetProperty("message").GetString();
                    string imageBase64 = doc.RootElement.GetProperty("image").GetString();

                    txtMessage.Text = message;

                    var base64Data = imageBase64.Substring(imageBase64.IndexOf(",") + 1);
                    var imageBytes = Convert.FromBase64String(base64Data);
                    using var ms = new MemoryStream(imageBytes);
                    var originalImg = Image.FromStream(ms);
                    var resized = new Bitmap(originalImg, new Size(480, 320));
                    pictureBox.Image = resized;
                }
                catch
                {
                    MessageBox.Show("Giải mã thất bại. Kiểm tra khóa hoặc nội dung mã hóa");
                }
            };

            btnSaveImage.Click += (s, e) =>
            {
                if (pictureBox.Image == null)
                {
                    MessageBox.Show("Không có ảnh để lưu.");
                    return;
                }

                using var sfd = new SaveFileDialog { Filter = "Ảnh JPEG|*.jpg" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    MessageBox.Show("Đã lưu ảnh.");
                }
            };
        }

        public static string DecryptTripleDES(string encryptedText, string key)
        {
            using var tdes = TripleDES.Create();
            tdes.Key = GenerateSecureKey(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            var data = Convert.FromBase64String(encryptedText);
            var result = tdes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);
        }

        public static byte[] GenerateSecureKey(string inputKey)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputKey));
            byte[] key = new byte[24];
            Array.Copy(hash, key, 24);

            while (TripleDES.IsWeakKey(key))
            {
                key[0] ^= 0x01;
            }

            return key;
        }
    }
}
