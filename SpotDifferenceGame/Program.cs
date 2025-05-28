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

            // فتح واجهة اختيار النمط
            var modeForm = new ModeSelectionForm();
            if (modeForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new Form1(modeForm.SelectedMode));
            }
        }
    }
}
