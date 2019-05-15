using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YMOA.DALFactory;
using YMOA.Model;

namespace YMOA.UnitTest
{
    [TestClass]
    public class SystemTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            LibraryEntity libraryEntity = new LibraryEntity();
            libraryEntity.id = 1;
            libraryEntity.tag = 1;
            libraryEntity.pid = 0;
            libraryEntity.sort = 5;
            libraryEntity.name = "行政";
            libraryEntity.code = "as";
            int i = DALCore.GetInstance().SystemCore.LibrarySave(libraryEntity);
        }
    }
}
