using System;
using System.Windows.Forms;

namespace SpotDifferenceGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var modeForm = new ModeSelectionForm();
            if (modeForm.ShowDialog() == DialogResult.OK)
            {
                if (modeForm.SelectedMode == ModeSelectionForm.GameMode.Attempts)
                {
                    // يبدأ مباشرة بنمط المحاولات بدون صعوبة
                    Application.Run(new Form1(ModeSelectionForm.GameMode.Attempts, DifficultySelectionForm.Difficulty.Easy));
                }
                else if (modeForm.SelectedMode == ModeSelectionForm.GameMode.Timer)
                {
                    // يفتح نافذة اختيار الصعوبة
                    var difficultyForm = new DifficultySelectionForm();
                    if (difficultyForm.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new Form1(ModeSelectionForm.GameMode.Timer, difficultyForm.SelectedDifficulty));
                    }
                }
            }
        }
    }
}
