using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace UptimeChecker
{
    public partial class MainForm : Form
    {
        private Timer timer;
        private int checkCount;
        private const bool CheckProductionStatus = true;
        private Status ProductionStatus;
        private Status DevelopmentStatus;

        public MainForm()
        {
            Program.KeepAwake();
            InitializeComponent();
            if (CheckProductionStatus)
                ProductionStatus = new Status("PRODUCTION:", "http://172.29.21.69:5000/");
            DevelopmentStatus = new Status("DEVELOPMENT:", "https://peeramid-dev/");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Check();
            timer = new Timer();
            timer.Interval = 1 * 60 * 1000;
            timer.Tick += (s, v) => Check();
            timer.Start();
        }

        private void Check()
        {
            checkCount++;

            if (DevelopmentStatus.IsUp && (!CheckProductionStatus || ProductionStatus.IsUp))
            {
                if ((checkCount % 5) != 0)
                    return;
            }

            ProductionStatus?.Check();
            DevelopmentStatus.Check();

            var now = DateTime.Now.ToLocalTime();
            //if ((now.Hour >= 6) && (now.Hour <= 21))
            Program.KeepAwake();

            BeginInvoke(new Action(ShowStatus));
        }

        private void ShowStatus()
        {
            var bothUp = DevelopmentStatus.IsUp && (!CheckProductionStatus || ProductionStatus.IsUp);

            notifyIcon1.Icon = bothUp ? Properties.Resources.Up : Properties.Resources.Down;
            if (CheckProductionStatus)
            {
                if (ProductionStatus.IsUp)
                {
                    notifyIcon1.Text = DevelopmentStatus.IsUp ? "Both servers are UP" : "The Development server is DOWN";
                }
                else
                {
                    notifyIcon1.Text = DevelopmentStatus.IsUp ? "The Production server is DOWN" : "Both servers are DOWN";
                }
            }
            else
            {
                notifyIcon1.Text = DevelopmentStatus.IsUp ? "The Development server is UP" : "The Development server is DOWN";
            }

            notifyIcon1.Visible = false;
            notifyIcon1.Visible = true;

            var b = new StringBuilder();
            if (CheckProductionStatus)
                b.AppendLine(ProductionStatus.ToString());
            b.Append(DevelopmentStatus.ToString());

            textBox1.Text = b.ToString();
            textBox1.BackColor = bothUp ? Color.LightGreen : Color.LightPink;
            textBox1.SelectionLength = 0;
        }

        private void ToggleVisibility()
        {
            ToggleVisibility(!this.Visible);
        }

        private void ToggleVisibility(bool newState)
        {
            this.Visible = newState;
            if (this.Visible)
                this.Activate();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ToggleVisibility();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleVisibility(true);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleVisibility(false);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //IntPtr hMenu = GetSystemMenu(this.Handle, false);
            //int menuItemCount = GetMenuItemCount(hMenu);
            //RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToggleVisibility(false);
        }

        private void OnTextBoxEnter(object sender, EventArgs e)
        {
            ShowStatus();

        }

        private void OnButtonEnter(object sender, EventArgs e)
        {
            ShowStatus();
        }
    }

    internal class Status
    {
        public readonly string Name;
        public bool IsUp;
        public Exception MostRecentException;
        public DateTime MostRecentCheckTime;
        public DateTime MostRecentExceptionTime;
        public readonly Uri Uri;

        public Status(string name, string url)
        {
            Name = name;
            Uri = new Uri(url);
        }

        public void Check()
        {
            try
            {
                MostRecentCheckTime = DateTime.Now;

                using (var wc = new WebClient())
                {
                    wc.UseDefaultCredentials = true;
                    var s = wc.DownloadString(Uri);
                    if (!s.Contains("navSectionLayout"))
                        throw new Exception("Bad page: " + s);
                    IsUp = true;
                }
            }
            catch (Exception ex)
            {
                IsUp = false;
                if (ex.Message != MostRecentException?.Message)
                {
                    MostRecentException = ex;
                    MostRecentExceptionTime = MostRecentCheckTime;
                }
            }
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            b.AppendLine(Name);

            if (IsUp)
            {
                b.AppendLine("\tThe server is UP.");
                b.AppendLine($"\tThe last check was at {MostRecentCheckTime}.");
                if (MostRecentExceptionTime != DateTime.MinValue)
                    b.AppendLine($"\tThe last time the server was down was {MostRecentExceptionTime}.").Append($"\tThe problem was: {MostRecentException.Message}");
            }
            else if (MostRecentExceptionTime == MostRecentCheckTime)
            {
                b.AppendLine($"The server is DOWN.");
                b.AppendLine($"\tThe last check was at {MostRecentCheckTime}.");
                b.Append($"\tThe problem is: {MostRecentException.Message}");
            }
            else
            {
                b.AppendLine($"\tThe server has been DOWN since {MostRecentExceptionTime}.");
                b.Append($"The problem is: {MostRecentException.Message}");
            }

            return b.ToString();
        }
    }
}