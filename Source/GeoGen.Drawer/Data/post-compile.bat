@ECHO OFF

SET /A starting_index = %1
SET /A number_of_files = %2
SET /A maximal_index = %starting_index% + %number_of_files% - 1

FOR /L %%i IN (%starting_index% 1 %maximal_index%) DO "C:\Program Files\SumatraPDF\SumatraPDF.exe" "figures.%%i"

EXIT 0