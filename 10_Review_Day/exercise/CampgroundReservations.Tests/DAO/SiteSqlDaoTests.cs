using CampgroundReservations.DAO;
using CampgroundReservations.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CampgroundReservations.Tests.DAO
{
    [TestClass]
    public class SiteSqlDaoTests : BaseDaoTests
    {
        [TestMethod]
        public void GetSitesThatAllowRVs_Should_ReturnSites()
        {
            // Arrange
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            // Act
            IList<Site> sites = dao.GetSitesThatAllowRVs(ParkId);

            // Assert
            Assert.AreEqual(2, sites.Count);
        }

        [TestMethod]
        public void AvailableSiteTest()
        {
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            List<Site> sites = dao.AvailableSites(ParkId);

            Assert.AreEqual(2, sites.Count);
        }

        [TestMethod]
        public void FutureAvailableSiteTest()
        {
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            List<Site> sites = dao.FutureAvailableSites(ParkId, DateTime.Now.AddDays(1), DateTime.Now.AddDays(5));

            Assert.AreEqual(2, sites.Count);
        }
    }
}
