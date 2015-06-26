Option Explicit
on error resume next

'Declarations
Dim ObjShell
Dim OWSH
Dim WalletAdr 'Self Explanitory
Dim StrFileName
Dim Filesys
Dim objFSO
Dim ObjFSO2 ' Avoiding redefinition
Dim Appdata2 ' Appdata is s system variable so it doesn't like anything else /sad
Dim strfolderpath2 'Documentation check path
Dim ObjFile 
Dim wshSystemEnv
Dim i ' File check variable
Dim Arch
Dim ArchPath
Dim Windir2
Dim OCLPath 'Where is OpenCL Located?
Dim OCL
Dim R 'Value entered for menu
Dim Q 'Quit variable
Dim R2 ' Second Menu Variable
Dim strMenu
Dim rc
Dim Cuda
Dim MSVCRx86
Dim CudaPath
Dim MSVCRx86path
Dim Electrumpresent
dim objWMIService 
Dim objItem 
Dim colItems 
Dim strComputer
Dim compModel
Dim strKeyPath
Dim strValueName
Dim strValue
Dim oReg
Dim AMD
Dim Threads
Dim CPU
Dim ThreadsPath
Dim CPUPath
Dim CPUMine
dim rc2
dim test
dim intanswer
dim objFileToWrite
dim S ' Yet another sleep variable
dim objFileToRead
dim strnumcpu 'number of host threads
dim intnumcpu
dim finalnumcpu
dim hidden
dim hiddenpath
dim inthidden
dim R3
dim SkeinPool
dim QubitPool
dim GroestlPool
dim scryptpool
dim ScryptPath
dim groestlpath
dim skeinpath
dim qubitpath
dim Strfolderpath3
dim User2
dim addressloc
dim coinutilinput
dim coinutiloutput
dim addresscheck
dim cmdline
dim cmdlineloc
dim firstrun
dim firstrunloc

Set Filesys = CreateObject("Scripting.FileSystemObject") 
Set objFSO = CreateObject("Scripting.FileSystemObject") 
Set objShell = CreateObject("Wscript.Shell")
Set wshSystemEnv = objShell.Environment("SYSTEM")

objShell.CurrentDirectory = ".\Scripts" ' Change working directory 

ThreadsPath = ".\Threads.conf"
CPUPath = ".\CPUMine.conf"
HiddenPath = ".\Hidden.conf"
addressloc = ".\Address.conf"
cmdlineloc = ".\cmdline.bat"
firstrunloc = ".\firstrun.conf"

'Begin Documentation Run Check
Appdata2=objShell.ExpandEnvironmentStrings("%Appdata%")
Strfolderpath2 = Appdata2 & "\Electrum-grs" 
User2=objShell.ExpandEnvironmentStrings("%USERPROFILE%")
Strfolderpath3 = User2 & "\Desktop" 
Set objShell = CreateObject("Wscript.Shell")
 If Not objFSO.FileExists(User2 & "\desktop" & "\Electrum-grsWallet.exe") then
 objFSO.CopyFile "Electrum-grsWallet.exe", (User2 & "\desktop\")
End If
 If Not objFSO.FolderExists(strfolderpath2) then
  Electrumpresent =0
  CreateObject("WScript.Shell").Run "http://myriadplatform.org/mining-setup/"
  CreateObject("WScript.Shell").Run ".\Electrum-grsWallet.exe"
  wscript.echo "Please re-open Easyminer when electrum setup is complete"
  wscript.quit
'One time only, Set Threads VAR to 1 less core than host system maximum to try to keep system fluidity
 End If

strnumcpu=objShell.ExpandEnvironmentStrings("%NUMBER_OF_PROCESSORS%")
intnumcpu=cint(strnumcpu)
Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(ThreadsPath,2,true)
finalnumcpu=intnumcpu-1
objFileToWrite.WriteLine(finalnumcpu)
objFileToWrite.Close

set objShell = CreateObject("Wscript.Shell")
 If objFSO.FolderExists(strfolderpath2) then
  Electrumpresent =1
End if
'End Documentation Check

'Are we on X86 or X64?
ArchPath= "C:\Program Files (X86)"
If objFSO.FolderExists(ArchPath) then
Arch="X64"
End If

If Not objFSO.FolderExists(ArchPath) then
Arch="X86"
End If

'Open CL Check
Windir2=objShell.ExpandEnvironmentStrings("%WinDir%")
OCLPath= Windir2 & "\System32\OpenCL.DLL"
If objFSO.FileExists(OCLPath) then
OCL=1
End If
If Not objFSO.fileExists(OCLPath) then
OCL=0
End If

'Nvidia Check
Windir2=objShell.ExpandEnvironmentStrings("%WinDir%")
CudaPath= Windir2 & "\System32\NVCuda.DLL"
If objFSO.FileExists(CudaPath) then
Cuda=1
End If
If Not objFSO.fileExists(CudaPath) then
Cuda=0
End If

'Visual Studio 2010 runtime check x86

Windir2=objShell.ExpandEnvironmentStrings("%WinDir%")
MSVCRx86Path= Windir2 & "\System32\MSVCR100.DLL"
If objFSO.FileExists(MSVCRx86path) then
MSVCRx86=1
Else MSVCRx86=0
End If

' AMD or Intel Check
const HKEY_LOCAL_MACHINE = &H80000002
strKeyPath = "SYSTEM\ControlSet001\Services\intelppm"
strValueName = "Start"
strValue = "1"
strComputer = "."
' WMI connection to Root CIM and get the computer type
set objWMIService = GetObject("winmgmts:\\" _
& strComputer & "\root\cimv2")
set colItems = objWMIService.ExecQuery(_
"Select Manufacturer from Win32_Processor")
'Loop through the results and store the type in compModel
for each objItem in colItems
compModel = objItem.Manufacturer
next
'Get a registry object
Set oReg=GetObject("winmgmts:{impersonationLevel=impersonate}!\\" & strComputer & "\root\default:StdRegProv")
'Check the computer type. If the processor is an Intel, then re-enable the driver
if compModel = "GenuineIntel" then
AMD=0
else
AMD=1
end if


'Did we create the folder but not the wallet aka did our setup get borked, WE MAKE DAMN SURE THE WALLET EXISTS
If Not objFSO.fileExists(strfolderpath2 & "\wallets\default_wallet") then
i=0
elseif objFSO.fileExists(strfolderpath2 & "\wallets\default_wallet") then
i=1
end if
If i=0 and Electrumpresent=1 then
CreateObject("WScript.Shell").Run "Electrum-grsWallet.exe"
end if


'loop
Do until i=1
If Not objFSO.fileExists(strfolderpath2 & "\wallets\default_wallet") then
i=0
elseif objFSO.fileExists(strfolderpath2 & "\wallets\default_wallet") then
i=1
end if
loop

'Parse Wallet with a horrendously coded hack
If i=1 then
StrFileName = Appdata2 & "\Electrum-grs\wallets\default_wallet"
Set objFile = objFSO.OpenTextFile(StrFileName) 
Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
ObjFile.SkipLine
ObjFile.SkipLine 
ObjFile.SkipLine 
ObjFile.SkipLine
ObjFile.SkipLine 
ObjFile.SkipLine 
ObjFile.SkipLine 
ObjFile.SkipLine 
ObjFile.SkipLine 
ObjFile.Skip(17)  
WalletADR = ObjFile.Read(66)
End If

'Menu code
strMenu="Please type Enter your selection (1-6)" & VbCrLf &_
 "1 Mine Groestl"  & VbCrLf &_
 "2 Help (Opens help Website)" & VbCrLf &_
 "3 Advanced (Opens power user options)"
 
 rc=InputBox(strMenu,"Menu",1)
 If IsNumeric(rc) Then
     Select Case rc
         Case 1
			 R=3
   		Case 2
		 R=5 
		 Case 3
		 R=6
     End Select
 End If
 
'Do a Barrel Roll straight into our Miner script

'Poweruser Menu code

If R=6 Then

strMenu="Please type Enter your selection (1-6)" & VbCrLf &_
 "1 Open CPU Miner?" & VbCrLf &_
 "2 Number of CPU Threads you wish to use" & VbCrLf &_
 "3 Set up merged mining" & VbCrLf &_
 "4 Launch Hidden?" & VbCrLf &_
 "5 Kill Silent Miners" & VbCrLf &_
 "6 Change Pool"

 rc=InputBox(strMenu,"Menu",1)
 If IsNumeric(rc2) Then
     Select Case rc
         Case 1 
             R2=1
         Case 2
             R2=2
	     Case 3
		     R2=4
	     Case 4
		     R2=5
		 Case 5
			 R2=6
     End Select
 End If
End if 
 'Begin Poweruser Menu Actions
 
 If R2=1 Then
intAnswer = _
    Msgbox("Open the CPU Miner?", _
        vbYesNo, "Mine on CPU?")
		
	End If

If intAnswer = vbYes Then
CPUMine=1
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(CPUPath,2,true)
objFileToWrite.WriteLine(CPUMine)
objFileToWrite.Close
Set objFileToWrite = Nothing
wscript.echo("Please restart the script")
End IF

If intAnswer = vbNo Then
CPUMine=0
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(CPUPath,2,true)
objFileToWrite.WriteLine(CPUMine)
objFileToWrite.Close
Set objFileToWrite = Nothing
wscript.echo("Please restart the script")
End If

If R2=2 Then

Threads = InputBox("Please enter the # of CPU threads to use", _
    "CPU threads")
	
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(ThreadsPath,2,true)
objFileToWrite.WriteLine(Threads)
objFileToWrite.Close

End if

If R2=3 Then
wscript.echo("Launching instructions to set up merged mining")
CreateObject("WScript.Shell").Run "http://grsiad.p2pool.geek.nz/merge"
End If

If R2=4 Then
intHidden = _
    Msgbox("Launch Hidden?", _
        vbYesNo, "Yes")
		
	End If

If intHidden = vbYes Then
Hidden=1
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(HiddenPath,2,true)
objFileToWrite.WriteLine(Hidden)
objFileToWrite.Close
Set objFileToWrite = Nothing
wscript.echo("Please restart the script")
End IF

If intHidden = vbNo Then
Hidden=0	
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(HiddenPath,2,true)
objFileToWrite.WriteLine(Hidden)
objFileToWrite.Close
Set objFileToWrite = Nothing
wscript.echo("Please restart the script")
End If

If R2=5 Then
objshell.Run ".\KillSilent.bat"
Wscript.quit
End If

If R2=6 Then

strMenu="Select which pool to change" & VbCrLf &_
 "1 Groestl"


 rc=InputBox(strMenu,"Menu",1)
 If IsNumeric(rc2) Then
     Select Case rc
		 Case 1
			 R3=3
     End Select
 End If
 End If
 
 If R3=1 Then
 SkeinPool = InputBox("Enter your Skein pool - URL ONLY", _
    "Skein Pool")
	
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(SkeinPath,2,true)
objFileToWrite.WriteLine(Skeinpool)
objFileToWrite.Close
End If

 If R3=2 Then
 SkeinPool = InputBox("Enter your Qubit pool - URL ONLY", _
    "Qubit Pool")
	
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(SkeinPath,2,true)
objFileToWrite.WriteLine(Qubitpool)
objFileToWrite.Close
End if
 
  If R3=3 Then
 SkeinPool = InputBox("Enter your Groestl pool - URL ONLY", _
    "Groestl Pool")
	
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(SkeinPath,2,true)
objFileToWrite.WriteLine(Groestlpool)
objFileToWrite.Close
End If

 If R3=4 Then
 SkeinPool = InputBox("Enter your Scrypt pool - URL ONLY", _
    "Scrypt Pool")
	
	Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(SkeinPath,2,true)
objFileToWrite.WriteLine(Scryptpool)
objFileToWrite.Close
 End If
 
'Read Variables from files

'Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\Skein.conf",1)
'Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
'SkeinPool = objFileToRead.ReadAll()
'objFileToRead.Close
'Set objFileToRead = Nothing

'Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\Qubit.conf",1)
'Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
'QubitPool = objFileToRead.ReadAll()
'objFileToRead.Close
'Set objFileToRead = Nothing

Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\Groestl.conf",1)
Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
GroestlPool = objFileToRead.ReadAll()
objFileToRead.Close
Set objFileToRead = Nothing

'Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\Scrypt.conf",1)
'Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
'ScryptPool = objFileToRead.ReadAll()
'objFileToRead.Close
'Set objFileToRead = Nothing


Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\CPUMine.conf",1)
Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
CPUMine = objFileToRead.ReadAll()
objFileToRead.Close
Set objFileToRead = Nothing

Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\Threads.conf",1)
Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
Threads = objFileToRead.ReadAll()
objFileToRead.Close
Set objFileToRead = Nothing

Set objFileToRead = CreateObject("Scripting.FileSystemObject").OpenTextFile(".\Hidden.conf",1)
Set ObjFso2 = CreateObject("Scripting.FileSystemObject")
Hidden = objFileToRead.ReadAll()
objFileToRead.Close
Set objFileToRead = Nothing

'Qubit Mining - CPU and GPU
'While R=2 do
'If Hidden=1 Then
'WshShell.Run chr(34) & ".\qubithidden.vbs" & Chr(34), 0
'End If
'If Hidden=0 Then
'objshell.Run ".\Qubit.vbs" 
'End If
'wscript.quit
'loop
'wend

'Skein Mining - CPU and GPU
'While R=1 do
'If Hidden=1 Then
'objshell.Run ".\Skeinhidden.vbs" 
'End If
'If Hidden=0 Then
'Set ObjShell = CreateObject("WScript.Shell") 
'objshell.Run chr(34) & ".\Skein.vbs" & Chr(34), 0
'Set ObjShell = Nothing
'End If
'wscript.quit
'loop
'wend

'Groestl Mining - CPU and GPU
cmdline=(".\coin-util.exe -a GRS pubkey-to-addr " & WalletADR & " > Address.conf")
Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(cmdlineloc,2,true)
objFileToWrite.WriteLine(cmdline)
objFileToWrite.Close
Set objFileToWrite = Nothing
objshell.run ".\cmdline.bat"

While R=3 do

If Hidden=1 Then
wscript.sleep 2000
objshell.Run ".\Groestlhidden.vbs"
End If
If Hidden=0 Then
wscript.sleep 2000
objshell.Run ".\Groestl.vbs" 
End If
wscript.quit
loop
wend

'Scrypt Mining
'While R=4 do
'If Hidden=1 Then
'objshell.Run chr(34) & ".\ScryptHidden.vbs" & Chr(34), 0
'End If
'If Hidden=0 Then
'objshell.Run ".\Scrypt.vbs" 
'End If
'wscript.quit
'loop
'wend

'Launch Help page
If R=5 Then
CreateObject("WScript.Shell").Run "http://myriadplatform.org/mining-help/"
End If


'SHA256 Mining - Placeholder incase we add 
'If R=6 Then
'Do until Q=1
'If Arch="X86" Then
'objShell.CurrentDirectory = ".\Skein\Skein32"
'objshell.run "SkeinCPU32.EXE" & " " & "-a skein -o stratum+tcp://birdspool.no-ip.org:5589 -u" & " " & WalletADR & " " & "-p 912837465"
'End If
'If Arch="X64" Then
'objShell.CurrentDirectory = ".\Skein"
'objshell.run "SkeinCPU64.EXE" & " " & "-a skein -o stratum+tcp://birdspool.no-ip.org:5589 -u" & " " & WalletADR & " " & "-p 912837465"
'End If
'If OCL=0 Then
'Wscript.echo("Please download the latest drivers for your video card to enable much faster GPU Mining!)
'End IF
'If OCL=1 Then
'objShell.CurrentDirectory = ".\SkeinGPU"
'objshell.run "SkeinGPU.EXE" & " " & "--kernel skein -o stratum+tcp://birdspool.no-ip.org:5589  -u" & " " & WalletADR & " " & "-p 912837465"
'End If
'Q=1
'wscript.quit