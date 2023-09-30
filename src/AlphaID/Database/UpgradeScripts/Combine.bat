@echo off
if not exist ".\Release" md "Release"
if exist ".\Release\Combined.sql" del ".\Release\Combined.sql"
for %%f in (*.sql) do type "%%f" >> .\Release\Combined.sql
echo success
pause