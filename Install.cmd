@ECHO OFF
SETLOCAL
where /q git
IF %ERRORLEVEL% == 1 (
	ECHO This setup script requires that git be in the system path.
	GOTO :eof 
)
SET MSBUILD=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
IF NOT EXIST "%MSBuild%" (
	ECHO The Shomrei Torah Suite requires .Net 4.0.
	GOTO :eof 
)
PUSHD %~dp0\..
SET REMOTE_BASE=https://github.com/ShomreiTorah

ECHO Cloning projects
git clone 2>nul %REMOTE_BASE%/Libraries	Libraries
git clone 2>nul %REMOTE_BASE%/Utilities	Utilities
git clone 2>nul %REMOTE_BASE%/Billing		Applications/Billing
git clone 2>nul %REMOTE_BASE%/Schedulizer	Applications/Schedulizer
git clone 2>nul %REMOTE_BASE%/Rafflizer	Applications/Rafflizer
git clone 2>nul %REMOTE_BASE%/Journal		Applications/Journal

ECHO Setting up private Config directory...
git init -q Config
mkdir "Config\Email Templates\" 2>nul
mkdir "Config\Word Templates\"  2>nul
copy "Template Files\Email Templates" "Config\Email Templates\" >nul
copy "Template Files\Word Templates"  "Config\Word Templates\"  >nul

ECHO Building Configurator...
"%MSBuild%" "Setup\Configurator\ShomreiTorah-Configurator.sln" /target:rebuild /property:Configuration=Release /noconlog /nologo /v:q
start Setup\Configurator\bin\Release\Configurator.exe

POPD
ENDLOCAL