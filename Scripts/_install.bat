@echo off

set ROOT_DIR=%~dp0..

set ANDROID_TOOLS=%ANDROID_HOME%\tools
set ANDROID_PLATFORM_TOOLS=%ANDROID_HOME%\platform-tools
set ADB="%ANDROID_PLATFORM_TOOLS%\adb.exe"
if not exist "%ADB%" (
	set ADB=%ANDROID_TOOLS%\adb.exe
)

call %ADB% install -r %ROOT_DIR%\Bin\Android\CrossPlatformMultiplayer_v001.apk
