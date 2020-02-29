::Simple batch file to remove the extra dll when after compiling
pushd %~dp0
del "Assembly-CSharp.dll"
del "Assembly-CSharp-firstpass.dll"
del "NAudio.dll"
del "NVorbis.dll"
del "TextMeshPro-1.0.55.56.0b11.dll"
del "UnityEngine.dll"
del "UnityEngine.UI.dll"