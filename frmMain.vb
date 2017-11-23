Imports System.Environment
Imports System.IO

Public Class frmMain

    Private Const PROGRAM_TITLE As String = "GroestlCoin EasyMiner"
    Private _newProcess As Process
    Private _SelectedMiningMethod As enumMiningMethod
    Private strModule As String = "frmMain: "

    Private Enum enumMiningMethod
        GroestlCPU32
        GroestlCPU64
        GroestlGPU
        GroestlNVGPU
    End Enum

    Private Enum enumRegistryValue
        CustomAddress
        CustomUsername
        CustomPassword
        CustomMiningPool
        CustomSelectedAlgorithm
    End Enum

    Public Property Result As DialogResult

    Private ReadOnly Property GetMiningBatch As String
        Get
            Return Application.StartupPath & "\scripts\StartMining.bat"
        End Get
    End Property

    Private ReadOnly Property GetMiningEXE As String
        Get
            Dim strExe As String = GetPathToMinerEXE
            For I As Integer = strExe.Length - 1 To 0 Step -1
                If strExe.ElementAt(I) = "\" Then
                    strExe = strExe.Substring(I, strExe.Length - I).Replace("\", "")
                    Exit For
                End If
            Next
            Return strExe
        End Get
    End Property

    Private ReadOnly Property GetPathToElectrum As String
        Get
            Return Application.StartupPath & "\Prerequisites\Electrum-GRS\Electrum-grs.exe"
        End Get
    End Property

    Private ReadOnly Property GetPathToMinerEXE() As String
        Get
            Dim strOutput As String = String.Empty
            strOutput = Application.StartupPath & "\Scripts\"

            Select Case _SelectedMiningMethod
                'GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL GROESTL
                Case enumMiningMethod.GroestlCPU64, enumMiningMethod.GroestlCPU32, enumMiningMethod.GroestlGPU
                    strOutput &= "groestl\"

                    If HasOpenCl = False Then

                        If Is64BitOperatingSystem Then
                            _SelectedMiningMethod = (enumMiningMethod.GroestlCPU64)
                        Else
                            _SelectedMiningMethod = (enumMiningMethod.GroestlCPU32)
                        End If

                        If _SelectedMiningMethod = enumMiningMethod.GroestlCPU64 Then
                            strOutput &= "GroestlCPU64.exe"
                        End If
                        If _SelectedMiningMethod = enumMiningMethod.GroestlCPU32 Then
                            strOutput &= "Groestl32\GroestlCPU32.exe"

                        End If
                    End If
                    If NVidiaCheck = True Then
                        _SelectedMiningMethod = enumMiningMethod.GroestlNVGPU
                        strOutput &= "GroestlGPU\GroestlNVGPU.exe"
                    End If

                    If HasOpenCl = True And NVidiaCheck = False Then
                        _SelectedMiningMethod = enumMiningMethod.GroestlGPU
                        strOutput &= "GroestlGPU\GroestlGPU.exe"
                    End If
            End Select

            Return strOutput
        End Get
    End Property

    Private ReadOnly Property HasOpenCl As Boolean
        Get
            Return My.Computer.FileSystem.FileExists(Environment.SystemDirectory & "\OpenCL.DLL")
        End Get
    End Property

    Private ReadOnly Property IsWalletSetup As Boolean
        Get
            Return My.Computer.FileSystem.DirectoryExists(WalletFolder) And WalletAddress.Length > 0
        End Get
    End Property

    Private ReadOnly Property LogFileLocation As String
        Get
            Return Application.StartupPath & "\scripts\minelog.txt"
        End Get
    End Property

    Private ReadOnly Property NVidiaCheck As Boolean
        Get
            Return My.Computer.FileSystem.FileExists(Environment.SystemDirectory & "\NVCuda.DLL")
        End Get
    End Property

    Private ReadOnly Property WalletAddress As String

        Get
            If rbUseElectrum.Checked Then
                Dim strFileName As String = ".\Address.conf"
                If System.IO.File.Exists(strFileName) = True Then

                    'Copied similar hack from vbs files to grab the first address in the file.
                    Dim objReader As New System.IO.StreamReader(strFileName)
                    Dim strWalletAddress As String = objReader.ReadLine()
                    objReader.Close()
                    Return strWalletAddress
                Else
                    MessageBox.Show("Electrum Wallet Not found.  Make sure you have run the inital setup.", PROGRAM_TITLE)
                    Return String.Empty
                End If
            Else 'Custom Address
                Return txtAddress.Text
            End If
        End Get
    End Property

    Private ReadOnly Property WalletFolder As String
        Get
            Return GetFolderPath(SpecialFolder.ApplicationData) & "\Electrum-GRS"
        End Get
    End Property

    Private Sub Address_CheckedChanged(sender As Object, e As EventArgs) Handles rbCustomAddress.CheckedChanged, rbUseElectrum.CheckedChanged
        Dim strLocation As String = "Address_CheckedChanged"
        Try
            If rbUseElectrum.Checked Then
                'check wallet is setup
                Process.Start(".\address.vbs")
            End If

            While WalletAddress = ""
                Threading.Thread.Sleep(500)
            End While
            If IsWalletSetup = True Then
                txtAddress.Text = WalletAddress
                txtAddress.ReadOnly = True
            End If

            If rbCustomAddress.Checked Then
                txtAddress.ReadOnly = False
                txtAddress.Text = GetSetting("Easyminer", "Settings", "CustomAddress")
                txtUsername.Text = GetSetting("Easyminer", "Settings", "CustomUsername")
                txtPassword.Text = GetSetting("Easyminer", "Settings", "CustomPassword")

            End If
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub btn_Click(sender As Object, e As EventArgs) Handles btnStart.Click, btnStop.Click
        Dim strLocation As String = "btn_Click"
        Try
            Dim btn As Button = CType(sender, Button)
            Select Case btn.Name
                Case "btnStart"
                    If rbCustomPool.Checked And txtPool.Text.Length < 1 Then
                        MessageBox.Show("You must enter a custom pool address to continue.", PROGRAM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop)
                        Exit Sub
                    End If
                    btnStart.Enabled = False
                    btnStop.Enabled = True
                    DisableEnableControls(False)
                    If rbCustomAddress.Checked Then
                        WriteToRegistry(enumRegistryValue.CustomAddress)
                    Else
                        If GetSetting("Easyminer", "Settings", "CustomAddress").Length > 0 Then
                            DeleteSetting("Easyminer", "Settings", "CustomAddress")
                        End If
                    End If

                    If rbCustomPool.Checked Then
                        WriteToRegistry(enumRegistryValue.CustomMiningPool)
                        WriteToRegistry(enumRegistryValue.CustomUsername)
                        WriteToRegistry(enumRegistryValue.CustomPassword)
                        WriteToRegistry(enumRegistryValue.CustomSelectedAlgorithm)
                    Else
                        If GetSetting("Easyminer", "Settings", "CustomMiningPool").Length > 0 Then
                            DeleteSetting("Easyminer", "Settings", "CustomMiningPool")
                            DeleteSetting("Easyminer", "Settings", "CustomUsername")
                            DeleteSetting("Easyminer", "Settings", "CustomPassword")
                            DeleteSetting("Easyminer", "Settings", "CustomSelectedAlgorithm")
                        End If
                    End If

                    txtOutput.Text &= vbCrLf & "Mining Started at " & Now

                    If rbGroestl.Checked Then
                        Select Case Result

                            Case DialogResult.Yes 'Mine CPU
                                If Is64BitOperatingSystem Then

                                    _SelectedMiningMethod = (enumMiningMethod.GroestlCPU64)
                                Else
                                    _SelectedMiningMethod = (enumMiningMethod.GroestlCPU32)
                                End If
                            Case DialogResult.No 'Mine GPU
                                _SelectedMiningMethod = enumMiningMethod.GroestlGPU

                        End Select
                    End If
                    WriteBatchFile()
                    StartMiningProcess()
                    Timer1.Start()
                    ProgressBar.Visible = True
                Case "btnStop"
                    btnStart.Enabled = True
                    btnStop.Enabled = False
                    txtOutput.Text &= vbCrLf & "Mining Stopped at " & Now
                    If _newProcess IsNot Nothing Then
                        _newProcess.Close()
                    End If

                    KillMiner()
                    Timer1.Stop()
                    ProgressBar.Visible = False
                    DisableEnableControls(True)
            End Select
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub btnOpenHelp_Click(sender As Object, e As EventArgs) Handles btnOpenHelp.Click
        Dim strLocation As String = "btnOpenHelp_Click"
        Try
            Process.Start(".\help.html")
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub btnWalletSetup_Click(sender As Object, e As EventArgs) Handles btnWalletSetup.Click
        Dim strLocation As String = "btnWalletSetup_Click"
        Try
            If Not My.Computer.FileSystem.FileExists(GetPathToElectrum) Then
                MessageBox.Show("Electrum was not found.", PROGRAM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Else
                Process.Start(GetPathToElectrum)
            End If
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub DisableEnableControls(ByVal blnEnabled As Boolean)
        Dim strLocation As String = strModule & ":" & "DisableEnableControls"
        Try
            gbMethod.Enabled = blnEnabled
            gbMiningPool.Enabled = blnEnabled
            gbWallet.Enabled = blnEnabled
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        KillMiner()
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim strLocation As String = "frmMain_Load"
        Try
            MaximizeBox = False
            rbGroestl.Checked = True

            'get registry setting from last time.  Are we using a cutom address or the electrum wallet
            Dim strRegAddress As String = GetSetting("Easyminer", "Settings", "CustomAddress")
            Dim strRegUsername As String = GetSetting("Easyminer", "Settings", "CustomUsername")
            Dim strRegPassword As String = GetSetting("Easyminer", "Settings", "CustomPassword")
            If strRegAddress.Length > 0 Then 'saved a custom address previously
                rbCustomAddress.Checked = True
                txtAddress.Text = strRegAddress
            Else
                rbUseElectrum.Checked = True
            End If

            Dim strRegPool As String = GetSetting("Easyminer", "Settings", "CustomMiningPool")
            If strRegPool.Length > 0 Then
                rbCustomPool.Checked = True
                txtPool.Text = strRegPool
                txtUsername.Text = strRegUsername
                txtPassword.Text = strRegPassword
            Else
                rbUsedwarfPool.Checked = True
            End If
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub frmMain_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Dim strLocation As String = "frmMain_Resize"
        Try
            If Me.WindowState = FormWindowState.Minimized Then
                NotifyIcon1.Visible = True
                Me.Hide()
                NotifyIcon1.BalloonTipText = PROGRAM_TITLE
                NotifyIcon1.ShowBalloonTip(500)
            End If
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub KillMiner()
        For Each p As Process In System.Diagnostics.Process.GetProcessesByName(GetMiningEXE.Replace(".exe", ""))
            Try
                p.Kill()
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Sub llWebsite_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles llWebsite.LinkClicked
        Dim strLocation As String = "llWebsite_LinkClicked"
        Try
            System.Diagnostics.Process.Start(".\websiteredir.html")
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.DoubleClick, NotifyIcon1.Click
        Dim strLocation As String = "NotifyIcon1_Click"
        Try
            Me.Show()
            Me.WindowState = FormWindowState.Normal
            NotifyIcon1.Visible = False
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub Pool_CheckedChanged(sender As Object, e As EventArgs) Handles rbUsedwarfPool.CheckedChanged, rbCustomPool.CheckedChanged
        Dim strLocation As String = "Pool_CheckedChanged"
        Try
            If rbUsedwarfPool.Checked Then
                txtPool.Text = ""
                txtUsername.Text = ""
                txtPassword.Text = ""
                txtPool.ReadOnly = True
                txtUsername.ReadOnly = True
                txtPassword.ReadOnly = True
            Else
                txtPool.Text = GetSetting("Easyminer", "Settings", "CustomMiningPool")
                txtUsername.Text = GetSetting("Easyminer", "Settings", "CustomUsername")
                txtPassword.Text = GetSetting("Easyminer", "Settings", "CustomPassword")
                Dim iCustomSelectedAlgorith As String = GetSetting("Easyminer", "Settings", "CustomSelectedAlgorithm")
                If iCustomSelectedAlgorith.Length = 0 Then iCustomSelectedAlgorith = 0
                Select Case iCustomSelectedAlgorith
                    '                    Case 0
                    '                    rbSkein.Checked = True
                    'Case 1
                    '   rbQubit.Checked = True
                    Case 2
                        rbGroestl.Checked = True
                        ' Case 3
                        '    rbScrypt.Checked = True
                End Select
                txtPool.ReadOnly = False
                txtUsername.ReadOnly = False
                txtPassword.ReadOnly = False
            End If
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub StartMiningProcess()
        Dim strLocation As String = "StartMiningProcess"
        Try
            'Just in case make sure there isn't another miner running from a crash or something else
            KillMiner()
            'here goes nothing....
            'load the dynamically created batch file as a process
            'I had to do it this way because for whatever reason when you try to log the output from the command window
            'you cannot log to file via the arguments using 2>file.txt
            'it never logs to the file when the process is started this way, permissions when creating a process?
            'but if you put everything you need into a batch and then start it, it works fine.
            _newProcess = New Process
            With _newProcess
                If System.IO.File.Exists(GetMiningBatch) Then
                    'TODO:Threads portion?
                    .StartInfo.FileName = GetMiningBatch
                    .StartInfo.UseShellExecute = False
                    .StartInfo.CreateNoWindow = True

                    If HasOpenCl = True Then
                        .StartInfo.RedirectStandardOutput = False
                    Else
                        .StartInfo.RedirectStandardOutput = True
                    End If

                    .StartInfo.RedirectStandardError = True
                    .Start()
                    'uncomment for debugging of command line processes
                    'freezes up if leave this in so..just for debugging.
                    'Dim strOutputError As String = _newProcess.StandardError.ReadToEnd
                    'Dim strOutput As String = _newProcess.StandardOutput.ReadToEnd
                    'Dim setbreakpointhere As String = ""
                Else
                    Throw New Exception(" StartMining.bat not found!")
                End If
            End With
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim strLocation As String = "Timer1_Tick"
        Try
            UpdateOutputWindow()
            Timer1.Start()
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub
    Private Sub UpdateOutputWindow()
        Dim strLocation As String = "UpdateOutputWindow"
        Try
            'So here is the issue..
            'the miner writes to the log file forever so it can get super big with log entries.
            'there is no way in hell we want to open the file and read line by line until we get to the last line
            'solution - open the file and binary read starting from the end of the file
            'I've got to think this would be faster and not tend to freeze things up.
            'This method is called via the timer.

            Dim fsLogFile As FileStream = New FileStream(LogFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Dim buffer(256) As Char 'assume a line is not more than 256 chars?

            Dim brReader As New IO.BinaryReader(fsLogFile)
            If fsLogFile.Length < 1 Then
                fsLogFile.Close()
                Exit Sub
            End If

            Dim iFound As Integer = 0
            For i As Integer = 1 To 1000
                If i > fsLogFile.Length Then Exit For
                fsLogFile.Position = fsLogFile.Length - i
                brReader = New IO.BinaryReader(fsLogFile)
                If brReader.ReadChars(1) = "[" Then
                    iFound = i
                    Exit For
                End If
            Next
            fsLogFile.Position = fsLogFile.Length - iFound
            brReader = New IO.BinaryReader(fsLogFile)
            brReader.Read(buffer, 0, 256)
            Dim strLastLine As New String(buffer)
            If Not txtOutput.Text.Contains(strLastLine) Then
                txtOutput.Text = strLastLine
            End If

            fsLogFile.Close()
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub WriteBatchFile()
        Dim strLocation As String = "StartMiningProcess"
        Try
            Dim strFileLine As String = String.Empty
            strFileLine &= """" & GetPathToMinerEXE & """"

            '        'TODO Threads always 1 aparently?
            Dim strAlgo As String = ""
            If _SelectedMiningMethod.ToString.ToUpper.Contains("GROESTL") Then

                If _SelectedMiningMethod = enumMiningMethod.GroestlCPU32 Or _SelectedMiningMethod = enumMiningMethod.GroestlCPU64 Or _SelectedMiningMethod = enumMiningMethod.GroestlNVGPU Then
                    strAlgo = "-a groestl"
                End If
                If _SelectedMiningMethod = enumMiningMethod.GroestlGPU Then
                    strAlgo = "-k groestlcoin -I d"

                End If
            End If
            'Set proper port for mining method
            If rbUsedwarfPool.Checked Then
                Dim strPort As String = String.Empty
                Select Case _SelectedMiningMethod
                    Case enumMiningMethod.GroestlCPU64, enumMiningMethod.GroestlCPU32, enumMiningMethod.GroestlGPU, enumMiningMethod.GroestlNVGPU
                        strPort = "3345"
                End Select

                strFileLine &= String.Format(" {1} -o stratum+tcp://moria.dwarfpool.com:{0} -u " & WalletAddress & " -p x ", strPort, strAlgo)
            Else
                strFileLine &= String.Format(" {2} -o stratum+tcp://{0} -u " & txtUsername.Text & " -p {1}", txtPool.Text, txtPassword.Text, strAlgo)
            End If

            'Lazy removal - gpu has no threads
            If _SelectedMiningMethod = enumMiningMethod.GroestlGPU Or _SelectedMiningMethod = enumMiningMethod.GroestlNVGPU Then
                strFileLine = strFileLine.Replace(" --threads 1", "")
            End If

            'Always
            strFileLine &= " 2>""" & LogFileLocation & """"

            Using writer As StreamWriter = New StreamWriter(GetMiningBatch)
                writer.Write(strFileLine)
                writer.Close() 'added this
            End Using
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

    Private Sub WriteToRegistry(ByVal enumRegValue As enumRegistryValue)
        Dim strLocation As String = "WriteToRegistry"
        Try
            Select Case enumRegValue
                Case enumRegistryValue.CustomAddress
                    SaveSetting("Easyminer", "Settings", "CustomAddress", txtAddress.Text)
                Case enumRegistryValue.CustomMiningPool
                    SaveSetting("Easyminer", "Settings", "CustomMiningPool", txtPool.Text)
                Case enumRegistryValue.CustomMiningPool
                    SaveSetting("Easyminer", "Settings", "numberofthreads", txtthreads.Text)
                Case enumRegistryValue.CustomUsername
                    SaveSetting("Easyminer", "Settings", "CustomUsername", txtUsername.Text)
                Case enumRegistryValue.CustomPassword
                    SaveSetting("Easyminer", "Settings", "CustomPassword", txtPassword.Text)
                Case enumRegistryValue.CustomSelectedAlgorithm
                    Dim iSelectedAlgorithm As Integer = 0
                    '                    If rbSkein.Checked Then
                    '                    iSelectedAlgorithm = 0
                    '                   End If
                    '                  If rbQubit.Checked Then
                    '                  iSelectedAlgorithm = 1
                    '                 End If
                    If rbGroestl.Checked Then
                        iSelectedAlgorithm = 2
                    End If
                    '                If rbScrypt.Checked Then
                    '                iSelectedAlgorithm = 3
                    '               End If

                    SaveSetting("Easyminer", "Settings", "CustomSelectedAlgorithm", iSelectedAlgorithm)
            End Select
        Catch ex As Exception
            BuildErrorMessage(strModule, strLocation, ex.Message)
        End Try
    End Sub

End Class