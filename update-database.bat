@echo off
echo Updating database to support decimal flight numbers...

REM Backup existing database
if exist discs.db (
    echo Backing up existing database...
    copy discs.db discs_backup_%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%.db
    echo Database backed up!
)

REM Delete old database to recreate with new schema
if exist discs.db (
    echo Removing old database...
    del discs.db
    echo Old database removed!
)

echo Database schema will be recreated when you start the app.
echo Your data has been backed up to discs_backup_*.db
echo.
echo Press any key to continue...
pause
