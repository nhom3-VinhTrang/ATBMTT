using System;
using System.Drawing;
using System.Windows.Forms;

namespace TripleDESEncryptionApp
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            this.Text = "Triple DES App";
            this.Width = 300;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnEncrypt = new Button { Text = "Mã hóa ảnh", Width = 120, Height = 45, Top = 20, Left = 80, Font = new Font("Segoe UI", 10), TextAlign = ContentAlignment.MiddleCenter };
            var btnDecrypt = new Button { Text = "Giải mã ảnh", Width = 120, Height = 45, Top = 70, Left = 80, Font = new Font("Segoe UI", 10), TextAlign = ContentAlignment.MiddleCenter };
            var btnEncryptFile = new Button { Text = "Mã hóa file", Width = 120, Height = 45, Top = 120, Left = 80, Font = new Font("Segoe UI", 10), TextAlign = ContentAlignment.MiddleCenter };
            var btnDecryptFile = new Button { Text = "Giải mã file", Width = 120, Height = 45, Top = 170, Left = 80, Font = new Font("Segoe UI", 10), TextAlign = ContentAlignment.MiddleCenter };

            btnEncrypt.Click += (s, e) => { this.Hide(); new EncryptForm().Show(); };
            btnDecrypt.Click += (s, e) => { this.Hide(); new DecryptForm().Show(); };
            btnEncryptFile.Click += (s, e) => { this.Hide(); new EncryptFileForm().Show(); };
            btnDecryptFile.Click += (s, e) => { this.Hide(); new DecryptFileForm().Show(); };

            this.Controls.Add(btnEncrypt);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(btnEncryptFile);
            this.Controls.Add(btnDecryptFile);
        }
    }
}
