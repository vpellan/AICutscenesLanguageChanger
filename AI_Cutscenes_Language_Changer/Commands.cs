using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using McMaster.Extensions.CommandLineUtils;

namespace AI_Cutscenes_Language_Changer
{
    [Command(Description = "Create backup and change cutscenes language")]
    public class ChangeCommand
    {
        [Required]
        [FileOrDirectoryExists]
        [Argument(0, Description = "File or folder containing usm files")]
        public string InputPath { get; set; }

        [Required]
        [Option(CommandOptionType.SingleValue, Description = "Specify system/game language", ShortName = "s",
            LongName = "sys-lang")]
        public int SysLang { get; set; } = 1;

        [Required]
        [Option(CommandOptionType.SingleValue, Description = "Specify desired language", ShortName = "d",
            LongName = "dest-lang")]
        public int DestLang { get; set; } = 0;
        
        [Option(CommandOptionType.NoValue, Description = "Do not make a backup of files. Warning: You will not be able to change the language anymore", ShortName = "", LongName = "no-backup")]
        public bool NoBackup { get; set; }
        
        [Option(CommandOptionType.SingleValue, Description = "Specify number of threads to use",
            ShortName = "t", LongName = "threads")]
        public int MaxNumberOfThreads { get; set; } = 1;

        protected int OnExecute(CommandLineApplication app)
        {
            var attr = File.GetAttributes(InputPath);
            if (!attr.HasFlag(FileAttributes.Directory))
                throw new DirectoryNotFoundException("Input is not a directory");
            if (SysLang == DestLang || SysLang is < 0 or > 6 || DestLang is < 0 or > 6)
                throw new ArgumentException("SysLang and DestLang must be between 0 and 6, and must be different");
            var backupPath = Path.Combine(InputPath, "backup");
            var fileList = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Program.AbsolutePath("fileList.json")));
            if (fileList == null) throw new NullReferenceException("fileList.json not found");
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            foreach (var file in Directory.GetFiles(InputPath, "*.usm").Where(file => fileList.Contains(Path.GetFileNameWithoutExtension(file))))
            {
                if (!File.Exists(Path.Combine(backupPath, Path.GetFileName(file))))
                    File.Move(file, Path.Combine(backupPath, Path.GetFileName(file)), true);
            }
            var tempPath = Path.Combine(InputPath, "temp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            Helpers.ExecuteProcess(Program.AbsolutePath("usmtoolkit/UsmToolkit"), $"convert \"{backupPath}\" -o=\"{tempPath}\" -c -t={MaxNumberOfThreads}");
            
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = MaxNumberOfThreads
            };
            Parallel.ForEach(Directory.GetFiles(tempPath, "*.avi"), parallelOptions, Process);
            
            Directory.Delete(tempPath, true);
            if (NoBackup)
                Directory.Delete(backupPath, true);
            
            return 0;
        }

        private void Process(string videoFileName)
        {
            var filePrefix = videoFileName[..^4];
            var tempPath = Path.GetDirectoryName(videoFileName);
            var moviesPath = tempPath![..^5];
            var outputFile = Path.Combine(moviesPath, Path.GetFileNameWithoutExtension(filePrefix) + ".usm");
            
            // Building arguments
            var args = new StringBuilder();
            args.Append("-gop_closed=on -gop_i=1 -gop_p=4 -gop_b=2 -video00=\"").Append(videoFileName).Append("\" -output=\"").Append(outputFile).Append("\" -br_range=0,16000000 -audio0").Append(SysLang).Append("=\"");
            var destAudioFile = "";
            foreach (var audioFile in Directory.GetFiles(tempPath, "*.wav").Where(file => file.StartsWith(filePrefix)))
            {
                if (audioFile[filePrefix.Length + 2] - '0' != DestLang) continue;
                destAudioFile = audioFile;
                break;
            }

            args.Append(destAudioFile).Append('"');
            if (Directory.GetFiles(tempPath, "*.txt").Any(file => file.StartsWith(filePrefix)))
            {
                var subtitles = Directory.GetFiles(tempPath, "*.txt").First(file => file.StartsWith(filePrefix));
                args.Append(" -subtitle00=\"").Append(subtitles).Append('"');
            }
            
            // Execute medianoche
            Helpers.ExecuteProcess(Program.AbsolutePath("ScaleformVideoEncoder/medianoche"), args.ToString(), true);
        }
    }

    [Command(Description = "Restore files from backup folder")]
    public class RestoreCommand
    {
        [Required]
        [FileOrDirectoryExists]
        [Argument(0, Description = "File or folder containing usm files")]
        public string InputPath { get; set; }

        protected int OnExecute(CommandLineApplication app)
        {
            var attr = File.GetAttributes(InputPath);
            if (!attr.HasFlag(FileAttributes.Directory))
                throw new DirectoryNotFoundException("Input is not a directory");
            var backupPath = Path.Combine(InputPath, "backup");
            if (!Directory.Exists(backupPath)) throw new DirectoryNotFoundException();
            foreach (var file in Directory.GetFiles(backupPath, "*.usm"))
            {
                File.Move(file, Path.Combine(InputPath, Path.GetFileName(file)), true);
            }
            if (Directory.GetFiles(backupPath).Length == 0)
                Directory.Delete(backupPath);

            return 0;
        }
    }
}