// ModeSelectionForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpotDifferenceGame
{
    public partial class ModeSelectionForm : Form
    {
        public enum GameMode { Timer, Attempts }
        public GameMode SelectedMode;

        public ModeSelectionForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "اختر نمط اللعب";
            this.Size = new Size(350, 230);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label();
            lblTitle.Text = "اختر نمط اللعب:";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(300, 30);
            lblTitle.Location = new Point(25, 20);

            Button btnTimer = new Button();
            btnTimer.Text = "⏱️ نمط المؤقت الزمني";
            btnTimer.Size = new Size(300, 50);
            btnTimer.Location = new Point(25, 60);
            btnTimer.Click += (s, e) =>
            {
                SelectedMode = GameMode.Timer;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            Button btnAttempts = new Button();
            btnAttempts.Text = "🎯 نمط المحاولات المحددة";
            btnAttempts.Size = new Size(300, 50);
            btnAttempts.Location = new Point(25, 120);
            btnAttempts.Click += (s, e) =>
            {
                SelectedMode = GameMode.Attempts;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnTimer);
            this.Controls.Add(btnAttempts);
        }
    }
}
