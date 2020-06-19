
using System.Collections.Generic;
using System.Linq;


namespace DB2GIT
{
    public class DBOperations
    {
        public List<LocalRepoFolder> getUniqueFolders()
        {
            using (var db = new DBChangesAuditEntities())
            {
                return db.DBObjChanges.Where(q => q.BatchID == null)
                .GroupBy(g => new { g.DBObjType, g.DatabaseName, g.SchemaName })
                .Select(s => new LocalRepoFolder
                { DBObjType = s.Key.DBObjType, DatabaseName = s.Key.DatabaseName, SchemaName = s.Key.SchemaName })
                .ToList();
            }
        }

        public List<DBObjChanges> getUniqueGITCommittablesByFolder(LocalRepoFolder locfldr)
        {
            using (var db = new DBChangesAuditEntities())
            {
                return db.DBObjChanges.Where(q => q.BatchID == null).Where(q => q.BatchID == null
                && q.DBObjType == locfldr.DBObjType && q.DatabaseName == locfldr.DatabaseName
                && q.SchemaName == locfldr.SchemaName)
                .OrderBy(o => o.ObjectName).ThenBy(o => o.ID)
            .ToList(); 
            }
        }

        public void saveGITCommittable(DBObjChanges doc)
        {
            using (var db = new DBChangesAuditEntities())
            {
                var result = db.DBObjChanges.SingleOrDefault(q => q.ID == doc.ID);
                if (result != null)
                {
                    result.WritingToGIT = doc.WritingToGIT;
                    result.BatchID = doc.BatchID;
                    result.GitLink = doc.GitLink;
                    db.SaveChanges();
                }
            }
        }

    }

    /// <summary>
    /// The 2 classes, LocalRepoFolder & GITObjData are to hold data (no other purpose)
    /// </summary>
    public class LocalRepoFolder
    {
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string DBObjType { get; set; }
        public string FolderStructure
        {
            get { return "\\" + DatabaseName + "\\" + DBObjType + "\\" + SchemaName; }
        }
    }

    public class GITObjData
    {
        public string FolderStructure { get; set; }
        public string ObjectName { get; set; }

        public string FileName
        {
            get { return ObjectName + ".sql"; }
        }
    }

}
