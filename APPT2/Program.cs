namespace FileSystemParser
{
    internal struct Directory
    {
        /// <summary>
        /// Name of a directory
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of subdirectories
        /// </summary>
        public Directory[] SubDirectories { get; set; }

        /// <summary>
        /// List of files
        /// </summary>
        public string[] Files { get; set; }
    }

    public class YouScrewedUpException : Exception
    {
        public YouScrewedUpException(string message) : base(message)
        {

        }
    }

    internal class Program
    {
        private const string StructureFileName = "Structure.txt";
        private static string[] getFileContent(string path)
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                throw new YouScrewedUpException("Structure.txt doesn't exist.");
            }
            string[] fileContent = File.ReadAllLines(file.FullName);
            if (fileContent.Length == 0)
            {
                throw new YouScrewedUpException("Content of the file is empty.");
            }
            
            return fileContent;
        }

        private static Directory DoForEverySubDirectory(string[] content, int contentStartLine, int step)
        {
            Directory rootDirectory = new Directory()
            {
                Name = content[contentStartLine].TrimStart('-'),
                SubDirectories = Array.Empty<Directory>(),
                Files = Array.Empty<string>()
            };

            Queue<Directory> directoriesQueue = new();
            Queue<string> filesQueue = new();

            for (int i = contentStartLine+1; i < content.Length; i++)
            {
                if (content[i][step] != '-')
                {
                    break;
                }
                if (content[i][step + 1] == '*')
                {
                    filesQueue.Enqueue(content[i].TrimStart('-').TrimStart('*'));
                }
                if ((content[i][step + 1] != '-') && (content[i][step + 1] != '*'))
                {
                    directoriesQueue.Enqueue(DoForEverySubDirectory(content, i, step + 1));
                }
            }
            
            rootDirectory.Files = filesQueue.ToArray();
            rootDirectory.SubDirectories = directoriesQueue.ToArray();
            directoriesQueue.Clear();
            filesQueue.Clear();
      
            return rootDirectory;
        }

        private static Directory ParseTree()
        {
            string[] structureFileContent = getFileContent(StructureFileName);
            Directory directory = DoForEverySubDirectory(structureFileContent, 0, 1);
            return directory;
        }

        private static string[] GetAllFilesWithExtensionRecursively(Directory directory, string extension)
        {  
            if (extension[0] == '.')
            {
                extension = extension.Substring(1);
            }

            string[] files = Array.Empty<string>();

            foreach (string file in directory.Files)
            {
                if (file.Split(".")[^1] == extension)
                {
                    Array.Resize(ref files, files.Length + 1);
                    files[^1] = file;
                }
            }

            foreach (Directory subDirectory in directory.SubDirectories)
            {
                string[] subDirsFiles = GetAllFilesWithExtensionRecursively(subDirectory, extension);
                Array.Resize(ref files, files.Length + subDirsFiles.Length);
                Array.Copy(subDirsFiles, 0, files, files.Length - subDirsFiles.Length, subDirsFiles.Length);
            }

            return files;
        }

        public static void Main(string[] args)
        {
            var cDrive = ParseTree();
            
            if (cDrive.Name != "C")
                throw new YouScrewedUpException("Root directory name must be C");

            if (cDrive.SubDirectories?.Length != 3 || cDrive.Files?.Length != 0)
                throw new YouScrewedUpException("Root directory must contain 3 subdirectories and 0 files, " +
                                                "still arrays must be intialized and stay empty");

            var system32Folder = cDrive.SubDirectories.FirstOrDefault(
                d => d.Name == "Windows").SubDirectories?.FirstOrDefault(
                d => d.Name == "System32");

            if (system32Folder?.Name != "System32"
                && system32Folder?.SubDirectories?.Length != 1
                && system32Folder?.Files?.Length != 3)
                throw new YouScrewedUpException("C:\\Windows\\System32 folder must exist, " +
                                                "contain 3 files and one subfolder");

            if (system32Folder?.SubDirectories?.First().Files.Any(f => f == "hosts") != true)
                throw new YouScrewedUpException("C:\\Windows\\System32\\drivers must contain one hosts file");

            var officeFolder = cDrive.SubDirectories.FirstOrDefault(
                d => d.Name == "Program Files").SubDirectories?.FirstOrDefault(
                d => d.Name == "Microsoft").SubDirectories?.FirstOrDefault(
                d => d.Name == "Office");

            if (officeFolder?.Files?.Length != 3 || officeFolder?.SubDirectories.Length != 1)
                throw new YouScrewedUpException("Office folder should must contain 3 files and 1 subfolder");

            string[] lnkFiles = GetAllFilesWithExtensionRecursively(cDrive, ".lnk");

            var lnkFilesThatShouldExist = new[]
                {"Excel.lnk", "Word.lnk", "PowerPoint.lnk", "Calculator.lnk", "SeaBeach.lnk"};

            if (lnkFiles?.Length != 5 || lnkFiles.All(
                    f => f != lnkFilesThatShouldExist[new Random().Next(lnkFilesThatShouldExist.Length)]))
                throw new YouScrewedUpException("C drive tree contains 5 lnk files in total " +
                                               "and random element from lnkFilesThatShouldExist should be present");
        }
    }
}