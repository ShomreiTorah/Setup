#Shomrei Torah Suite
This project contains setup scripts and common files used by Shomrei Torah applications, as well as a configuration GUI.

##Installing

The Shomrei Torah suite requires .Net 4.0 and Visual Studio 2012 (2010 should also work).  
The GUI applications (except for the configurator) also require the [DevExpress WinForms suite](http://www.devexpress.com/Products/NET/Controls/WinForms/).

The Shomrei Torah projects must be laid out in a specific directory structure, created by `Install.cmd` in this repository.  
To get started, create a `ShomreiTorah` folder (or other name) to contain all of the projects, then clone this repository into the `Setup` subfolder.  
Then, run `Install.bat`, which will clone the other repositories into the correct directories, initial the `Config` directory, then build and run the configurator.

##Configuration

Shomrei Torah applications read configuration data (eg, organization info, DB and SMTP servers) from a `ShomreiTorahConfig.xml` using the `ShomreiTorah.Common.Config` class.  The Configurator project in the Setup repository provides a graphical editor for this file.  

Debug builds will load ShomreiTorahConfig from `Config\Debug\ShomreiTorahConfig.xml` in the source tree.  

Release builds will look in the following directories, loading the first XML file found.  (subsequent files are not merged in)

 1. The value of the `ShomreiTorah.Common.Config.FilePath` property, if set manually before the config is loaded.
 2. A `ShomreiTorahConfig.xml` file in the current directory.
 3. The value of the `ShomreiTorahConfig.xml` from `<appSettings>` in `App.config`
 4. The `HKCU\SOFTWARE\Shomrei Torah\ShomreiTorahConfig.xml" value in the registry
 5. The `HKLM\SOFTWARE\Shomrei Torah\ShomreiTorahConfig.xml" value in the registry
