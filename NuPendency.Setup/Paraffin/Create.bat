mkdir ..\Fragments
Paraffin.exe -dir ..\..\out ^
             -NoRootDirectory ^
             -alias $(var.SourcePath) ^
             -GroupName NuPendency ..\Fragments\Fragments.wxs ^
             -ext .xml ^
             -ext .json ^
             -ext .sdf ^
             -ext .pdb ^
             -regExExclude "\.vshost\." ^
             -regExExclude "\.dll\.conf"
