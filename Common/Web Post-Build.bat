@echo off
REM This file is invoked from the post-build step of all web projects.
REM It copies configuration files from the shared Config folder to the
REM appropriate locations within the website.

REM It should be invoked as follows:
REM "$(ProjectDir)..\..\Setup\Common\Web Post-Build.bat" "$(ProjectDir)" $(ConfigurationName)

REM The two Web.*.config files should not be included in the project;
REM they are only referenced by filename, without the project system.
REM (by the configuration loader, and by the XDT transformer).  
REM App_Data\ShomreiTorahConfig.xml must be included in the project,
REM since it needs to be published to the production server.
REM It is not used at all when debugging.

if not "%1" == "" goto :exec
echo This script must be invoked from a post-build step as follows:
echo "$(ProjectDir)..\..\Setup\Common\Web Post-Build.bat" "$(ProjectDir)" $(ConfigurationName)

goto :end


:exec
copy "%1..\..\Config\Debug\Web.ConnectionStrings.config" "%1"
copy "%1..\..\Config\Production\Web.release.config" "%1"

mkdir "%1App_Data\"
if "%2" == "Debug" (
    del "%1App_Data\ShomreiTorahConfig.xml"
) else (
    copy "%1..\..\Config\Production\ShomreiTorahConfig.xml" "%1App_Data\"
)

:end