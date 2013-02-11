@ECHO OFF
IF NOT "%~n0"=="Setup" (
	ECHO ShomreiTorah/Setup must be cloned to a directory named "Setup".
	GOTO :eof 
)
set MSBUILD=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
IF NOT EXIST "%MSBuild%" (
	ECHO The Shomrei Torah Suite requires .Net 4.0.
	GOTO :eof 
)
pushd %~dp0
SET REMOTE_BASE=https://github.com/ShomreiTorah

ECHO Cloning projects
git clone -q %REMOTE_BASE%/Libraries	Libraries
git clone -q %REMOTE_BASE%/Utilities	Utilities
git clone -q %REMOTE_BASE%/Billing		Applications/Billing
git clone -q %REMOTE_BASE%/Schedulizer	Applications/Schedulizer
git clone -q %REMOTE_BASE%/Rafflizer	Applications/Rafflizer
git clone -q %REMOTE_BASE%/Journal		Applications/Journal

ECHO Setting up private Config directory
git init Config
copy "Template Files\Email Templates" "Config\Email Templates\"
copy "Template Files\Word Templates"  "Config\Word Templates\"

ECHO Building Configurator
"%MSBuild%" "Configurator/ShomreiTorah-Configurator.sln" /target:rebuild /property:Configuration=Release
Configurator\bin\Release\Configurator.exe

popd