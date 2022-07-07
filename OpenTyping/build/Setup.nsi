!include MUI2.nsh
!include nsProcess.nsh 

; HM NIS Edit Wizard helper defines
!define MUI_ICON "icon.ico"
!define MUI_UNICON "icon.ico"
!define PRODUCT_NAME "TezTer"
!define APP_NAME "TezTer.exe"
!define PRODUCT_VERSION "1.2.4"
!define PRODUCT_PUBLISHER "RoboticsWare"
!define PRODUCT_WEB_SITE "https://RoboticsWare.uz"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; Pages
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES


; Language files
!insertmacro MUI_LANGUAGE "Russian" ;first language is the default language

LangString TEXT_TEZTER ${LANG_RUSSIAN} "TezTer"
LangString TEXT_TEZTER_DELETE ${LANG_RUSSIAN} "TezTer O'chirish"
LangString TEXT_TEZTER_TITLE ${LANG_RUSSIAN} "TezTer (zaruriy)"
LangString TEXT_START_MENU_TITLE ${LANG_RUSSIAN} "Boshlash menu yorliq"
LangString TEXT_DESKTOP_TITLE ${LANG_RUSSIAN} "Desktop yorlig'i"
LangString DESC_TEZTER ${LANG_RUSSIAN} "TezTer Dasturi"
LangString DESC_START_MENU ${LANG_RUSSIAN} "Boshlash menuda yorliqni yaratadi"
LangString DESC_DESKTOP ${LANG_RUSSIAN} "Desktopda yorliqni yaratadi"
LangString SETUP_UNINSTALL_MSG ${LANG_RUSSIAN} "TezTer alaqachon o'rinatilgan. $\n$\nOldingisini o'chirish uchun 'OK'ni bosing yoki o'rnatishni xohlamasangiz 'Отмена'ni boshing."

!insertmacro MUI_LANGUAGE "English"

LangString TEXT_TEZTER ${LANG_ENGLISH} "TezTer"
LangString TEXT_TEZTER_DELETE ${LANG_ENGLISH} "TezTer Uninstall"
LangString TEXT_TEZTER_TITLE ${LANG_ENGLISH} "TezTer (required)"
LangString TEXT_START_MENU_TITLE ${LANG_ENGLISH} "Start menu shortcut"
LangString TEXT_DESKTOP_TITLE ${LANG_ENGLISH} "Desktop shortcut"
LangString DESC_TEZTER ${LANG_ENGLISH} "TezTer Program"
LangString DESC_START_MENU ${LANG_ENGLISH} "Create shortcut on start menu"
LangString DESC_DESKTOP ${LANG_ENGLISH} "Create shortcut on desktop"
LangString SETUP_UNINSTALL_MSG ${LANG_ENGLISH} "TezTer is already installed. $\n$\nClick 'OK' to remove the previous version or 'Cancel' to cancel this upgrade."

!insertmacro MUI_LANGUAGE "Korean" 

LangString TEXT_TEZTER ${LANG_KOREAN} "TezTer"
LangString TEXT_TEZTER_DELETE ${LANG_KOREAN} "TezTer 제거"
LangString TEXT_TEZTER_TITLE ${LANG_KOREAN} "TezTer (필수)"
LangString TEXT_START_MENU_TITLE ${LANG_KOREAN} "시작메뉴에 바로가기"
LangString TEXT_DESKTOP_TITLE ${LANG_KOREAN} "바탕화면에 바로가기"
LangString DESC_TEZTER ${LANG_KOREAN} "TezTer 기본 프로그램"
LangString DESC_START_MENU ${LANG_KOREAN} "시작메뉴에 바로가기 아이콘이 생성됩니다."
LangString DESC_DESKTOP ${LANG_KOREAN} "바탕화면에 바로가기 아이콘이 생성됩니다."
LangString SETUP_UNINSTALL_MSG ${LANG_KOREAN} "TezTer가 이미 설치되어 있습니다. $\n$\r'확인' 버튼을 누르면 이전 버전을 삭제 후 재설치하고 '취소' 버튼을 누르면 업그레이드를 취소합니다."


; The name of the installer
Name "$(TEXT_TEZTER)"

; The file to write
OutFile "${PRODUCT_NAME}_${PRODUCT_VERSION}_Setup.exe"

InstallDir "$PROGRAMFILES\TezTer"

; Registry key to check for directory (so if you install again, it will
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\${PRODUCT_NAME}" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin 

ShowInstDetails show

ShowUnInstDetails show

 

Section $(TEXT_TEZTER_TITLE) SectionTezTer

  SectionIn RO

 ; Put file there
  SetOutPath "$INSTDIR\data"
  File "..\bin\x86\Release\data\*.*"

  SetOutPath "$INSTDIR\layouts"
  File "..\bin\x86\Release\layouts\*.*"

  SetOutPath "$INSTDIR\Resources"
  File /r "..\bin\x86\Release\Resources\*.*"

  SetOutPath "$INSTDIR\ko"
  File "..\bin\x86\Release\ko\*.*"

  SetOutPath "$INSTDIR\uz"
  File "..\bin\x86\Release\uz\*.*"

  SetOutPath "$INSTDIR\ru"
  File "..\bin\x86\Release\ru\*.*"

  SetOutPath "$INSTDIR\x86"
  File "..\bin\x86\Release\x86\*.*"

  SetOutPath "$INSTDIR\x64"
  File "..\bin\x86\Release\x64\*.*"

  SetOutPath "$INSTDIR"
  File "..\bin\x86\Release\TezTer.exe"
  File "..\bin\x86\Release\NLog.config"
  File "icon.ico"

  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\${PRODUCT_NAME}" "Install_Dir" "$INSTDIR"

; Write the uninstall keys for Windows
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" '"$INSTDIR\$(TEXT_TEZTER_DELETE).exe"'
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\icon.ico"
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoModify" 1
  WriteRegDWORD ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoRepair" 1
  WriteUninstaller "$INSTDIR\$(TEXT_TEZTER_DELETE).exe"

SectionEnd

; Optional section (can be disabled by the user)
Section $(TEXT_START_MENU_TITLE) SectionStartMenu

  CreateDirectory "$SMPROGRAMS\${PRODUCT_NAME}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\$(TEXT_TEZTER_DELETE).lnk" "$INSTDIR\$(TEXT_TEZTER_DELETE).exe" "" "$INSTDIR\$(TEXT_TEZTER_DELETE).exe" 0
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\$(TEXT_TEZTER).lnk" "$INSTDIR\${PRODUCT_NAME}.exe" "" "$INSTDIR\icon.ico" 0

SectionEnd

 
; Optional section (can be disabled by the user)
Section $(TEXT_DESKTOP_TITLE) SectionDesktop

  CreateShortCut "$DESKTOP\$(TEXT_TEZTER).lnk" "$INSTDIR\${PRODUCT_NAME}.exe" "" "$INSTDIR\icon.ico" 0

SectionEnd

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${SectionTezTer} $(DESC_TEZTER)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionStartMenu} $(DESC_START_MENU)
    !insertmacro MUI_DESCRIPTION_TEXT ${SectionDesktop} $(DESC_DESKTOP)
!insertmacro MUI_FUNCTION_DESCRIPTION_END


; Uninstaller
Section "Uninstall"

  ; Remove registry keys
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "SOFTWARE\${PRODUCT_NAME}"

  ; Remove files and uninstaller
  Delete $INSTDIR\*


  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\*.*"

  Delete "$DESKTOP\$(TEXT_TEZTER).lnk"

  ; Remove directories used
  RMDir "$SMPROGRAMS\${PRODUCT_NAME}"
  RMDir /r "$INSTDIR"
  RMDir /r "$APPDATA\${PRODUCT_NAME}"
  RMDir /r "$LOCALAPPDATA\${PRODUCT_NAME}"

SectionEnd

Function LaunchLink
  Exec "${APP_NAME}"
FunctionEnd

Function .onInit
	${nsProcess::FindProcess} "${APP_NAME}" $R0
	StrCmp $R0 0 mfound notRunning
	mfound:
		${nsProcess::KillProcess} "${APP_NAME}" $R0
	notRunning:

	ReadRegStr $R0 HKLM \
	"Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" \
	"UninstallString"
	StrCmp $R0 "" done

    ReadRegStr $R1 HKLM "SOFTWARE\${PRODUCT_NAME}" "Install_Dir"
    StrCmp $R1 "" done

	MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION \
	$(SETUP_UNINSTALL_MSG) \
	IDOK uninst
	Abort

	;Run the uninstaller
	uninst:
		ClearErrors
		;ExecWait '$R0 _?=$INSTDIR'
		ExecWait '$R0 _?=$R1'

		;IfErrors no_remove_uninstaller done
		;no_remove_uninstaller:
	IfErrors 0 +2
		Goto no_remove_uninstaller
		RMDir /r /REBOOTOK $R1
    RMDir /r "$APPDATA\${PRODUCT_NAME}"
	RMDir /r "$LOCALAPPDATA\${PRODUCT_NAME}"
		Goto done

	no_remove_uninstaller:
		DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
		DeleteRegKey HKLM "SOFTWARE\${PRODUCT_NAME}"

	done:

FunctionEnd
