using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.Diagnostics;

namespace SpotDifferenceGame
{
    public partial class Form1 : Form
    {
       private PictureBox pictureBoxLeft;
        private PictureBox pictureBoxRight;
        private Label labelStatus;
        private Label labelTimer;
        private System.Windows.Forms.Timer gameTimer;

        private ModeSelectionForm.GameMode gameMode;
        private DifficultySelectionForm.Difficulty difficulty;
        private int remainingAttempts = 5;
        private int timeLeft = 60;
        private bool gameOver = false;

        private List<Rectangle> differences = new List<Rectangle>();
        private HashSet<int> discoveredDifferences = new HashSet<int>();

        public Form1(ModeSelectionForm.GameMode mode, DifficultySelectionForm.Difficulty diff)
        {
            gameMode = mode;
            difficulty = diff;
            InitializeComponent();
            this.Load += (sender, e) => {
                SetDifficultyTime();
                LoadImages();
                DrawAllDifferenceMarkers();
                StartGameTimer();
            };
            pictureBoxRight.MouseClick += PictureBoxRight_MouseClick;
        }

        private void SetDifficultyTime()
        {
            switch (difficulty)
            {
                case DifficultySelectionForm.Difficulty.Easy:
                    timeLeft = 60;
                    break;
                case DifficultySelectionForm.Difficulty.Medium:
                    timeLeft = 45;
                    break;
                case DifficultySelectionForm.Difficulty.Hard:
                    timeLeft = 30;
                    break;
            }
        }


private void LoadImages()
{
    try
    {
        string leftImagePath = "Assets/image_left.jpg";
        string rightImagePath = "Assets/image_right.jpg";

        if (!File.Exists(leftImagePath))
            throw new FileNotFoundException("ملف الصورة اليسرى غير موجود");
        if (!File.Exists(rightImagePath))
            throw new FileNotFoundException("ملف الصورة اليمنى غير موجود");

        // تحميل الصور كصور خام بدون ضبط حجم
        Bitmap leftImage = (Bitmap)Image.FromFile(leftImagePath);
        Bitmap rightImage = (Bitmap)Image.FromFile(rightImagePath);

        // تعديل الأبعاد لتكون متطابقة (بناءً على الأصغر)
        int minWidth = Math.Min(leftImage.Width, rightImage.Width);
        int minHeight = Math.Min(leftImage.Height, rightImage.Height);

        // إنشاء نسخ معدلة الحجم
        Bitmap resizedLeft = new Bitmap(leftImage, minWidth, minHeight);
        Bitmap resizedRight = new Bitmap(rightImage, minWidth, minHeight);

        // تعيين الصور المعدلة
        pictureBoxLeft.Image = resizedLeft;
        pictureBoxRight.Image = resizedRight;

        // تحرير الصور الأصلية
        leftImage.Dispose();
        rightImage.Dispose();

        // اكتشاف الفروقات
        differences = DetectDifferences(resizedLeft, resizedRight, blockSize: 15, threshold: 50);

        if (differences.Count == 0)
        {
            MessageBox.Show("لم يتم العثور على فروقات بين الصورتين", "تحذير", 
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        DrawAllDifferenceMarkers();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"حدث خطأ في تحميل الصور: {ex.Message}", "خطأ", 
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
        pictureBoxLeft.Image = null;
        pictureBoxRight.Image = null;
    }
}

private Bitmap ResizeToMatchDimensions(Image sourceImage, Image targetImage)
{
    // تحديد الأبعاد الأصغر
    int newWidth = Math.Min(sourceImage.Width, targetImage.Width);
    int newHeight = Math.Min(sourceImage.Height, targetImage.Height);

    // إنشاء صورة جديدة بالأبعاد المطلوبة
    Bitmap result = new Bitmap(newWidth, newHeight);

    using (Graphics g = Graphics.FromImage(result))
    {
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
    }

    return result;
}
        private void InitializeComponent()
        {
            this.pictureBoxLeft = new PictureBox();
            this.pictureBoxRight = new PictureBox();
            this.labelStatus = new Label();
            this.labelTimer = new Label();
            this.gameTimer = new System.Windows.Forms.Timer();

            Label labelTitle = new Label();
            Button btnRestart = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.SuspendLayout();

            // labelTitle
            labelTitle.Text = "Spot the Difference Game";
            labelTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            labelTitle.Size = new Size(880, 40);
            labelTitle.Location = new Point(0, 0);

            // pictureBoxLeft
            this.pictureBoxLeft.Location = new Point(20, 60);
            this.pictureBoxLeft.Size = new Size(400, 400);
            this.pictureBoxLeft.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxLeft.BorderStyle = BorderStyle.FixedSingle;

            // pictureBoxRight
            this.pictureBoxRight.Location = new Point(440, 60);
            this.pictureBoxRight.Size = new Size(400, 400);
            this.pictureBoxRight.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxRight.BorderStyle = BorderStyle.FixedSingle;

            // labelStatus
            this.labelStatus.Location = new Point(0, 520);
            this.labelStatus.Size = new Size(880, 35);
            this.labelStatus.TextAlign = ContentAlignment.MiddleCenter;
            this.labelStatus.Font = new Font("Arial", 12, FontStyle.Bold);
            this.labelStatus.BackColor = Color.LightGray;
            this.labelStatus.BorderStyle = BorderStyle.FixedSingle;

            // labelTimer
            this.labelTimer.Location = new Point(370, 470);
            this.labelTimer.Size = new Size(140, 35);
            this.labelTimer.TextAlign = ContentAlignment.MiddleCenter;
            this.labelTimer.Font = new Font("Arial", 12, FontStyle.Bold);
            this.labelTimer.BackColor = Color.LightYellow;
            this.labelTimer.BorderStyle = BorderStyle.FixedSingle;
            this.labelTimer.Text = gameMode == ModeSelectionForm.GameMode.Timer ? 
                $"⏱️ الوقت: {timeLeft} ثانية" : $"🎯 المحاولات: {remainingAttempts}";

            // btnRestart
            btnRestart.Text = "إعادة التشغيل";
            btnRestart.Size = new Size(120, 35);
            btnRestart.Location = new Point(20, 470);
            btnRestart.Click += BtnRestart_Click;

            // Form
            this.ClientSize = new Size(880, 570);
            this.Controls.Add(labelTitle);
            this.Controls.Add(this.pictureBoxLeft);
            this.Controls.Add(this.pictureBoxRight);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(btnRestart);
            this.Text = "Spot the Difference Game";

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.ResumeLayout(false);
        }

        private void StartGameTimer()
        {
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            if (gameMode == ModeSelectionForm.GameMode.Timer)
                gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                labelTimer.Text = $"⏱️ الوقت: {timeLeft} ثانية";
                
                if (timeLeft <= 10)
                {
                    labelTimer.BackColor = Color.Orange;
                    labelTimer.ForeColor = Color.White;
                }
            }
            else
            {
                EndGame("⏰ انتهى الوقت!");
            }
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            timeLeft = 60;
            remainingAttempts = 5;
            gameOver = false;
            discoveredDifferences.Clear();
            
            LoadImages();
            DrawAllDifferenceMarkers();
            
            labelTimer.Text = gameMode == ModeSelectionForm.GameMode.Timer ? 
                $"⏱️ الوقت: {timeLeft} ثانية" : $"🎯 المحاولات: {remainingAttempts}";
            labelStatus.BackColor = Color.LightGray;
            labelTimer.BackColor = Color.LightYellow;
            labelTimer.ForeColor = Color.Black;
            pictureBoxRight.Enabled = true;
            
            if (gameMode == ModeSelectionForm.GameMode.Timer)
                gameTimer.Start();
        }

        private void PictureBoxRight_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameOver || pictureBoxRight.Image == null) return;

            Point clickPos = ConvertToImageCoordinates(e.Location);
            bool found = CheckForDifference(clickPos);

            if (!found && gameMode == ModeSelectionForm.GameMode.Attempts)
            {
                remainingAttempts--;
                labelTimer.Text = $"🎯 المحاولات: {remainingAttempts}";

                if (remainingAttempts <= 0)
                {
                    EndGame("❌ انتهت المحاولات!");
                }
            }

//             Debug.WriteLine($"تم اكتشاف {differences.Count} فروقات");
// foreach (var diff in differences)
// {
//     Debug.WriteLine($"الفرق: X={diff.X}, Y={diff.Y}, Width={diff.Width}, Height={diff.Height}");
// }
        }
private Point ConvertToImageCoordinates(Point clickPoint)
{
    if (pictureBoxRight.Image == null || pictureBoxRight.Width == 0 || pictureBoxRight.Height == 0)
        return Point.Empty;

    float scaleX = (float)pictureBoxRight.Image.Width / pictureBoxRight.Width;
    float scaleY = (float)pictureBoxRight.Image.Height / pictureBoxRight.Height;
    
    int x = (int)(clickPoint.X * scaleX);
    int y = (int)(clickPoint.Y * scaleY);
    
    // تأكد من أن الإحداثيات ضمن نطاق الصورة
    x = Math.Clamp(x, 0, pictureBoxRight.Image.Width - 1);
    y = Math.Clamp(y, 0, pictureBoxRight.Image.Height - 1);
    
    return new Point(x, y);
}

        private bool CheckForDifference(Point clickPos)
        {
            const int tolerance = 10;
            
            for (int i = 0; i < differences.Count; i++)
            {
                Rectangle expanded = ExpandRectangle(differences[i], tolerance);
                if (expanded.Contains(clickPos))
                {
                    if (!discoveredDifferences.Contains(i))
                    {
                        discoveredDifferences.Add(i);
                        PlayCorrectSound();
                        DrawMarkerOnFound(differences[i], Color.Red);
                        UpdateStatusLabel();
                        return true;
                    }
                    MessageBox.Show("💡 لقد اكتشفت هذا الفرق سابقًا.", "معلومة");
                    return true;
                }
            }
            
            PlayWrongSound();
            MessageBox.Show("❌ هذا ليس فرقًا", "حاول مرة أخرى");
            return false;
        }

        private void EndGame(string message)
        {
            gameTimer.Stop();
            gameOver = true;
            pictureBoxRight.Enabled = false;
            labelStatus.Text = message;
            labelStatus.BackColor = Color.IndianRed;
            MessageBox.Show(message, "انتهت اللعبة");
        }

        private void UpdateStatusLabel()
        {
            int remaining = differences.Count - discoveredDifferences.Count;
            labelStatus.Text = $"✅ مكتشفة: {discoveredDifferences.Count} / ❗ المتبقية: {remaining}";

            if (remaining == 0)
            {
                gameTimer.Stop();
                gameOver = true;
                PlayWinSound();
                labelStatus.BackColor = Color.LightGreen;
                labelStatus.Text = "🎉 مبروك! لقد وجدت كل الفروقات!";
                MessageBox.Show("🎉 أحسنت! أنهيت اللعبة بنجاح!", "مبروك");
            }
        }

        private Rectangle ExpandRectangle(Rectangle rect, int padding)
        {
            return new Rectangle(
                rect.X - padding,
                rect.Y - padding,
                rect.Width + 2 * padding,
                rect.Height + 2 * padding);
        }

        private void DrawMarkerOnFound(Rectangle rect, Color color)
        {
            if (pictureBoxRight.Image == null) return;

            Bitmap bmp = new Bitmap(pictureBoxRight.Image);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawEllipse(new Pen(color, 3), rect);
            }
            pictureBoxRight.Image = bmp;
        }

  private void DrawAllDifferenceMarkers()
{
    if (pictureBoxRight.Image == null) return;

    Bitmap bmp = new Bitmap(pictureBoxRight.Image);
    var pen = new Pen(Color.Blue, 2);

    // فلترة المستطيلات لضمان صحتها
    var validRects = differences.Where(r => 
        r.Width > 0 && r.Height > 0 &&
        r.X >= 0 && r.Y >= 0 &&
        r.Right <= bmp.Width && r.Bottom <= bmp.Height).ToList();

    using (Graphics g = Graphics.FromImage(bmp))
    {
        foreach (var rect in validRects)
        {
            // رسم كل مستطيل على حدة مع التحقق مرة أخرى
            if (rect.X >= 0 && rect.Y >= 0 && 
                rect.Right <= bmp.Width && rect.Bottom <= bmp.Height)
            {
                g.DrawRectangle(pen, rect);
            }
        }
    }

    pictureBoxRight.Image = bmp;
    UpdateStatusLabel();
}
private void PlayCorrectSound() => PlaySound("Assets/magical-hit.wav");
        private void PlayWrongSound() => PlaySound("Assets/fail.wav");
        private void PlayWinSound() => PlaySound("Assets/you-win.wav");

        private void PlaySound(string path)
        {
            try
            {
                new SoundPlayer(path).Play();
            }
            catch { /* تجاهل الأخطاء الصوتية */ }
        }
   
private List<Rectangle> DetectDifferences(Bitmap img1, Bitmap img2, int blockSize = 10, int threshold = 30)
{
    var foundDifferences = new List<Rectangle>();
    
    if (img1 == null || img2 == null) 
        return foundDifferences;

    blockSize = Math.Clamp(blockSize, 5, 20);
    
    // مسح الصور من الشفافية إذا كانت موجودة
    img1 = RemoveTransparency(img1);
    img2 = RemoveTransparency(img2);

    bool[,] visited = new bool[img1.Width, img1.Height];

    for (int y = 0; y < img1.Height - blockSize; y += blockSize/2)
    {
        for (int x = 0; x < img1.Width - blockSize; x += blockSize/2)
        {
            if (visited[x, y]) continue;

            if (IsBlockDifferent(img1, img2, x, y, blockSize, threshold))
            {
                var area = FloodFillDifference(img1, img2, visited, x, y, blockSize, threshold);
                if (area.Width > 0 && area.Height > 0)
                {
                    foundDifferences.Add(area);
                }
            }
        }
    }

    return foundDifferences;
}

private Bitmap RemoveTransparency(Bitmap img)
{
    Bitmap newImg = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    using (Graphics g = Graphics.FromImage(newImg))
    {
        g.Clear(Color.White);
        g.DrawImage(img, 0, 0, img.Width, img.Height);
    }
    return newImg;
}

private bool IsValidRectangle(Rectangle rect, int maxWidth, int maxHeight)
{
    return rect.Width > 0 && rect.Height > 0 &&
           rect.X >= 0 && rect.Y >= 0 &&
           rect.Right <= maxWidth && rect.Bottom <= maxHeight;
}
private Rectangle FloodFillDifference(Bitmap img1, Bitmap img2, bool[,] visited, 
                                   int startX, int startY, int blockSize, int threshold)
{
    int minX = startX, minY = startY, maxX = startX, maxY = startY;
    var queue = new Queue<Point>();
    queue.Enqueue(new Point(startX, startY));

    while (queue.Count > 0)
    {
        var p = queue.Dequeue();
        if (p.X < 0 || p.Y < 0 || p.X >= img1.Width || p.Y >= img1.Height || visited[p.X, p.Y])
            continue;

        visited[p.X, p.Y] = true;

        if (IsPixelDifferent(img1, img2, p.X, p.Y, threshold))
        {
            minX = Math.Min(minX, p.X);
            minY = Math.Min(minY, p.Y);
            maxX = Math.Max(maxX, p.X);
            maxY = Math.Max(maxY, p.Y);

            // إضافة الجيران
            queue.Enqueue(new Point(p.X + 1, p.Y));
            queue.Enqueue(new Point(p.X - 1, p.Y));
            queue.Enqueue(new Point(p.X, p.Y + 1));
            queue.Enqueue(new Point(p.X, p.Y - 1));
        }
    }

    return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
}

private bool IsPixelDifferent(Bitmap img1, Bitmap img2, int x, int y, int threshold)
{
    Color c1 = img1.GetPixel(x, y);
    Color c2 = img2.GetPixel(x, y);
    int delta = Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
    return delta > threshold;
}

private void AddNeighbor(Queue<Point> queue, int x, int y, int maxWidth, int maxHeight)
{
    if (x >= 0 && y >= 0 && x < maxWidth && y < maxHeight)
    {
        queue.Enqueue(new Point(x, y));
    }
}
   private bool IsBlockDifferent(Bitmap img1, Bitmap img2, int startX, int startY, int blockSize, int threshold)
{
    int diffCount = 0;
    int totalPixels = 0;
    int totalDifference = 0;

    for (int y = 0; y < blockSize; y++)
    {
        int py = startY + y;
        if (py >= img1.Height) continue;

        for (int x = 0; x < blockSize; x++)
        {
            int px = startX + x;
            if (px >= img1.Width) continue;

            totalPixels++;
            Color c1 = img1.GetPixel(px, py);
            Color c2 = img2.GetPixel(px, py);

            // حساب الفرق مع مراعاة جميع قنوات الألوان
            int delta = Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
            totalDifference += delta;

            if (delta > threshold)
                diffCount++;
        }
    }

    // شرطان للكشف عن الفرق:
    // 1. أن يكون عدد البكسلات المختلفة أكثر من 25%
    // 2. أن يكون متوسط الفرق في القنوات اللونية أكثر من العتبة
    return totalPixels > 0 && 
          (diffCount > (totalPixels / 4) || 
          (totalDifference / totalPixels) > (threshold / 2));
}
    }
}