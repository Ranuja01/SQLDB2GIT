using System.Configuration;
using System.IO;

namespace DB2GIT
{
    public class FileOperations
    {
        public string RepoFolder { get; set; }
        public string DirFullPath { get; set; }

        public FileOperations(LocalRepoFolder lf)
        {
            RepoFolder = ConfigurationManager.AppSettings["RepoFolder"];
            DirFullPath = RepoFolder + lf.FolderStructure;
            //Directory.CreateDirectory(dirFullPath);
        }

        public void writeToFile(DBObjChanges dBObjAll)
        {
            var fileContent = dBObjAll.DBObjChange;
            var fileName = dBObjAll.ObjectName + ".sql";
            var FileFullPath = DirFullPath + "\\" + fileName;
            if (!Directory.Exists(DirFullPath))
            {
                Directory.CreateDirectory(DirFullPath);
            }
            File.WriteAllText(FileFullPath, fileContent);
        }
    }
}


/*
public FileOperations()
{
    RepoFolder = ConfigurationManager.AppSettings["RepoFolder"];
    DirFullPath = RepoFolder;
    //Directory.CreateDirectory(dirFullPath);
}
*/