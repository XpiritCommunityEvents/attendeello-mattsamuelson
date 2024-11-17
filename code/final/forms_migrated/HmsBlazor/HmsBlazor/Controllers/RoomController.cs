using Microsoft.AspNetCore.Mvc;
using Project_HMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HmsBlazor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly DataAccess _dataAccess;

        public RoomController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            try
            {
                var sql = "select * from Room;";
                var ds = ExecuteQuery(sql);
                var rooms = ds.Tables[0].AsEnumerable().Select(row => new Room
                {
                    RId = row.Field<int>("RId"),
                    Category = row.Field<string>("Category"),
                    IsBooked = row.Field<string>("IsBooked"),
                    RoomCost = row.Field<double>("RoomCost")
                }).ToList();

                return Ok(rooms);
            }
            catch (Exception exc)
            {
                return StatusCode(500, "Internal server error: " + exc.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(int id)
        {
            try
            {
                var sql = $"select * from Room where RId = {id};";
                var ds = ExecuteQuery(sql);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return NotFound();
                }

                var row = ds.Tables[0].Rows[0];
                var room = new Room
                {
                    RId = row.Field<int>("RId"),
                    Category = row.Field<string>("Category"),
                    IsBooked = row.Field<string>("IsBooked"),
                    RoomCost = row.Field<double>("RoomCost")
                };

                return Ok(room);
            }
            catch (Exception exc)
            {
                return StatusCode(500, "Internal server error: " + exc.Message);
            }
        }

        [HttpPost]
        public ActionResult AddRoom([FromBody] Room room)
        {
            try
            {
                if (room == null)
                {
                    return BadRequest("Room is null.");
                }

                var sql = $"select * from Room where RId = {room.RId};";
                var ds = ExecuteQuery(sql);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    // Update
                    string query = $"update Room set Category = '{room.Category}', IsBooked = '{room.IsBooked}', RoomCost = {room.RoomCost} where RId = {room.RId};";
                    int count = ExecuteDML(query);
                    if (count == 1)
                    {
                        return Ok("Room Updated Successfully.");
                    }
                    else
                    {
                        return StatusCode(500, "Room Upgradation Failed.");
                    }
                }
                else
                {
                    // Insert
                    string query = $"insert into Room values ({room.RId}, '{room.Category}', '{room.IsBooked}', {room.RoomCost}, NULL);";
                    int count = ExecuteDML(query);
                    if (count == 1)
                    {
                        return Ok("Room Inserted.");
                    }
                    else
                    {
                        return StatusCode(500, "Room Insertion Failed.");
                    }
                }
            }
            catch (Exception exc)
            {
                return StatusCode(500, "Internal server error: " + exc.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRoom(int id)
        {
            try
            {
                var sql = $"delete from Room where RId = {id};";
                int count = ExecuteDML(sql);
                if (count == 1)
                {
                    return Ok($"Room {id} has been deleted.");
                }
                else
                {
                    return StatusCode(500, "Room Deletion Failed.");
                }
            }
            catch (Exception exc)
            {
                return StatusCode(500, "Internal server error: " + exc.Message);
            }
        }

        private DataSet ExecuteQuery(string sql)
        {
            return _dataAccess.ExecuteQuery(sql);
        }

        private int ExecuteDML(string sql)
        {
            return _dataAccess.ExecuteDML(sql);
        }

        public class Room
        {
            public int RId { get; set; }
            public string Category { get; set; }
            public string IsBooked { get; set; }
            public double RoomCost { get; set; }
        }
    }
}
