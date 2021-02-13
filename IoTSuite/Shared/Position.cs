using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using IoTSuite.Shared.Filters;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IoTSuite.Shared
{
    public class Position
    {
        [Key, Required]
        public long Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public float Velocity { get; set; }

        public float Course { get; set; }

        [Required, ForeignKey("ThingId")]
        public Guid ThingId { get; set; }
        
        [JsonIgnore]
        public virtual Thing Thing { get; set; }

        public Position MapFromDTO(PositionDTO positionDTO)
        {
            ThingId = positionDTO.ThingId;
            Latitude = positionDTO.Latitude;
            Longitude = positionDTO.Longitude;
            Velocity = positionDTO.Velocity;
            Course = positionDTO.Course;

            return this;
        }
    }

    public class PositionDTO
    {

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        public float Velocity { get; set; }

        public float Course { get; set; }

        [Required, ForeignKey("ThingId")]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public virtual Thing Thing { get; set; }
    }

    public class PositionPaginationFilter : PaginationFilter, IValidatableObject
    {
        public Guid? ThingId { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public float? MinLatitude { get; set; }
        public float? MaxLatitude { get; set; }

        public float? MinLongitude { get; set; }
        public float? MaxLongitude { get; set; }

        public float? MinVelocity { get; set; }
        public float? MaxVelocity { get; set; }

        public float? MinCourse { get; set; }
        public float? MaxCourse { get; set; }

        public OrderByPosition OrderBy { get; set; }

        public async Task<(IQueryable<Position>, int)> Process(IQueryable<Position> positions, DbSet<Thing> thingsContext, Guid userId)
        {
            var things = Thing.Filter(thingsContext, userId);
            positions = positions.Where(positions => things.Select(thing => thing.ThingId).Contains(positions.ThingId));

            if (ThingId.HasValue)
            {
                positions = positions.Where(position => position.ThingId.Equals(ThingId.Value));
            }

            if (MinDate.HasValue)
            {
                positions = positions.Where(position => position.Date.CompareTo(MinDate.Value) >= 0);
            }
            if (MaxDate.HasValue)
            {
                positions = positions.Where(position => position.Date.CompareTo(MaxDate.Value) <= 0);
            }

            if (MinLatitude.HasValue)
            {
                positions = positions.Where(position => position.Latitude >= MinLatitude.Value);
            }
            if (MaxLatitude.HasValue)
            {
                positions = positions.Where(position => position.Latitude <= MaxLatitude.Value);
            }

            if (MinLongitude.HasValue)
            {
                positions = positions.Where(position => position.Longitude >= MinLongitude.Value);
            }
            if (MaxLongitude.HasValue)
            {
                positions = positions.Where(position => position.Longitude <= MinLongitude.Value);
            }

            if (MinVelocity.HasValue)
            {
                positions = positions.Where(position => position.Velocity >= MinVelocity.Value);
            }
            if (MaxVelocity.HasValue)
            {
                positions = positions.Where(position => position.Velocity <= MaxVelocity.Value);
            }

            if (MinCourse.HasValue)
            {
                positions = positions.Where(position => position.Course >= MinCourse.Value);
            }
            if (MaxCourse.HasValue)
            {
                positions = positions.Where(position => position.Course <= MaxCourse.Value);
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByPosition.Id:
                        positions = positions.OrderBy(position => position.Id);
                        break;
                    case OrderByPosition.Date:
                        positions = positions.OrderBy(position => position.Date);
                        break;
                    case OrderByPosition.ThingId:
                        positions = positions.OrderBy(position => position.ThingId);
                        break;
                    case OrderByPosition.Latitude:
                        positions = positions.OrderBy(position => position.Latitude);
                        break;
                    case OrderByPosition.Longitude:
                        positions = positions.OrderBy(position => position.Longitude);
                        break;
                    case OrderByPosition.Velocity:
                        positions = positions.OrderBy(position => position.Velocity);
                        break;
                    case OrderByPosition.Course:
                        positions = positions.OrderBy(position => position.Course);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByPosition.Id:
                        positions = positions.OrderByDescending(position => position.Id);
                        break;
                    case OrderByPosition.Date:
                        positions = positions.OrderByDescending(position => position.Date);
                        break;
                    case OrderByPosition.ThingId:
                        positions = positions.OrderByDescending(position => position.ThingId);
                        break;
                    case OrderByPosition.Latitude:
                        positions = positions.OrderByDescending(position => position.Latitude);
                        break;
                    case OrderByPosition.Longitude:
                        positions = positions.OrderByDescending(position => position.Longitude);
                        break;
                    case OrderByPosition.Velocity:
                        positions = positions.OrderByDescending(position => position.Velocity);
                        break;
                    case OrderByPosition.Course:
                        positions = positions.OrderByDescending(position => position.Course);
                        break;
                }
            }

            var count = await positions.CountAsync();

            positions = positions.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (positions, count);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MinDate > MaxDate)
            {
                results.Add(new ValidationResult($"MaxDate must be greater than MinDate", new string[] { "MinDate", "MaxDate" }));
            }

            if (MinLatitude > MaxLatitude)
            {
                results.Add(new ValidationResult($"MaxLatitude must be greater than MinLatitude", new string[] { "MinLatitude", "MaxLatitude" }));
            }

            if (MinLongitude > MaxLongitude)
            {
                results.Add(new ValidationResult($"MaxLongitude must be greater than MinLongitude", new string[] { "MinLongitude", "MaxLongitude" }));
            }

            if (MinVelocity > MaxVelocity)
            {
                results.Add(new ValidationResult($"MaxVelocity must be greater than MinVelocity", new string[] { "MinVelocity", "MaxVelocity" }));
            }

            if (MinCourse > MaxCourse)
            {
                results.Add(new ValidationResult($"MaxCourse must be greater than MinCourse", new string[] { "MinCourse", "MaxCourse" }));
            }

            return results;
        }
    }

    public enum OrderByPosition
    {
        Id,
        Date,
        ThingId,
        Latitude,
        Longitude,
        Velocity,
        Course
    }
}
