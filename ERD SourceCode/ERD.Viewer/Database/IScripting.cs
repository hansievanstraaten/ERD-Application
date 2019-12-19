using ERD.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Viewer.Database
{
  internal interface IScripting
  {
    DatabaseTypeEnum DatabaseScriptingType { get; }

    string ScriptTableCreate(TableModel table);

    string BuildForeignKey(TableModel table);

    string DropForeignKey(string tableName, string constaintName);
  }
}
