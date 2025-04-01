@echo off
echo Converting map files from Content/Maps...

for %%f in (Content\Maps\*.txt) do (
    echo Processing: %%f
    python mapconverter.py "%%f" --output-dir Content\Maps
)

echo.
echo Conversion complete! Press any key to exit...
pause >nul