using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class ReservationSqlDao : IReservationDao
    {
        private readonly string connectionString;

        public ReservationSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int CreateReservation(int siteId, string name, DateTime fromDate, DateTime toDate)
        {
            int reservationId;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO reservation (site_id, name, from_date, to_date) " +
                    "OUTPUT INSERTED.reservation_id " +
                    "VALUES (@site_id, @name, @from_date, @to_date);", conn);
                cmd.Parameters.AddWithValue("@site_id", siteId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@from_date", fromDate);
                cmd.Parameters.AddWithValue("@to_date", toDate);

                reservationId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return reservationId;
        }

        public List<Reservation> GetUpcomingReservation(int parkId)
        {

            List<Reservation> upcomingReservation = new List<Reservation>();
            DateTime dateTime = DateTime.Now.AddDays(30);
            DateTime today = DateTime.Now;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM reservation r " +
                    "JOIN site s ON r.site_id = s.site_id " +
                    "JOIN campground c ON s.campground_id = c.campground_id " +
                    "WHERE park_id = @park_id AND from_date >= @today AND from_date <= @DateTime;", conn);
                cmd.Parameters.AddWithValue("@park_id", parkId);
                cmd.Parameters.AddWithValue("@DateTime", dateTime);
                cmd.Parameters.AddWithValue("@today", today);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Reservation reservation = GetReservationFromReader(reader);
                    upcomingReservation.Add(reservation);

                }
            }

                return upcomingReservation;
        }

        
            


        private Reservation GetReservationFromReader(SqlDataReader reader)
        {
            Reservation reservation = new Reservation();
            reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
            reservation.SiteId = Convert.ToInt32(reader["site_id"]);
            reservation.Name = Convert.ToString(reader["name"]);
            reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
            reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
            reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

            return reservation;
        }
    }
}
