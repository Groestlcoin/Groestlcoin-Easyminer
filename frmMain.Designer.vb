
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.pbGroestl = New System.Windows.Forms.PictureBox()
        Me.gbWallet = New System.Windows.Forms.GroupBox()
        Me.btnOpenHelp = New System.Windows.Forms.Button()
        Me.rbCustomAddress = New System.Windows.Forms.RadioButton()
        Me.rbUseElectrum = New System.Windows.Forms.RadioButton()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.lblAddress = New System.Windows.Forms.Label()
        Me.btnWalletSetup = New System.Windows.Forms.Button()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.llWebsite = New System.Windows.Forms.LinkLabel()
        Me.gbStatus = New System.Windows.Forms.GroupBox()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.txtOutput = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.rbUsedwarfPool = New System.Windows.Forms.RadioButton()
        Me.rbCustomPool = New System.Windows.Forms.RadioButton()
        Me.txtPool = New System.Windows.Forms.TextBox()
        Me.gbMiningPool = New System.Windows.Forms.GroupBox()
        Me.lblCustomAddress = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.txtUsername = New System.Windows.Forms.TextBox()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.txtthreads = New System.Windows.Forms.TextBox()
        Me.rbGroestl = New System.Windows.Forms.RadioButton()
        Me.gbMethod = New System.Windows.Forms.GroupBox()
        CType(Me.pbGroestl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbWallet.SuspendLayout()
        Me.gbStatus.SuspendLayout()
        Me.gbMiningPool.SuspendLayout()
        Me.gbMethod.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbGroestl
        '
        Me.pbGroestl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbGroestl.Image = Global.EasyMiner.My.Resources.Resources.header_logo2
        Me.pbGroestl.Location = New System.Drawing.Point(176, 1)
        Me.pbGroestl.Name = "pbGroestl"
        Me.pbGroestl.Size = New System.Drawing.Size(313, 91)
        Me.pbGroestl.TabIndex = 0
        Me.pbGroestl.TabStop = False
        '
        'gbWallet
        '
        Me.gbWallet.BackColor = System.Drawing.Color.WhiteSmoke
        Me.gbWallet.Controls.Add(Me.btnOpenHelp)
        Me.gbWallet.Controls.Add(Me.rbCustomAddress)
        Me.gbWallet.Controls.Add(Me.rbUseElectrum)
        Me.gbWallet.Controls.Add(Me.txtAddress)
        Me.gbWallet.Controls.Add(Me.lblAddress)
        Me.gbWallet.Controls.Add(Me.btnWalletSetup)
        Me.gbWallet.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbWallet.Location = New System.Drawing.Point(176, 111)
        Me.gbWallet.Name = "gbWallet"
        Me.gbWallet.Size = New System.Drawing.Size(315, 134)
        Me.gbWallet.TabIndex = 1
        Me.gbWallet.TabStop = False
        Me.gbWallet.Text = "Wallet Address"
        '
        'btnOpenHelp
        '
        Me.btnOpenHelp.Location = New System.Drawing.Point(12, 52)
        Me.btnOpenHelp.Name = "btnOpenHelp"
        Me.btnOpenHelp.Size = New System.Drawing.Size(136, 23)
        Me.btnOpenHelp.TabIndex = 12
        Me.btnOpenHelp.Text = "Display Help"
        Me.btnOpenHelp.UseVisualStyleBackColor = True
        '
        'rbCustomAddress
        '
        Me.rbCustomAddress.AutoSize = True
        Me.rbCustomAddress.Location = New System.Drawing.Point(12, 53)
        Me.rbCustomAddress.Name = "rbCustomAddress"
        Me.rbCustomAddress.Size = New System.Drawing.Size(127, 20)
        Me.rbCustomAddress.TabIndex = 11
        Me.rbCustomAddress.TabStop = True
        Me.rbCustomAddress.Text = "Custom Address"
        Me.rbCustomAddress.UseVisualStyleBackColor = True
        '
        'rbUseElectrum
        '
        Me.rbUseElectrum.AutoSize = True
        Me.rbUseElectrum.Location = New System.Drawing.Point(12, 23)
        Me.rbUseElectrum.Name = "rbUseElectrum"
        Me.rbUseElectrum.Size = New System.Drawing.Size(137, 20)
        Me.rbUseElectrum.TabIndex = 10
        Me.rbUseElectrum.TabStop = True
        Me.rbUseElectrum.Text = "Electrum Address"
        Me.rbUseElectrum.UseVisualStyleBackColor = True
        '
        'txtAddress
        '
        Me.txtAddress.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAddress.Location = New System.Drawing.Point(9, 104)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(268, 20)
        Me.txtAddress.TabIndex = 9
        '
        'lblAddress
        '
        Me.lblAddress.AutoSize = True
        Me.lblAddress.Location = New System.Drawing.Point(9, 80)
        Me.lblAddress.Name = "lblAddress"
        Me.lblAddress.Size = New System.Drawing.Size(118, 16)
        Me.lblAddress.TabIndex = 8
        Me.lblAddress.Text = "Selected Address:"
        '
        'btnWalletSetup
        '
        Me.btnWalletSetup.Location = New System.Drawing.Point(162, 52)
        Me.btnWalletSetup.Name = "btnWalletSetup"
        Me.btnWalletSetup.Size = New System.Drawing.Size(138, 24)
        Me.btnWalletSetup.TabIndex = 8
        Me.btnWalletSetup.Text = "Open Wallet"
        Me.btnWalletSetup.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStart.Location = New System.Drawing.Point(215, 467)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(114, 30)
        Me.btnStart.TabIndex = 2
        Me.btnStart.Text = "Start Mining"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Enabled = False
        Me.btnStop.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStop.Location = New System.Drawing.Point(338, 467)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(114, 30)
        Me.btnStop.TabIndex = 3
        Me.btnStop.Text = "Stop Mining"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'llWebsite
        '
        Me.llWebsite.ActiveLinkColor = System.Drawing.Color.Black
        Me.llWebsite.AutoSize = True
        Me.llWebsite.Location = New System.Drawing.Point(523, 476)
        Me.llWebsite.Name = "llWebsite"
        Me.llWebsite.Size = New System.Drawing.Size(103, 13)
        Me.llWebsite.TabIndex = 5
        Me.llWebsite.TabStop = True
        Me.llWebsite.Text = "GroestlCoin Website"
        '
        'gbStatus
        '
        Me.gbStatus.Controls.Add(Me.ProgressBar)
        Me.gbStatus.Controls.Add(Me.txtOutput)
        Me.gbStatus.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbStatus.Location = New System.Drawing.Point(23, 362)
        Me.gbStatus.Name = "gbStatus"
        Me.gbStatus.Size = New System.Drawing.Size(628, 91)
        Me.gbStatus.TabIndex = 7
        Me.gbStatus.TabStop = False
        Me.gbStatus.Text = "Mining Status"
        '
        'ProgressBar
        '
        Me.ProgressBar.ForeColor = System.Drawing.Color.RoyalBlue
        Me.ProgressBar.Location = New System.Drawing.Point(13, 73)
        Me.ProgressBar.MarqueeAnimationSpeed = 20
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(603, 10)
        Me.ProgressBar.Step = 100
        Me.ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.ProgressBar.TabIndex = 1
        Me.ProgressBar.Visible = False
        '
        'txtOutput
        '
        Me.txtOutput.Font = New System.Drawing.Font("Georgia", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtOutput.Location = New System.Drawing.Point(13, 18)
        Me.txtOutput.Multiline = True
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.Size = New System.Drawing.Size(603, 47)
        Me.txtOutput.TabIndex = 0
        '
        'Timer1
        '
        Me.Timer1.Interval = 500
        '
        'rbUsedwarfPool
        '
        Me.rbUsedwarfPool.AutoSize = True
        Me.rbUsedwarfPool.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbUsedwarfPool.Location = New System.Drawing.Point(9, 21)
        Me.rbUsedwarfPool.Name = "rbUsedwarfPool"
        Me.rbUsedwarfPool.Size = New System.Drawing.Size(172, 20)
        Me.rbUsedwarfPool.TabIndex = 8
        Me.rbUsedwarfPool.TabStop = True
        Me.rbUsedwarfPool.Text = "Use DwarfPool (default)"
        Me.rbUsedwarfPool.UseVisualStyleBackColor = True
        '
        'rbCustomPool
        '
        Me.rbCustomPool.AutoSize = True
        Me.rbCustomPool.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbCustomPool.Location = New System.Drawing.Point(10, 46)
        Me.rbCustomPool.Name = "rbCustomPool"
        Me.rbCustomPool.Size = New System.Drawing.Size(104, 20)
        Me.rbCustomPool.TabIndex = 9
        Me.rbCustomPool.TabStop = True
        Me.rbCustomPool.Text = "Custom Pool"
        Me.rbCustomPool.UseVisualStyleBackColor = True
        '
        'txtPool
        '
        Me.txtPool.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPool.Location = New System.Drawing.Point(201, 44)
        Me.txtPool.Name = "txtPool"
        Me.txtPool.Size = New System.Drawing.Size(288, 20)
        Me.txtPool.TabIndex = 10
        '
        'gbMiningPool
        '
        Me.gbMiningPool.Controls.Add(Me.lblCustomAddress)
        Me.gbMiningPool.Controls.Add(Me.txtPassword)
        Me.gbMiningPool.Controls.Add(Me.lblPassword)
        Me.gbMiningPool.Controls.Add(Me.lblUsername)
        Me.gbMiningPool.Controls.Add(Me.txtUsername)
        Me.gbMiningPool.Controls.Add(Me.txtPool)
        Me.gbMiningPool.Controls.Add(Me.rbCustomPool)
        Me.gbMiningPool.Controls.Add(Me.rbUsedwarfPool)
        Me.gbMiningPool.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbMiningPool.Location = New System.Drawing.Point(23, 251)
        Me.gbMiningPool.Name = "gbMiningPool"
        Me.gbMiningPool.Size = New System.Drawing.Size(627, 105)
        Me.gbMiningPool.TabIndex = 2
        Me.gbMiningPool.TabStop = False
        Me.gbMiningPool.Text = "Mining Pool"
        '
        'lblCustomAddress
        '
        Me.lblCustomAddress.AutoSize = True
        Me.lblCustomAddress.Location = New System.Drawing.Point(132, 48)
        Me.lblCustomAddress.Name = "lblCustomAddress"
        Me.lblCustomAddress.Size = New System.Drawing.Size(63, 16)
        Me.lblCustomAddress.TabIndex = 13
        Me.lblCustomAddress.Text = "Address:"
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(389, 70)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(100, 22)
        Me.txtPassword.TabIndex = 8
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Location = New System.Drawing.Point(314, 73)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(71, 16)
        Me.lblPassword.TabIndex = 12
        Me.lblPassword.Text = "Password:"
        '
        'lblUsername
        '
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Location = New System.Drawing.Point(121, 73)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Size = New System.Drawing.Size(75, 16)
        Me.lblUsername.TabIndex = 11
        Me.lblUsername.Text = "Username:"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(201, 70)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(100, 22)
        Me.txtUsername.TabIndex = 8
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "Groestl EasyMiner"
        Me.NotifyIcon1.Visible = True
        '
        'txtthreads
        '
        Me.txtthreads.Font = New System.Drawing.Font("Georgia", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtthreads.Location = New System.Drawing.Point(310, 44)
        Me.txtthreads.Name = "txtthreads"
        Me.txtthreads.Size = New System.Drawing.Size(40, 20)
        Me.txtthreads.TabIndex = 11
        '
        'rbGroestl
        '
        Me.rbGroestl.AutoSize = True
        Me.rbGroestl.Location = New System.Drawing.Point(16, 21)
        Me.rbGroestl.Name = "rbGroestl"
        Me.rbGroestl.Size = New System.Drawing.Size(98, 20)
        Me.rbGroestl.TabIndex = 6
        Me.rbGroestl.TabStop = True
        Me.rbGroestl.Text = "Groestlcoin"
        Me.rbGroestl.UseVisualStyleBackColor = True
        Me.rbGroestl.UseWaitCursor = True
        '
        'gbMethod
        '
        Me.gbMethod.Controls.Add(Me.rbGroestl)
        Me.gbMethod.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbMethod.Location = New System.Drawing.Point(200, 111)
        Me.gbMethod.Name = "gbMethod"
        Me.gbMethod.Size = New System.Drawing.Size(250, 134)
        Me.gbMethod.TabIndex = 4
        Me.gbMethod.TabStop = False
        Me.gbMethod.Text = "Mining Method"
        Me.gbMethod.UseWaitCursor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(672, 508)
        Me.Controls.Add(Me.gbWallet)
        Me.Controls.Add(Me.gbMiningPool)
        Me.Controls.Add(Me.gbStatus)
        Me.Controls.Add(Me.llWebsite)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.pbGroestl)
        Me.Controls.Add(Me.gbMethod)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "GroestlCoin EasyMiner"
        CType(Me.pbGroestl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbWallet.ResumeLayout(False)
        Me.gbWallet.PerformLayout()
        Me.gbStatus.ResumeLayout(False)
        Me.gbStatus.PerformLayout()
        Me.gbMiningPool.ResumeLayout(False)
        Me.gbMiningPool.PerformLayout()
        Me.gbMethod.ResumeLayout(False)
        Me.gbMethod.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pbGroestl As System.Windows.Forms.PictureBox
    Friend WithEvents gbWallet As System.Windows.Forms.GroupBox
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents llWebsite As System.Windows.Forms.LinkLabel
    ' Friend WithEvents rbQubit As System.Windows.Forms.RadioButton
    ' Friend WithEvents rbSkein As System.Windows.Forms.RadioButton
    Friend WithEvents gbStatus As System.Windows.Forms.GroupBox
    Friend WithEvents txtOutput As System.Windows.Forms.TextBox
    Friend WithEvents txtAddress As System.Windows.Forms.TextBox
    Friend WithEvents lblAddress As System.Windows.Forms.Label
    Friend WithEvents btnWalletSetup As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents rbCustomAddress As System.Windows.Forms.RadioButton
    Friend WithEvents rbUseElectrum As System.Windows.Forms.RadioButton
    Friend WithEvents rbUsedwarfPool As System.Windows.Forms.RadioButton
    Friend WithEvents rbCustomPool As System.Windows.Forms.RadioButton
    Friend WithEvents txtPool As System.Windows.Forms.TextBox
    Friend WithEvents gbMiningPool As System.Windows.Forms.GroupBox
    Friend WithEvents lblCustomAddress As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents txtUsername As System.Windows.Forms.TextBox
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents txtthreads As System.Windows.Forms.TextBox
    Friend WithEvents rbGroestl As RadioButton
    Friend WithEvents gbMethod As GroupBox
    Friend WithEvents btnOpenHelp As Button
End Class
