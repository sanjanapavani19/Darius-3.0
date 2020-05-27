<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim Series3 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.Button_Phasor = New System.Windows.Forms.Button()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.Textbox_exposure = New System.Windows.Forms.ToolStripTextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Button_Home = New System.Windows.Forms.Button()
        Me.Button_bottom = New System.Windows.Forms.Button()
        Me.Button_top = New System.Windows.Forms.Button()
        Me.Button_left = New System.Windows.Forms.Button()
        Me.Button_right = New System.Windows.Forms.Button()
        Me.Button_adjustBrightness = New System.Windows.Forms.Button()
        Me.RadioButton_zoom_out = New System.Windows.Forms.RadioButton()
        Me.RadioButton_zoom_in = New System.Windows.Forms.RadioButton()
        Me.TabControl_Settings = New System.Windows.Forms.TabControl()
        Me.TabPage12 = New System.Windows.Forms.TabPage()
        Me.Label_html_dir = New System.Windows.Forms.Label()
        Me.CheckBox_no_html = New System.Windows.Forms.CheckBox()
        Me.CheckBox_incognito = New System.Windows.Forms.CheckBox()
        Me.Button_experiment = New System.Windows.Forms.Button()
        Me.Button_OpenHtml = New System.Windows.Forms.Button()
        Me.TabPage13 = New System.Windows.Forms.TabPage()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.TextBox_Acceleration = New System.Windows.Forms.TextBox()
        Me.TextBox_Speed = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TextBoxGC = New System.Windows.Forms.TextBox()
        Me.TextBoxGY = New System.Windows.Forms.TextBox()
        Me.TextBoxGain = New System.Windows.Forms.TextBox()
        Me.LabelGC = New System.Windows.Forms.Label()
        Me.LabelGY = New System.Windows.Forms.Label()
        Me.LabelGain = New System.Windows.Forms.Label()
        Me.TextBox_GainB = New System.Windows.Forms.TextBox()
        Me.TextBox_GainG = New System.Windows.Forms.TextBox()
        Me.TextBox_GainR = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.HScrollBar_PhasorPanels = New System.Windows.Forms.HScrollBar()
        Me.TrackBar_Phasorthreshold = New System.Windows.Forms.TrackBar()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Button_Refresh = New System.Windows.Forms.Button()
        Me.RadioButton_unmix3 = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Conversion = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Zoom = New System.Windows.Forms.RadioButton()
        Me.PictureBox_Phasor = New System.Windows.Forms.PictureBox()
        Me.PictureBoxLive = New System.Windows.Forms.PictureBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.PictureBox0 = New System.Windows.Forms.PictureBox()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.Button_Save = New System.Windows.Forms.Button()
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.VScrollBarB = New System.Windows.Forms.VScrollBar()
        Me.VScrollBarC = New System.Windows.Forms.VScrollBar()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.VScrollBar_HUE = New System.Windows.Forms.VScrollBar()
        Me.Chart2 = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.ToolStrip1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TabControl_Settings.SuspendLayout()
        Me.TabPage12.SuspendLayout()
        Me.TabPage13.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.TrackBar_Phasorthreshold, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.PictureBox_Phasor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxLive, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.PictureBox0, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage4.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage5.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage6.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Chart2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button_Phasor
        '
        Me.Button_Phasor.Location = New System.Drawing.Point(93, 28)
        Me.Button_Phasor.Name = "Button_Phasor"
        Me.Button_Phasor.Size = New System.Drawing.Size(75, 45)
        Me.Button_Phasor.TabIndex = 0
        Me.Button_Phasor.Text = "Phasor !"
        Me.Button_Phasor.UseVisualStyleBackColor = True
        '
        'ToolStrip1
        '
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(48, 48)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.Textbox_exposure})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1465, 25)
        Me.ToolStrip1.TabIndex = 46
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(54, 22)
        Me.ToolStripLabel1.Text = "Exposure"
        '
        'Textbox_exposure
        '
        Me.Textbox_exposure.ForeColor = System.Drawing.SystemColors.WindowFrame
        Me.Textbox_exposure.Name = "Textbox_exposure"
        Me.Textbox_exposure.Size = New System.Drawing.Size(40, 25)
        Me.Textbox_exposure.Text = "0.1"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Button_Home)
        Me.GroupBox3.Controls.Add(Me.Button_bottom)
        Me.GroupBox3.Controls.Add(Me.Button_top)
        Me.GroupBox3.Controls.Add(Me.Button_left)
        Me.GroupBox3.Controls.Add(Me.Button_right)
        Me.GroupBox3.Location = New System.Drawing.Point(1257, 580)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(195, 208)
        Me.GroupBox3.TabIndex = 106
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "XYZ"
        '
        'Button_Home
        '
        Me.Button_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Button_Home.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_Home.ForeColor = System.Drawing.SystemColors.Control
        Me.Button_Home.Location = New System.Drawing.Point(62, 72)
        Me.Button_Home.Margin = New System.Windows.Forms.Padding(0)
        Me.Button_Home.Name = "Button_Home"
        Me.Button_Home.Size = New System.Drawing.Size(53, 47)
        Me.Button_Home.TabIndex = 40
        Me.Button_Home.UseVisualStyleBackColor = True
        '
        'Button_bottom
        '
        Me.Button_bottom.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button_bottom.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_bottom.Image = CType(resources.GetObject("Button_bottom.Image"), System.Drawing.Image)
        Me.Button_bottom.Location = New System.Drawing.Point(73, 132)
        Me.Button_bottom.Name = "Button_bottom"
        Me.Button_bottom.Size = New System.Drawing.Size(40, 40)
        Me.Button_bottom.TabIndex = 39
        Me.Button_bottom.UseVisualStyleBackColor = True
        '
        'Button_top
        '
        Me.Button_top.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button_top.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_top.Image = CType(resources.GetObject("Button_top.Image"), System.Drawing.Image)
        Me.Button_top.Location = New System.Drawing.Point(73, 29)
        Me.Button_top.Name = "Button_top"
        Me.Button_top.Size = New System.Drawing.Size(40, 40)
        Me.Button_top.TabIndex = 38
        Me.Button_top.UseVisualStyleBackColor = True
        '
        'Button_left
        '
        Me.Button_left.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button_left.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_left.Image = CType(resources.GetObject("Button_left.Image"), System.Drawing.Image)
        Me.Button_left.Location = New System.Drawing.Point(19, 75)
        Me.Button_left.Name = "Button_left"
        Me.Button_left.Size = New System.Drawing.Size(40, 40)
        Me.Button_left.TabIndex = 37
        Me.Button_left.UseVisualStyleBackColor = True
        '
        'Button_right
        '
        Me.Button_right.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Button_right.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_right.Image = CType(resources.GetObject("Button_right.Image"), System.Drawing.Image)
        Me.Button_right.Location = New System.Drawing.Point(124, 75)
        Me.Button_right.Name = "Button_right"
        Me.Button_right.Size = New System.Drawing.Size(40, 40)
        Me.Button_right.TabIndex = 36
        Me.Button_right.UseVisualStyleBackColor = True
        '
        'Button_adjustBrightness
        '
        Me.Button_adjustBrightness.Location = New System.Drawing.Point(12, 28)
        Me.Button_adjustBrightness.Name = "Button_adjustBrightness"
        Me.Button_adjustBrightness.Size = New System.Drawing.Size(75, 45)
        Me.Button_adjustBrightness.TabIndex = 108
        Me.Button_adjustBrightness.Text = "Adjust Brightness"
        Me.Button_adjustBrightness.UseVisualStyleBackColor = True
        '
        'RadioButton_zoom_out
        '
        Me.RadioButton_zoom_out.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButton_zoom_out.Font = New System.Drawing.Font("Microsoft Sans Serif", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadioButton_zoom_out.Location = New System.Drawing.Point(223, 28)
        Me.RadioButton_zoom_out.Name = "RadioButton_zoom_out"
        Me.RadioButton_zoom_out.Size = New System.Drawing.Size(43, 43)
        Me.RadioButton_zoom_out.TabIndex = 110
        Me.RadioButton_zoom_out.Text = "-"
        Me.RadioButton_zoom_out.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.RadioButton_zoom_out.UseVisualStyleBackColor = True
        '
        'RadioButton_zoom_in
        '
        Me.RadioButton_zoom_in.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButton_zoom_in.AutoSize = True
        Me.RadioButton_zoom_in.Font = New System.Drawing.Font("Microsoft Sans Serif", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadioButton_zoom_in.Location = New System.Drawing.Point(174, 28)
        Me.RadioButton_zoom_in.Name = "RadioButton_zoom_in"
        Me.RadioButton_zoom_in.Size = New System.Drawing.Size(43, 43)
        Me.RadioButton_zoom_in.TabIndex = 109
        Me.RadioButton_zoom_in.Text = "+"
        Me.RadioButton_zoom_in.UseVisualStyleBackColor = True
        '
        'TabControl_Settings
        '
        Me.TabControl_Settings.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
        Me.TabControl_Settings.Controls.Add(Me.TabPage12)
        Me.TabControl_Settings.Controls.Add(Me.TabPage13)
        Me.TabControl_Settings.Controls.Add(Me.TabPage1)
        Me.TabControl_Settings.Location = New System.Drawing.Point(1257, 809)
        Me.TabControl_Settings.Name = "TabControl_Settings"
        Me.TabControl_Settings.SelectedIndex = 0
        Me.TabControl_Settings.Size = New System.Drawing.Size(449, 255)
        Me.TabControl_Settings.TabIndex = 111
        '
        'TabPage12
        '
        Me.TabPage12.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage12.Controls.Add(Me.Label_html_dir)
        Me.TabPage12.Controls.Add(Me.CheckBox_no_html)
        Me.TabPage12.Controls.Add(Me.CheckBox_incognito)
        Me.TabPage12.Controls.Add(Me.Button_experiment)
        Me.TabPage12.Controls.Add(Me.Button_OpenHtml)
        Me.TabPage12.Location = New System.Drawing.Point(4, 25)
        Me.TabPage12.Name = "TabPage12"
        Me.TabPage12.Size = New System.Drawing.Size(441, 226)
        Me.TabPage12.TabIndex = 2
        Me.TabPage12.Text = "HTML"
        '
        'Label_html_dir
        '
        Me.Label_html_dir.AutoSize = True
        Me.Label_html_dir.Location = New System.Drawing.Point(16, 80)
        Me.Label_html_dir.Name = "Label_html_dir"
        Me.Label_html_dir.Size = New System.Drawing.Size(75, 13)
        Me.Label_html_dir.TabIndex = 36
        Me.Label_html_dir.Text = "HTML Folder :"
        '
        'CheckBox_no_html
        '
        Me.CheckBox_no_html.AutoSize = True
        Me.CheckBox_no_html.Location = New System.Drawing.Point(19, 128)
        Me.CheckBox_no_html.Name = "CheckBox_no_html"
        Me.CheckBox_no_html.Size = New System.Drawing.Size(121, 17)
        Me.CheckBox_no_html.TabIndex = 35
        Me.CheckBox_no_html.Text = "Don't write to HTML"
        Me.CheckBox_no_html.UseVisualStyleBackColor = True
        '
        'CheckBox_incognito
        '
        Me.CheckBox_incognito.AutoSize = True
        Me.CheckBox_incognito.Location = New System.Drawing.Point(19, 104)
        Me.CheckBox_incognito.Name = "CheckBox_incognito"
        Me.CheckBox_incognito.Size = New System.Drawing.Size(129, 17)
        Me.CheckBox_incognito.TabIndex = 34
        Me.CheckBox_incognito.Text = "Get hide from Richard"
        Me.CheckBox_incognito.UseVisualStyleBackColor = True
        '
        'Button_experiment
        '
        Me.Button_experiment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Button_experiment.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_experiment.ForeColor = System.Drawing.SystemColors.Control
        Me.Button_experiment.Location = New System.Drawing.Point(7, 22)
        Me.Button_experiment.Margin = New System.Windows.Forms.Padding(0)
        Me.Button_experiment.Name = "Button_experiment"
        Me.Button_experiment.Size = New System.Drawing.Size(30, 30)
        Me.Button_experiment.TabIndex = 45
        Me.Button_experiment.UseVisualStyleBackColor = True
        '
        'Button_OpenHtml
        '
        Me.Button_OpenHtml.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.Button_OpenHtml.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_OpenHtml.ForeColor = System.Drawing.SystemColors.Control
        Me.Button_OpenHtml.Location = New System.Drawing.Point(50, 22)
        Me.Button_OpenHtml.Margin = New System.Windows.Forms.Padding(0)
        Me.Button_OpenHtml.Name = "Button_OpenHtml"
        Me.Button_OpenHtml.Size = New System.Drawing.Size(30, 30)
        Me.Button_OpenHtml.TabIndex = 48
        Me.Button_OpenHtml.UseVisualStyleBackColor = True
        '
        'TabPage13
        '
        Me.TabPage13.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage13.Controls.Add(Me.Label19)
        Me.TabPage13.Controls.Add(Me.TextBox_Acceleration)
        Me.TabPage13.Controls.Add(Me.TextBox_Speed)
        Me.TabPage13.Controls.Add(Me.Label6)
        Me.TabPage13.Controls.Add(Me.TextBox3)
        Me.TabPage13.Controls.Add(Me.TextBox2)
        Me.TabPage13.Controls.Add(Me.TextBox1)
        Me.TabPage13.Controls.Add(Me.Label3)
        Me.TabPage13.Controls.Add(Me.Label2)
        Me.TabPage13.Controls.Add(Me.Label1)
        Me.TabPage13.Location = New System.Drawing.Point(4, 25)
        Me.TabPage13.Name = "TabPage13"
        Me.TabPage13.Size = New System.Drawing.Size(441, 226)
        Me.TabPage13.TabIndex = 3
        Me.TabPage13.Text = "Stage"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(195, 85)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(34, 16)
        Me.Label19.TabIndex = 41
        Me.Label19.Text = "Acc."
        '
        'TextBox_Acceleration
        '
        Me.TextBox_Acceleration.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox_Acceleration.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox_Acceleration.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox_Acceleration.Location = New System.Drawing.Point(250, 81)
        Me.TextBox_Acceleration.Name = "TextBox_Acceleration"
        Me.TextBox_Acceleration.Size = New System.Drawing.Size(56, 20)
        Me.TextBox_Acceleration.TabIndex = 40
        Me.TextBox_Acceleration.Text = "6"
        Me.TextBox_Acceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBox_Speed
        '
        Me.TextBox_Speed.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox_Speed.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox_Speed.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox_Speed.Location = New System.Drawing.Point(249, 46)
        Me.TextBox_Speed.Name = "TextBox_Speed"
        Me.TextBox_Speed.Size = New System.Drawing.Size(56, 20)
        Me.TextBox_Speed.TabIndex = 39
        Me.TextBox_Speed.Text = "6"
        Me.TextBox_Speed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(195, 46)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(49, 16)
        Me.Label6.TabIndex = 38
        Me.Label6.Text = "Speed"
        '
        'TextBox3
        '
        Me.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox3.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox3.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox3.Location = New System.Drawing.Point(85, 115)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(56, 20)
        Me.TextBox3.TabIndex = 37
        Me.TextBox3.Text = "0.1"
        Me.TextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBox2
        '
        Me.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox2.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox2.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox2.Location = New System.Drawing.Point(85, 79)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(56, 20)
        Me.TextBox2.TabIndex = 36
        Me.TextBox2.Text = "0.1"
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBox1
        '
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox1.Location = New System.Drawing.Point(85, 44)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(56, 20)
        Me.TextBox1.TabIndex = 35
        Me.TextBox1.Text = "0.1"
        Me.TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(31, 119)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(48, 16)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Z (um):"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(31, 81)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 16)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Y (mm):"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(31, 44)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(52, 16)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "X (mm):"
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabPage1.Controls.Add(Me.TextBoxGC)
        Me.TabPage1.Controls.Add(Me.TextBoxGY)
        Me.TabPage1.Controls.Add(Me.TextBoxGain)
        Me.TabPage1.Controls.Add(Me.LabelGC)
        Me.TabPage1.Controls.Add(Me.LabelGY)
        Me.TabPage1.Controls.Add(Me.LabelGain)
        Me.TabPage1.Controls.Add(Me.TextBox_GainB)
        Me.TabPage1.Controls.Add(Me.TextBox_GainG)
        Me.TabPage1.Controls.Add(Me.TextBox_GainR)
        Me.TabPage1.Controls.Add(Me.Label13)
        Me.TabPage1.Controls.Add(Me.Label14)
        Me.TabPage1.Controls.Add(Me.Label17)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(441, 226)
        Me.TabPage1.TabIndex = 5
        Me.TabPage1.Text = "Camera"
        '
        'TextBoxGC
        '
        Me.TextBoxGC.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBoxGC.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxGC.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBoxGC.Location = New System.Drawing.Point(296, 110)
        Me.TextBoxGC.Name = "TextBoxGC"
        Me.TextBoxGC.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxGC.TabIndex = 49
        Me.TextBoxGC.Text = "0"
        Me.TextBoxGC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBoxGY
        '
        Me.TextBoxGY.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBoxGY.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxGY.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBoxGY.Location = New System.Drawing.Point(296, 74)
        Me.TextBoxGY.Name = "TextBoxGY"
        Me.TextBoxGY.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxGY.TabIndex = 48
        Me.TextBoxGY.Text = "1"
        Me.TextBoxGY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBoxGain
        '
        Me.TextBoxGain.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBoxGain.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxGain.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBoxGain.Location = New System.Drawing.Point(296, 39)
        Me.TextBoxGain.Name = "TextBoxGain"
        Me.TextBoxGain.Size = New System.Drawing.Size(56, 20)
        Me.TextBoxGain.TabIndex = 47
        Me.TextBoxGain.Text = "10"
        Me.TextBoxGain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LabelGC
        '
        Me.LabelGC.AutoSize = True
        Me.LabelGC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelGC.Location = New System.Drawing.Point(225, 114)
        Me.LabelGC.Name = "LabelGC"
        Me.LabelGC.Size = New System.Drawing.Size(65, 16)
        Me.LabelGC.TabIndex = 46
        Me.LabelGC.Text = "GammaC"
        '
        'LabelGY
        '
        Me.LabelGY.AutoSize = True
        Me.LabelGY.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelGY.Location = New System.Drawing.Point(225, 76)
        Me.LabelGY.Name = "LabelGY"
        Me.LabelGY.Size = New System.Drawing.Size(65, 16)
        Me.LabelGY.TabIndex = 45
        Me.LabelGY.Text = "GammaY"
        '
        'LabelGain
        '
        Me.LabelGain.AutoSize = True
        Me.LabelGain.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelGain.Location = New System.Drawing.Point(225, 41)
        Me.LabelGain.Name = "LabelGain"
        Me.LabelGain.Size = New System.Drawing.Size(36, 16)
        Me.LabelGain.TabIndex = 44
        Me.LabelGain.Text = "Gain"
        '
        'TextBox_GainB
        '
        Me.TextBox_GainB.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox_GainB.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox_GainB.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox_GainB.Location = New System.Drawing.Point(89, 110)
        Me.TextBox_GainB.Name = "TextBox_GainB"
        Me.TextBox_GainB.Size = New System.Drawing.Size(56, 20)
        Me.TextBox_GainB.TabIndex = 43
        Me.TextBox_GainB.Text = "1"
        Me.TextBox_GainB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBox_GainG
        '
        Me.TextBox_GainG.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox_GainG.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox_GainG.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox_GainG.Location = New System.Drawing.Point(89, 74)
        Me.TextBox_GainG.Name = "TextBox_GainG"
        Me.TextBox_GainG.Size = New System.Drawing.Size(56, 20)
        Me.TextBox_GainG.TabIndex = 42
        Me.TextBox_GainG.Text = "1"
        Me.TextBox_GainG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBox_GainR
        '
        Me.TextBox_GainR.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox_GainR.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox_GainR.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TextBox_GainR.Location = New System.Drawing.Point(89, 39)
        Me.TextBox_GainR.Name = "TextBox_GainR"
        Me.TextBox_GainR.Size = New System.Drawing.Size(56, 20)
        Me.TextBox_GainR.TabIndex = 41
        Me.TextBox_GainR.Text = "1"
        Me.TextBox_GainR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(35, 114)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(48, 16)
        Me.Label13.TabIndex = 40
        Me.Label13.Text = "Gain B"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(35, 76)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(49, 16)
        Me.Label14.TabIndex = 39
        Me.Label14.Text = "Gain G"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(35, 39)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(49, 16)
        Me.Label17.TabIndex = 38
        Me.Label17.Text = "Gain R"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(633, 28)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 112
        Me.Button1.Text = "White"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(714, 28)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 113
        Me.Button2.Text = "Blue"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(795, 28)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 114
        Me.Button3.Text = "OFF"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'HScrollBar_PhasorPanels
        '
        Me.HScrollBar_PhasorPanels.LargeChange = 1
        Me.HScrollBar_PhasorPanels.Location = New System.Drawing.Point(1257, 526)
        Me.HScrollBar_PhasorPanels.Name = "HScrollBar_PhasorPanels"
        Me.HScrollBar_PhasorPanels.Size = New System.Drawing.Size(400, 26)
        Me.HScrollBar_PhasorPanels.TabIndex = 115
        '
        'TrackBar_Phasorthreshold
        '
        Me.TrackBar_Phasorthreshold.LargeChange = 1
        Me.TrackBar_Phasorthreshold.Location = New System.Drawing.Point(272, 28)
        Me.TrackBar_Phasorthreshold.Maximum = 100
        Me.TrackBar_Phasorthreshold.Name = "TrackBar_Phasorthreshold"
        Me.TrackBar_Phasorthreshold.Size = New System.Drawing.Size(323, 45)
        Me.TrackBar_Phasorthreshold.TabIndex = 116
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Button_Refresh)
        Me.GroupBox1.Controls.Add(Me.RadioButton_unmix3)
        Me.GroupBox1.Controls.Add(Me.RadioButton_Conversion)
        Me.GroupBox1.Controls.Add(Me.RadioButton_Zoom)
        Me.GroupBox1.Location = New System.Drawing.Point(885, 19)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(417, 63)
        Me.GroupBox1.TabIndex = 117
        Me.GroupBox1.TabStop = False
        '
        'Button_Refresh
        '
        Me.Button_Refresh.BackgroundImage = Global.SciCam.My.Resources.Resources.REFRESH
        Me.Button_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Button_Refresh.Location = New System.Drawing.Point(369, 17)
        Me.Button_Refresh.Name = "Button_Refresh"
        Me.Button_Refresh.Size = New System.Drawing.Size(40, 37)
        Me.Button_Refresh.TabIndex = 88
        Me.Button_Refresh.UseVisualStyleBackColor = True
        '
        'RadioButton_unmix3
        '
        Me.RadioButton_unmix3.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButton_unmix3.BackgroundImage = Global.SciCam.My.Resources.Resources.unmix3
        Me.RadioButton_unmix3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.RadioButton_unmix3.Location = New System.Drawing.Point(109, 17)
        Me.RadioButton_unmix3.MaximumSize = New System.Drawing.Size(50, 50)
        Me.RadioButton_unmix3.Name = "RadioButton_unmix3"
        Me.RadioButton_unmix3.Size = New System.Drawing.Size(40, 40)
        Me.RadioButton_unmix3.TabIndex = 87
        Me.RadioButton_unmix3.UseVisualStyleBackColor = True
        '
        'RadioButton_Conversion
        '
        Me.RadioButton_Conversion.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButton_Conversion.BackgroundImage = Global.SciCam.My.Resources.Resources.conversion
        Me.RadioButton_Conversion.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.RadioButton_Conversion.Location = New System.Drawing.Point(63, 17)
        Me.RadioButton_Conversion.MaximumSize = New System.Drawing.Size(50, 50)
        Me.RadioButton_Conversion.Name = "RadioButton_Conversion"
        Me.RadioButton_Conversion.Size = New System.Drawing.Size(40, 40)
        Me.RadioButton_Conversion.TabIndex = 86
        Me.RadioButton_Conversion.UseVisualStyleBackColor = True
        '
        'RadioButton_Zoom
        '
        Me.RadioButton_Zoom.Appearance = System.Windows.Forms.Appearance.Button
        Me.RadioButton_Zoom.BackgroundImage = Global.SciCam.My.Resources.Resources.Zoom
        Me.RadioButton_Zoom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.RadioButton_Zoom.Location = New System.Drawing.Point(17, 17)
        Me.RadioButton_Zoom.MaximumSize = New System.Drawing.Size(50, 50)
        Me.RadioButton_Zoom.Name = "RadioButton_Zoom"
        Me.RadioButton_Zoom.Size = New System.Drawing.Size(40, 40)
        Me.RadioButton_Zoom.TabIndex = 85
        Me.RadioButton_Zoom.UseVisualStyleBackColor = True
        '
        'PictureBox_Phasor
        '
        Me.PictureBox_Phasor.BackColor = System.Drawing.Color.Black
        Me.PictureBox_Phasor.Location = New System.Drawing.Point(1257, 101)
        Me.PictureBox_Phasor.Name = "PictureBox_Phasor"
        Me.PictureBox_Phasor.Size = New System.Drawing.Size(400, 400)
        Me.PictureBox_Phasor.TabIndex = 107
        Me.PictureBox_Phasor.TabStop = False
        '
        'PictureBoxLive
        '
        Me.PictureBoxLive.BackColor = System.Drawing.Color.Black
        Me.PictureBoxLive.Location = New System.Drawing.Point(10, 28)
        Me.PictureBoxLive.Name = "PictureBoxLive"
        Me.PictureBoxLive.Size = New System.Drawing.Size(966, 946)
        Me.PictureBoxLive.TabIndex = 1
        Me.PictureBoxLive.TabStop = False
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Location = New System.Drawing.Point(139, 82)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1236, 1028)
        Me.TabControl1.TabIndex = 118
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.PictureBoxLive)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1228, 1002)
        Me.TabPage2.TabIndex = 0
        Me.TabPage2.Text = "Live"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.PictureBox0)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(1228, 1002)
        Me.TabPage3.TabIndex = 1
        Me.TabPage3.Text = "Bright field"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'PictureBox0
        '
        Me.PictureBox0.BackColor = System.Drawing.Color.Black
        Me.PictureBox0.Location = New System.Drawing.Point(10, 28)
        Me.PictureBox0.Name = "PictureBox0"
        Me.PictureBox0.Size = New System.Drawing.Size(966, 946)
        Me.PictureBox0.TabIndex = 2
        Me.PictureBox0.TabStop = False
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.PictureBox1)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(1228, 1002)
        Me.TabPage4.TabIndex = 2
        Me.TabPage4.Text = "Fluorescence"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Black
        Me.PictureBox1.Location = New System.Drawing.Point(10, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(966, 946)
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.PictureBox2)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(1228, 1002)
        Me.TabPage5.TabIndex = 3
        Me.TabPage5.Text = "Collagen"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.Color.Black
        Me.PictureBox2.Location = New System.Drawing.Point(10, 28)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(966, 946)
        Me.PictureBox2.TabIndex = 2
        Me.PictureBox2.TabStop = False
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.Button_Save)
        Me.TabPage6.Controls.Add(Me.PictureBox3)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(1228, 1002)
        Me.TabPage6.TabIndex = 4
        Me.TabPage6.Text = "Overlay"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'Button_Save
        '
        Me.Button_Save.Location = New System.Drawing.Point(3, 3)
        Me.Button_Save.Name = "Button_Save"
        Me.Button_Save.Size = New System.Drawing.Size(55, 50)
        Me.Button_Save.TabIndex = 3
        Me.Button_Save.Text = "Save"
        Me.Button_Save.UseVisualStyleBackColor = True
        '
        'PictureBox3
        '
        Me.PictureBox3.BackColor = System.Drawing.Color.Black
        Me.PictureBox3.Location = New System.Drawing.Point(10, 28)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(966, 946)
        Me.PictureBox3.TabIndex = 2
        Me.PictureBox3.TabStop = False
        '
        'VScrollBarB
        '
        Me.VScrollBarB.Location = New System.Drawing.Point(69, 171)
        Me.VScrollBarB.Maximum = 255
        Me.VScrollBarB.Name = "VScrollBarB"
        Me.VScrollBarB.Size = New System.Drawing.Size(28, 295)
        Me.VScrollBarB.TabIndex = 5
        '
        'VScrollBarC
        '
        Me.VScrollBarC.Location = New System.Drawing.Point(108, 171)
        Me.VScrollBarC.Maximum = 255
        Me.VScrollBarC.Name = "VScrollBarC"
        Me.VScrollBarC.Size = New System.Drawing.Size(28, 295)
        Me.VScrollBarC.TabIndex = 4
        Me.VScrollBarC.Value = 255
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'VScrollBar_HUE
        '
        Me.VScrollBar_HUE.Location = New System.Drawing.Point(37, 171)
        Me.VScrollBar_HUE.Name = "VScrollBar_HUE"
        Me.VScrollBar_HUE.Size = New System.Drawing.Size(19, 295)
        Me.VScrollBar_HUE.TabIndex = 119
        '
        'Chart2
        '
        Me.Chart2.BackColor = System.Drawing.Color.Transparent
        Me.Chart2.BorderlineColor = System.Drawing.Color.Transparent
        Me.Chart2.BorderSkin.PageColor = System.Drawing.Color.Transparent
        ChartArea1.AxisX.MajorGrid.Enabled = False
        ChartArea1.AxisY.IsMarksNextToAxis = False
        ChartArea1.AxisY.LabelStyle.Enabled = False
        ChartArea1.AxisY.LineWidth = 0
        ChartArea1.AxisY.MajorGrid.Enabled = False
        ChartArea1.AxisY.MajorTickMark.Enabled = False
        ChartArea1.AxisY2.MajorGrid.Enabled = False
        ChartArea1.AxisY2.MajorTickMark.Enabled = False
        ChartArea1.BackColor = System.Drawing.Color.Transparent
        ChartArea1.BackSecondaryColor = System.Drawing.Color.Transparent
        ChartArea1.Name = "ChartArea1"
        Me.Chart2.ChartAreas.Add(ChartArea1)
        Me.Chart2.Location = New System.Drawing.Point(143, 1212)
        Me.Chart2.Name = "Chart2"
        Series1.BackImageTransparentColor = System.Drawing.Color.Transparent
        Series1.BackSecondaryColor = System.Drawing.Color.Transparent
        Series1.BorderColor = System.Drawing.Color.Transparent
        Series1.ChartArea = "ChartArea1"
        Series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area
        Series1.Color = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer))
        Series1.IsVisibleInLegend = False
        Series1.Name = "Series1"
        Series2.ChartArea = "ChartArea1"
        Series2.Name = "Series2"
        Series3.ChartArea = "ChartArea1"
        Series3.Name = "Series3"
        Me.Chart2.Series.Add(Series1)
        Me.Chart2.Series.Add(Series2)
        Me.Chart2.Series.Add(Series3)
        Me.Chart2.Size = New System.Drawing.Size(508, 182)
        Me.Chart2.TabIndex = 121
        Me.Chart2.Text = "Chart2"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1465, 1490)
        Me.Controls.Add(Me.Chart2)
        Me.Controls.Add(Me.VScrollBar_HUE)
        Me.Controls.Add(Me.VScrollBarC)
        Me.Controls.Add(Me.VScrollBarB)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.TrackBar_Phasorthreshold)
        Me.Controls.Add(Me.HScrollBar_PhasorPanels)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TabControl_Settings)
        Me.Controls.Add(Me.RadioButton_zoom_out)
        Me.Controls.Add(Me.RadioButton_zoom_in)
        Me.Controls.Add(Me.Button_adjustBrightness)
        Me.Controls.Add(Me.PictureBox_Phasor)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.Button_Phasor)
        Me.Name = "Form1"
        Me.Text = "SciCam"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.TabControl_Settings.ResumeLayout(False)
        Me.TabPage12.ResumeLayout(False)
        Me.TabPage12.PerformLayout()
        Me.TabPage13.ResumeLayout(False)
        Me.TabPage13.PerformLayout()
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        CType(Me.TrackBar_Phasorthreshold, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.PictureBox_Phasor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxLive, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        CType(Me.PictureBox0, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage4.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage5.ResumeLayout(False)
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage6.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Chart2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Button_Phasor As Button
    Friend WithEvents PictureBoxLive As PictureBox
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents Textbox_exposure As ToolStripTextBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Button_Home As Button
    Friend WithEvents Button_bottom As Button
    Friend WithEvents Button_top As Button
    Friend WithEvents Button_left As Button
    Friend WithEvents Button_right As Button
    Friend WithEvents PictureBox_Phasor As PictureBox
    Friend WithEvents Button_adjustBrightness As Button
    Friend WithEvents RadioButton_zoom_out As RadioButton
    Friend WithEvents RadioButton_zoom_in As RadioButton
    Friend WithEvents TabControl_Settings As TabControl
    Friend WithEvents TabPage12 As TabPage
    Friend WithEvents Label_html_dir As Label
    Friend WithEvents CheckBox_no_html As CheckBox
    Friend WithEvents CheckBox_incognito As CheckBox
    Friend WithEvents Button_experiment As Button
    Friend WithEvents Button_OpenHtml As Button
    Friend WithEvents TabPage13 As TabPage
    Friend WithEvents Label19 As Label
    Friend WithEvents TextBox_Acceleration As TextBox
    Friend WithEvents TextBox_Speed As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TextBoxGC As TextBox
    Friend WithEvents TextBoxGY As TextBox
    Friend WithEvents TextBoxGain As TextBox
    Friend WithEvents LabelGC As Label
    Friend WithEvents LabelGY As Label
    Friend WithEvents LabelGain As Label
    Friend WithEvents TextBox_GainB As TextBox
    Friend WithEvents TextBox_GainG As TextBox
    Friend WithEvents TextBox_GainR As TextBox
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents HScrollBar_PhasorPanels As HScrollBar
    Friend WithEvents TrackBar_Phasorthreshold As TrackBar
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents RadioButton_unmix3 As RadioButton
    Friend WithEvents RadioButton_Conversion As RadioButton
    Friend WithEvents RadioButton_Zoom As RadioButton
    Friend WithEvents Button_Refresh As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents PictureBox0 As PictureBox
    Friend WithEvents TabPage4 As TabPage
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents TabPage5 As TabPage
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents TabPage6 As TabPage
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents Button_Save As Button
    Friend WithEvents VScrollBarB As VScrollBar
    Friend WithEvents VScrollBarC As VScrollBar
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents VScrollBar_HUE As VScrollBar
    Friend WithEvents Chart2 As DataVisualization.Charting.Chart
End Class
