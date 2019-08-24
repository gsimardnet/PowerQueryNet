@echo off
cls
cd "C:\Temp\PowerQueryNet\Samples\PowerQueryApp\MyQueries"
C:
REM PQNet "#Hello World.pq"
REM PQNet "AdventureWorksSales.pq" -c "#credentials.xml"
REM PQNet "AdventureWorksSales.pq" -c "#credentials.xml" -o csv -f "%temp%\AdventureWorksSales.csv"
REM PQNet "AdventureWorksSales.pq" -c "#credentials.xml" -o html -f "%temp%\AdventureWorksSales.html"
REM PQNet "AdventureWorksSales.pq" -c "#credentials.xml" -o json -f "%temp%\AdventureWorksSales.json"
REM PQNet "AdventureWorksSales.pq" -c "#credentials.xml" -o xml -f "%temp%\AdventureWorksSales.xml"
REM PQNet "..\MyFiles\MyReport.pbix" vChineseCalendar -s "Data Source=.\SQL2016;Initial Catalog=master;Integrated Security=True" -t tChineseCalendar -a dc
REM PQNet "..\MyFiles\MyReport.xlsx" vChineseCalendar
@echo on
PQNet "..\MyFiles\MyReport.xlsx" vChineseCalendar
@echo off
if %ERRORLEVEL% EQU 0 (
    echo Success
) else (
    echo Exit Code is %errorlevel%
)