using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace SpotDifferenceGame
{
    public partial class ModeSelectionForm : Form
    {
        public enum GameMode { Timer, Attempts }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GameMode SelectedMode { get; private set; }

        public ModeSelectionForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            this.Text = "اختر نمط اللعب";
            this.Size = new System.Drawing.Size(350, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitle = new Label();
            lblTitle.Text = "اختر نمط اللعب المفضل";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(300, 30);
            lblTitle.Location = new Point(25, 20);

            Button btnTimer = new Button();
            btnTimer.Text = "⏱️ نمط المؤقت الزمني (60 ثانية)";
            btnTimer.Size = new Size(300, 50);
            btnTimer.Location = new Point(25, 70);
            btnTimer.Font = new Font("Arial", 12);
            btnTimer.Click += (s, e) => SetModeAndClose(GameMode.Timer);

            Button btnAttempts = new Button();
            btnAttempts.Text = "🎯 نمط المحاولات المحددة (5 محاولات)";
            btnAttempts.Size = new Size(300, 50);
            btnAttempts.Location = new Point(25, 130);
            btnAttempts.Font = new Font("Arial", 12);
            btnAttempts.Click += (s, e) => SetModeAndClose(GameMode.Attempts);

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnTimer);
            this.Controls.Add(btnAttempts);
        }

        private void SetModeAndClose(GameMode mode)
        {
            this.SelectedMode = mode;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}