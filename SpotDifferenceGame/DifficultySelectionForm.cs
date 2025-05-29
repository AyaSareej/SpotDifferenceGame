using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpotDifferenceGame
{
    public partial class DifficultySelectionForm : Form
    {
        public enum Difficulty { Easy, Medium, Hard }
        public Difficulty SelectedDifficulty = Difficulty.Easy;

        public DifficultySelectionForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "اختر مستوى الصعوبة";
            this.Size = new Size(300, 270);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label();
            lblTitle.Text = "اختر مستوى الصعوبة";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(280, 30);
            lblTitle.Location = new Point(10, 20);

            Button btnEasy = new Button();
            btnEasy.Text = "🟢 سهل (60 ثانية)";
            btnEasy.Size = new Size(240, 40);
            btnEasy.Location = new Point(30, 70);
            btnEasy.Click += (s, e) => { SelectedDifficulty = Difficulty.Easy; this.DialogResult = DialogResult.OK; this.Close(); };

            Button btnMedium = new Button();
            btnMedium.Text = "🟡 متوسط (45 ثانية)";
            btnMedium.Size = new Size(240, 40);
            btnMedium.Location = new Point(30, 120);
            btnMedium.Click += (s, e) => { SelectedDifficulty = Difficulty.Medium; this.DialogResult = DialogResult.OK; this.Close(); };

            Button btnHard = new Button();
            btnHard.Text = "🔴 صعب (30 ثانية)";
            btnHard.Size = new Size(240, 40);
            btnHard.Location = new Point(30, 170);
            btnHard.Click += (s, e) => { SelectedDifficulty = Difficulty.Hard; this.DialogResult = DialogResult.OK; this.Close(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnEasy);
            this.Controls.Add(btnMedium);
            this.Controls.Add(btnHard);
        }
    }
}