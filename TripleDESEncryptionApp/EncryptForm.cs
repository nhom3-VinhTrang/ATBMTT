
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace TripleDESEncryptionApp
{
    public partial class EncryptForm : Form
    {
        private string imageBase64 = "";

        public EncryptForm()
        {
            this.Text = "Mã hóa ảnh";
            this.Width = 520;
            this.Height = 530;
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnSelect = new Button { Text = "Chọn ảnh", Width = 100, Top = 10, Left = 10 };
            var pictureBox = new PictureBox
            {
                Top = 10,
                Left = 10,
                Width = 480,
                Height = 320,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            var txtMessage = new TextBox { Top = 360, Left = 10, Width = 400, PlaceholderText = "Tin nhắn" };
            var txtKey = new TextBox
            {
                Top = 390,
                Left = 10,
                Width = 400,
                PlaceholderText = "Khóa bí mật",
                ScrollBars = ScrollBars.Horizontal,
                WordWrap = false
            };
            var btnEncrypt = new Button { Text = "Mã hóa và lưu", Top = 420, Height = 45, Left = 10, Width = 150 };

            this.Controls.Add(btnSelect);
            this.Controls.Add(pictureBox);
            this.Controls.Add(txtMessage);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnEncrypt);

            var btnBack = new Button { Text = "Trở về", Top = 420, Height = 45, Left = 180, Width = 100 };
            btnBack.Click += (s, e) =>
            {
                this.Hide();
                new MainMenu().Show();
            };
            this.Controls.Add(btnBack);

            btnSelect.Click += (s, e) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Ảnh|*.jpg;*.png" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var image = Image.FromFile(ofd.FileName);
                    pictureBox.Image = image;
                    using var ms = new MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageBase64 = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
                }
            };

            btnEncrypt.Click += (s, e) =>
            {
                string message = txtMessage.Text;
                string key = txtKey.Text;
                if (string.IsNullOrWhiteSpace(imageBase64) || string.IsNullOrWhiteSpace(key))
                {
                    MessageBox.Show("Hãy chọn ảnh và nhập khóa");
                    return;
                }
                var payload = JsonSerializer.Serialize(new { image = imageBase64, message });
                string encrypted = EncryptTripleDES(payload, key);

                using var sfd = new SaveFileDialog { Filter = "Text File|*.txt" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, encrypted);
                    MessageBox.Show("Đã lưu file mã hóa.");
                }
            };
        }

        public static string EncryptTripleDES(string plainText, string key)
        {
            using var tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = DecryptForm.GenerateSecureKey(key);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            var data = Encoding.UTF8.GetBytes(plainText);
            var result = tdes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(result);
        }
    }
}
