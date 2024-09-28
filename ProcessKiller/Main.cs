using System.Diagnostics;

namespace ProcessKiller
{
    public partial class Main : Form
    {
        private ComboBox processComboBox;
        private Button killButton;
        private Label processInfoLabel;
        private Icon defaultIcon;

        public Main()
        {
            InitializeComponent();
            GenerateLabel();

            GenerateComboBox();
            GenerateKillButton();
        }
        #region ViewElements

        void GenerateComboBox()
        {
            processComboBox = new ComboBox();
            processComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            processComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            processComboBox.Dock = DockStyle.Top;

            processComboBox.DrawItem += ProcessComboBox_DrawItem;
            processComboBox.MeasureItem += ProcessComboBox_MeasureItem;

            defaultIcon = SystemIcons.Application;
            processComboBox.SelectedIndexChanged += ProcessComboBox_SelectedIndexChanged;

            Controls.Add(processComboBox);

        }

        void GenerateKillButton()
        {
            killButton = new Button();
            killButton.Text = "Kill";
            killButton.Dock = DockStyle.Bottom;
            killButton.Click += KillButton_Click;
            killButton.Height = 40;
            Controls.Add(killButton);
        }
        void GenerateLabel()
        {
            processInfoLabel = new Label();
            processInfoLabel.Dock = DockStyle.Top;
            processInfoLabel.Height = 60;  
            processInfoLabel.TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(processInfoLabel);
        }
        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception exception)
            {
                MessageBox.Show("HAHAHA I Am Jamal");
            }
        }

        void LoadData()
        {
            var processes = Process.GetProcesses().ToList();
            processComboBox.Items.AddRange(processes.OrderBy(p => p.ProcessName).ToArray());
        }

        private void ProcessComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Process process = (Process)processComboBox.Items[e.Index];

            Icon processIcon = null;
            try
            {
                string filePath = process.MainModule?.FileName;
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    processIcon = Icon.ExtractAssociatedIcon(filePath);
                }
            }
            catch
            {
                processIcon = defaultIcon;
            }

            e.DrawBackground();

            if (processIcon != null)
            {
                e.Graphics.DrawIcon(processIcon, new Rectangle(e.Bounds.Left, e.Bounds.Top, 16, 16));
            }
            string processName = process.ProcessName;
            e.Graphics.DrawString(processName, e.Font, Brushes.Black, e.Bounds.Left + 20, e.Bounds.Top);
            e.DrawFocusRectangle();
        }

        private void ProcessComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 18;
        }

        private void KillButton_Click(object sender, EventArgs e)
        {
            Process selectedProcess = (Process)processComboBox.SelectedItem;

            if (selectedProcess != null)
            {
                try
                {
                    selectedProcess.Kill();
                    MessageBox.Show($"Process '{selectedProcess.ProcessName}' has been killed.", "Process Killed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    processComboBox.Items.Clear();
                    LoadData();
                    processInfoLabel.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to kill process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a process to kill.", "No Process Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ProcessComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Process selectedProcess = (Process)processComboBox.SelectedItem;

            if (selectedProcess != null)
            {
                try
                {
                    string processInfo = $"ID: {selectedProcess.Id}\n" +
                                         $"Memory: {selectedProcess.PrivateMemorySize64 / 1024} KB\n" +
                                         $"Path: {selectedProcess.MainModule?.FileName}";
                    processInfoLabel.Text = processInfo;
                }
                catch (Exception ex)
                {
                    processInfoLabel.Text = "Unable to retrieve process details.";
                }
            }
            else
            {
                processInfoLabel.Text = "No process selected.";
            }
        }
    }
}
