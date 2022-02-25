using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class SiteSqlDao : ISiteDao
    {
        private readonly string connectionString;

        public SiteSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Site> GetSitesThatAllowRVs(int parkId)
        {
            IList<Site> sites = new List<Site>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * " +
                    "FROM site s JOIN campground c ON s.campground_id = c.campground_id " +
                    "WHERE max_rv_length > 0 AND park_id = @park_id;", conn);
                cmd.Parameters.AddWithValue("@park_id", parkId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = GetSiteFromReader(reader);
                    sites.Add(site);
                }
            }
            return sites;
        }
        public List<Site> AvailableSites(int parkId)
        {

            List<Site> availableSites = new List<Site>();
            DateTime today = DateTime.Now;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM site " +
                    "WHERE site.site_id " +
                    "NOT IN(SELECT s.site_id FROM site s " +
                    "JOIN campground c ON s.campground_id = c.campground_id " +
                    "JOIN reservation r ON s.site_id = r.site_id WHERE park_id = @park_id AND @today " +
                    "BETWEEN from_date AND to_date GROUP BY s.site_id);", conn);

                cmd.Parameters.AddWithValue("@park_id", parkId);
                cmd.Parameters.AddWithValue("@today", today);


                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = GetSiteFromReader(reader);
                    availableSites.Add(site);

                }

            }

            return availableSites;

        }

        private Site GetSiteFromReader(SqlDataReader reader)
        {
            Site site = new Site();
            site.SiteId = Convert.ToInt32(reader["site_id"]);
            site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            site.SiteNumber = Convert.ToInt32(reader["site_number"]);
            site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            site.Accessible = Convert.ToBoolean(reader["accessible"]);
            site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
            site.Utilities = Convert.ToBoolean(reader["utilities"]);

            return site;
        }
    }
}
