﻿using ComedorAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ComedorAPI.Data;
using ComedorAPI.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ComedorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {

        private readonly IMongoCollection<Reservation> _reservations;

        public ReservationController(MongoDbService mongoDbService)
        {
            _reservations = mongoDbService.Database?.GetCollection<Reservation>("reservation");


        }
        // GET: api/<ReservationController>
        [HttpGet]
        public async Task<IEnumerable<Reservation>> Get()
        {

            return await _reservations.Find(FilterDefinition<Reservation>.Empty).ToListAsync(); 
        }

        // GET api/<ReservationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation?>> GetById(string id)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);
            var reservation = _reservations.Find(filter).FirstOrDefault();  
            return reservation is not null ? Ok(reservation) : NotFound();
        }

        // POST api/<ReservationController>
        [HttpPost]
        public async Task<ActionResult> Create(Reservation reservation)
        {
            await _reservations.InsertOneAsync(reservation);    
            return CreatedAtAction(nameof(GetById), new { 
                id = reservation.Id,
                reservation
            });   
        }

        // PUT api/<ReservationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Reservation reservation)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, reservation.Id);
            await _reservations.ReplaceOneAsync(filter, reservation);
            return Ok();
        }

        // DELETE api/<ReservationController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);
            await _reservations.DeleteOneAsync(filter);
            return Ok();
        }
    }
}
