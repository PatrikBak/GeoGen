@echo OFF
FOR /L %%i IN (1 1 %1) DO "C:\Program Files\SumatraPDF\SumatraPDF.exe" "figures.%%i"
EXIT 0