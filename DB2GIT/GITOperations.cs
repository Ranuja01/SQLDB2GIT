using System;
using System.Configuration;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace DB2GIT
{
    // ********************************************************************
    // Install-Package LibGit2Sharp -version  0.21.0.176
    // (other versions do not work as expected
    // ********************************************************************
    public class GITOperations
    {
        // All the data get from App.config data are encrypted.
         
        private string UserName = ConfigurationManager.AppSettings["GITUserName"];
        private string Word_pAss = ConfigurationManager.AppSettings["GITWord_pAss"];
        private string GITRepoUrl = ConfigurationManager.AppSettings["GITRepoUrl"];
        private string GITRepoCommitUrl = ConfigurationManager.AppSettings["GITRepoUrl"] + @"/commit/";
        private string BranchName = ConfigurationManager.AppSettings["GITBranchName"];
        private string RemoteName = ConfigurationManager.AppSettings["GITRemoteName"];
        private string RepoFolder = ConfigurationManager.AppSettings["RepoFolder"];
        private readonly UsernamePasswordCredentials Credentials;

        public DirectoryInfo FileFullPath { get; set; }
        public string CommitMessage { get; set; }
        public GITOperations()
        {
            Credentials = new UsernamePasswordCredentials
            {
                Username = this.UserName,
                Password = this.Word_pAss
            };
            FileFullPath = new DirectoryInfo(RepoFolder);
        }

        public string CommitAllChanges(string msg)
        {
            CommitMessage = msg;
            string gitID = string.Empty;
            using (var repo = new Repository(RepoFolder))
            {
                var files = FileFullPath.GetFiles("*", SearchOption.AllDirectories).Select(f => f.FullName);
                repo.Stage("*");
                try
                {
                    repo.Commit(msg);
                    //var x = repo.Head.Tip.Message;
                    //var x1 = repo.Head.Tip.MessageShort;  -- attempt to get descriptive comment

                    // getting back the Hex # (ID) from GIT
                    gitID = GITRepoCommitUrl + repo.Head.Tip.Id.ToString();
                }
                catch (Exception excp)
                {
                    if ( excp.Message != "No changes; nothing to commit.")
                    {
                        // let it continue 
                    }
                    else
                    {
                        // log it -- not done yet
                    }
                }
            }
            return gitID;
        }

        public void PushCommits()
        {
            using (var repo = new Repository(RepoFolder))
            {
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == RemoteName);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(RemoteName, GITRepoUrl);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == RemoteName);
                }

                var options = new PushOptions
                {
                    CredentialsProvider = (url, usernameFromUrl, types) => Credentials
                };
                repo.Network.Push(remote, BranchName, options);
            }
        }

    }
}
