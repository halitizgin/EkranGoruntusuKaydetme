using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace EkranGoruntusuKaydetme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);
        [DllImport("kernel32", SetLastError = true)]
        private static extern short GlobalAddAtom(string lpString);
        // 
        private const int MOD_ALT = 1;
        private const int MOD_CONTROL = 2;
        private const int MOD_SHIFT = 4;
        // 

        void RegisterGlobalHotKey(Keys hotkey, int modifiers)
        {
            short hotkeyID = GlobalAddAtom(base.Name);
            RegisterHotKey(base.Handle, hotkeyID, modifiers, (int)hotkey);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m); if (m.Msg == 0x312)
            {
                string sifre = Settings1.Default.sifre;
                string girilen = Microsoft.VisualBasic.Interaction.InputBox("Şifreyi giriniz:", "Şifre", "");
                if (girilen.Trim() == sifre.Trim())
                {
                    this.Show();
                }
                else
                {
                    MessageBox.Show("Şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
            else if (radioButton2.Checked == true)
            {
                this.Hide();
                notifyIcon1.Visible = false;
                MessageBox.Show("CTRL + F6 tuş birleşimi ile programı tekrar görüntüleyebilirsiniz.\nBu tuşları kullandıktan sonra şifreyi girmeden program açılmayacaktır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterGlobalHotKey(Keys.F6, 2);
            string sifre = Settings1.Default.sifre;
            string girilen = Microsoft.VisualBasic.Interaction.InputBox("Şifreyi giriniz:", "Şifre", "");
            if (girilen.Trim() == sifre.Trim())
            {
                textBox1.Text = Settings1.Default.kayit;
                textBox2.Text = Settings1.Default.aralik.ToString();
                comboBox1.Text = Settings1.Default.tur;
            }
            else
            {
                MessageBox.Show("Yanlış şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.ExitThread();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void gösterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void tamamenGizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sifre = Settings1.Default.sifre;
            string girilen = Microsoft.VisualBasic.Interaction.InputBox("Şifreyi giriniz:", "Şifre", "");
            if (girilen.Trim() == sifre.Trim())
            {
                Application.ExitThread();
            }
            else
            {
                MessageBox.Show("Şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            string sifre = Settings1.Default.sifre;
            string girilen = Microsoft.VisualBasic.Interaction.InputBox("Şifreyi giriniz:", "Şifre", "");
            if (girilen.Trim() == sifre.Trim())
            {
                this.Show();
            }
            else
            {
                MessageBox.Show("Şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Settings1.Default.kayit = textBox1.Text.Trim();
            Settings1.Default.aralik = Convert.ToInt32(textBox2.Text.Trim());
            Settings1.Default.tur = comboBox1.Text;
            Settings1.Default.Save();
            MessageBox.Show("Değişikler kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        bool gBasla = false;

        private void button5_Click(object sender, EventArgs e)
        {
            if (gBasla == false)
            {
                string tur = Settings1.Default.tur;
                if (tur == "Saniye")
                {
                    timer1.Interval = Settings1.Default.aralik * 1000;
                }
                else if (tur == "Dakika")
                {
                    timer1.Interval = Settings1.Default.aralik * 60000;
                }
                else if (tur == "Saat")
                {
                    timer1.Interval = Settings1.Default.aralik * 360000;
                }
                timer1.Start();
                button5.Text = "Durdur";
                gBasla = true;
            }
            else
            {
                timer1.Stop();
                button5.Text = "Başlat";
                gBasla = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private Bitmap Screenshot() // Bitmap türünde olşuturuyoruz  fonksiyonumuzu. 
        {
            Bitmap Screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics GFX = Graphics.FromImage(Screenshot);
            GFX.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            return Screenshot;
        }

        private string temizle(string tarih)
        {
            tarih = tarih.Replace(" ", "");
            tarih = tarih.Replace(".", "");
            tarih = tarih.Replace(":", "");
            return tarih;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string tarih = DateTime.Now.ToString();
            tarih = temizle(tarih);
            Screenshot();
            Screenshot().Save(Settings1.Default.kayit + "/" + tarih + ".png", ImageFormat.Png);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult yapilan = folderBrowserDialog1.ShowDialog();
            if (yapilan == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void KlasorAc(string dosyaYolu)
        {
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = dosyaYolu;
            prc.Start();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            KlasorAc(Settings1.Default.kayit);
        }
    }
}
