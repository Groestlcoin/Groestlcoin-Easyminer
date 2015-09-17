Option Explicit


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
dim strdesktop
dim oMyShortcut
dim strcurdir
dim wshshell




Set Filesys = CreateObject("Scripting.FileSystemObject") 
Set objFSO = CreateObject("Scripting.FileSystemObject") 
Set objShell = CreateObject("Wscript.Shell")
Set wshSystemEnv = objShell.Environment("SYSTEM")


addressloc = ".\Address.conf"
cmdlineloc = ".\cmdline.bat"


'Begin Documentation Run Check
Appdata2=objShell.ExpandEnvironmentStrings("%Appdata%")
Strfolderpath2 = Appdata2 & "\Electrum-grs" 
User2=objShell.ExpandEnvironmentStrings("%USERPROFILE%")
Strfolderpath3 = User2 & "\Desktop" 
Set objShell = CreateObject("Wscript.Shell")
 If Not objFSO.FolderExists(strfolderpath2) then
  Electrumpresent =0
  CreateObject("WScript.Shell").Run ".\taskend.bat"
  wscript.sleep 1500
  CreateObject("WScript.Shell").Run ".\installation.html"
   wscript.sleep 2500
  CreateObject("WScript.Shell").Run ".\Electrum-grsWallet.exe"
  wscript.echo "Please re-open Easyminer when electrum setup is complete"
  wscript.quit
'One time only, Set Threads VAR to 1 less core than host system maximum to try to keep system fluidity
 End If

set objShell = CreateObject("Wscript.Shell")
 If objFSO.FolderExists(strfolderpath2) then
  Electrumpresent =1
End if
'End Documentation Check

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



cmdline=(".\coin-util.exe -a GRS pubkey-to-addr " & WalletADR & " > Address.conf")
Set objFileToWrite = CreateObject("Scripting.FileSystemObject").OpenTextFile(cmdlineloc,2,true)
objFileToWrite.WriteLine(cmdline)
objFileToWrite.Close
Set objFileToWrite = Nothing
objshell.run ".\cmdline.bat"
CreateObject("WScript.Shell").Run ".\cmdline.bat"
End if