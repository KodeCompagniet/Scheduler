@echo off

rem del Help\*.* /s /q

attrib -r Documentation\lib\*.*
copy DemiCode.Scheduling\bin\Debug\DemiCode.Scheduling.dll Documentation\lib /Y /V
copy DemiCode.Scheduling\bin\Debug\DemiCode.Scheduling.xml Documentation\lib /Y /V
copy DemiCode.Shared\bin\Debug\DemiCode.Shared.dll Documentation\lib /Y /V
copy DemiCode.Shared\bin\Debug\DemiCode.Shared.xml Documentation\lib /Y /V
copy DemiCode.Scheduling.Loggers.log4net\bin\Debug\DemiCode.Logging.log4net.dll Documentation\lib /Y /V
copy DemiCode.Scheduling.Loggers.log4net\bin\Debug\DemiCode.Logging.log4net.xml Documentation\lib /Y /V

msbuild Documentation\Documentation.shfbproj

start Help\LastBuild.log
start Help\SchedulerDocumentation.chm