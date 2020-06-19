using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DB2GIT
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbOp = new DBOperations();
            var go = new GITOperations();
            // get all unique folders
            var uniqueFoldersInTable = dbOp.getUniqueFolders();

            // loop folders
            foreach (var f in uniqueFoldersInTable)
            {
                // file op object for the particular folder is initialized 
                // needed to 
                // 1. get all the records to be saved 
                //    (order by object name, then by ID and  - it is a must)
                // 2. get the folder to be put in to GIT 
                var fo = new FileOperations(f);

                var uniqueGITCommittables = dbOp.getUniqueGITCommittablesByFolder(f);
                var batchID = Guid.NewGuid();
                foreach (var ugc in uniqueGITCommittables)
                {

                    fo.writeToFile(ugc);
                    var msg = "ID: " + ugc.ID.ToString() + ((ugc.ChangeComment != "")? "Changes: " + ugc.ChangeComment : "") ;

                    var gitID = go.CommitAllChanges(msg);

                    ugc.WritingToGIT = DateTime.Now;
                    ugc.GitLink = gitID;
                    ugc.BatchID = batchID;
                    dbOp.saveGITCommittable(ugc);
                }
                go.PushCommits();
            }

        }

        static void Mainxxx(string[] args)
        {

            
            var dbOp = new DBOperations();
            //var go = new GITOperations();
            //// get all unique folders
            ////var uniqueFoldersInTable = dbOp.getUniqueFolders();
            //// loop folders
            //var fo = new FileOperations();
            //var uniqueGITCommittables = dbOp.getAllGITCommittables();

            //var batchID = Guid.NewGuid();
            //foreach (var ugc in uniqueGITCommittables)
            //{

            //    fo.writeToFile(ugc);
            //    var msg = "ID: " + ugc.ID.ToString();

            //    var gitID = go.CommitAllChanges(msg);

            //    ugc.WritingToGIT = DateTime.Now;
            //    ugc.GitLink = gitID;
            //    ugc.BatchID = batchID;
            //    dbOp.saveGITCommittable(ugc);
            //}
            //go.PushCommits();
            var uniqueFoldersInTable = dbOp.getUniqueFolders();
            var orgLstSatisfyingTheCriteria = GetFilteredList(uniqueFoldersInTable, "Ran");
            //foreach (var item in organizations)
            //{
            //    if (srchStrExists(item, "Alter"))
            //    {
            //        orgLstSatisfyingTheCriteria.Add(item);
            //    }
            //}


        }
        public static List<T> GetFilteredList<T>(List<T> inputList, string srchStr)
        {
            List<T> rtn = new List<T>();
            foreach (var item in inputList)
            {
                if (srchStrExists(item, srchStr))
                {
                    rtn.Add(item);
                }
            }
            return rtn;
        }

        public static bool srchStrExists(object obj, string srchStr)
        {
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.GetValue(obj, null) != null)
                {
                    string propVal = property.GetValue(obj, null).ToString();

                    if (propVal.Contains(srchStr))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    class c1
    {
        public string Year { get; set; }
        public string Period { get; set; }
        public string Allowed { get; set; }
    }

    class c2
    {
        public string Year { get; set; }
        public string Period { get; set; }
    }

 


}
