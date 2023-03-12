using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petbook.Data;
using Petbook.Models;
using Petbook.Controllers;
using System;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Tests
{
    [TestClass]
    public class PostsControllerTests
    {   
        private ApplicationDbContext GetDbContext()
        {
            ApplicationDbContext db = new ApplicationDbContext(null);
            db.PopulateDb(10);
            return db;
        }

        [TestMethod]
        public void TestIndex()
        {
            // arrange
            var db = GetDbContext();
            var controller = new PostsController(db);

            // act
            var result = controller.Index();
            
            // assert
            Assert.IsNotNull(result);
            //Assert.AreEqual(10, result.Value.Count);
        }
    }
}