# Alien Isolation Cutscenes Language Changer

Tool that changes the language of Alien Isolation cutscenes by demuxing and remuxing .usm files.

## Getting started

Download and put Scaleform Video Encoder binaries in a folder named `ScaleformVideoEncoder` created at the same path as `AI_Cutscenes_Language_Changer` executable (Make sure that medianoche.exe and all of its dependency are in the `ScaleformVideoEncoder` folder). 

You can easily find them on Google.

### Changing the language
```
AI_Cutscenes_Language_Changer change <InputPath> [options]
```

| Arguments   |                                                                        Description |
|:------------|-----------------------------------------------------------------------------------:|
| InputPath   | File or folder containing usm files<br/> (usually `<path-to-game>/data/UI/Movies`) |

| Options                                     |                                                                                 Description |
|:--------------------------------------------|--------------------------------------------------------------------------------------------:|
| -s &#124; --sys-lang <SYS_LANG>             |                                                 Specify system/game language **(required)** |
| -d &#124; --dest-lang <DEST_LANG>           |                                                     Specify desired language **(required)** |
| --no-backup                                 | Do not make a backup of files. Warning: You will not be able to change the language anymore |
| -t &#124; --threads <MAX_NUMBER_OF_THREADS> |                                               Specify number of threads to use (default: 1) |
| -? &#124; -h &#124; --help                  |                                                                      Show help information. |


| `<SYS_LANG>` and `<DEST_LANG>` |               Language |
|:-------------------------------|-----------------------:|
| 0                              |                English |
| 1                              |                 French |
| 2                              |                Italian |
| 3                              |                 German |
| 4                              |                Spanish |
| 5                              | Portuguese (Brazilian) |
| 6                              |                Russian |

`<MAX_NUMBER_OF_THREADS>` - Integer

### Restoring original .usm files

**Warning** : if you have deleted your backup files, or used `--no-backup`, you cannot restore original files.
```
 AI_Cutscenes_Language_Changer restore <InputPath>
```

| Arguments   |                                                                                                       Description |
|:------------|------------------------------------------------------------------------------------------------------------------:|
| InputPath   | File or folder containing usm files<br/> (usually `<path-to-game>/data/UI/Movies`) and containing `backup` folder |

## License

UsmToolkit follows the MIT License. It uses code from [VGMToolbox](https://sourceforge.net/projects/vgmtoolbox/).
