namespace AddUser;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        label1 = new Label();
        _comboBoxServer = new ComboBox();
        label2 = new Label();
        _textBoxUsers = new TextBox();
        textBox2 = new TextBox();
        _buttonClose = new Button();
        _buttonOK = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 24);
        label1.Name = "label1";
        label1.Size = new Size(97, 15);
        label1.TabIndex = 0;
        label1.Text = "PeerAMid Server:";
        // 
        // _comboBoxServer
        // 
        _comboBoxServer.DropDownStyle = ComboBoxStyle.DropDownList;
        _comboBoxServer.FormattingEnabled = true;
        _comboBoxServer.Location = new Point(136, 21);
        _comboBoxServer.Name = "_comboBoxServer";
        _comboBoxServer.Size = new Size(188, 23);
        _comboBoxServer.TabIndex = 1;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(12, 58);
        label2.Name = "label2";
        label2.Size = new Size(97, 15);
        label2.TabIndex = 2;
        label2.Text = "PeerAMid Server:";
        // 
        // _textBoxUsers
        // 
        _textBoxUsers.AcceptsReturn = true;
        _textBoxUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _textBoxUsers.Location = new Point(136, 58);
        _textBoxUsers.Multiline = true;
        _textBoxUsers.Name = "_textBoxUsers";
        _textBoxUsers.Size = new Size(280, 138);
        _textBoxUsers.TabIndex = 3;
        // 
        // textBox2
        // 
        textBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        textBox2.Location = new Point(12, 211);
        textBox2.Multiline = true;
        textBox2.Name = "textBox2";
        textBox2.ReadOnly = true;
        textBox2.Size = new Size(404, 39);
        textBox2.TabIndex = 4;
        textBox2.Text = "Multiple user names can be entered in the text box, seperated by spaces or semicolons.";
        // 
        // _buttonClose
        // 
        _buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _buttonClose.DialogResult = DialogResult.Cancel;
        _buttonClose.Location = new Point(335, 262);
        _buttonClose.Name = "_buttonClose";
        _buttonClose.Size = new Size(75, 23);
        _buttonClose.TabIndex = 5;
        _buttonClose.Text = "Close";
        _buttonClose.UseVisualStyleBackColor = true;
        _buttonClose.Click += CloseClicked;
        // 
        // _buttonOK
        // 
        _buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _buttonOK.DialogResult = DialogResult.OK;
        _buttonOK.Location = new Point(245, 262);
        _buttonOK.Name = "_buttonOK";
        _buttonOK.Size = new Size(75, 23);
        _buttonOK.TabIndex = 6;
        _buttonOK.Text = "Add User";
        _buttonOK.UseVisualStyleBackColor = true;
        _buttonOK.Click += OkClicked;
        // 
        // MainForm
        // 
        AcceptButton = _buttonOK;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _buttonClose;
        ClientSize = new Size(428, 297);
        Controls.Add(_buttonOK);
        Controls.Add(_buttonClose);
        Controls.Add(textBox2);
        Controls.Add(_textBoxUsers);
        Controls.Add(label2);
        Controls.Add(_comboBoxServer);
        Controls.Add(label1);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MainForm";
        Text = "Add Users to PeerAMid";
        Load += MainFormOnLoad;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label label1;
    private ComboBox _comboBoxServer;
    private Label label2;
    private TextBox _textBoxUsers;
    private TextBox textBox2;
    private Button _buttonClose;
    private Button _buttonOK;
}
