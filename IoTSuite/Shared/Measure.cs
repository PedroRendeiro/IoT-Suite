using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IoTSuite.Shared.Extensions;
using IoTSuite.Shared.Filters;
using Microsoft.EntityFrameworkCore;

namespace IoTSuite.Shared
{
    public class Measure
    {
        [Key, Required]
        public long Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required, ForeignKey("ThingId")]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        [Required]
        public MeasureType Type { get; set; }
        
        [Required]
        public double Value { get; set; }

        [NotMapped, Required]
        public string Unit {
            get
            {
                return Type.GetUnit();
            }
        }

        public Measure() {}

        public Measure(MeasureDTO measureDTO)
        {
            ThingId = measureDTO.ThingId;
            Type = measureDTO.Type;
            Value = measureDTO.Value;
        }

        public Measure MapFromDTO(MeasureDTO measureDTO)
        {
            ThingId = measureDTO.ThingId;
            Type = measureDTO.Type;
            Value = measureDTO.Value;

            return this;
        }
    }

    public class MeasureDTO
    {
        [Required]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        [Required]
        public MeasureType Type { get; set; }

        [Required]
        public double Value { get; set; }
    }

    public enum MeasureType
    {
        [Unit("A")]
        EnergyConsumption,
        [Unit("L/min")]
        Flow,
        [Unit("°C")]
        Temperature,
        [Unit("%")]
        Humidity,
        [Unit("°C")]
        WaterTemperature
    }

    public class MeasurePaginationFilter : PaginationFilter, IValidatableObject
    {
        public Guid? ThingId { get; set; }

        public MeasureType[] Type { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }

        public OrderByMeasure OrderBy { get; set; }

        public async Task<(IQueryable<Measure>, int)> Process(IQueryable<Measure> measures, DbSet<Thing> thingsContext, Guid userId)
        {
            var things = Thing.Filter(thingsContext, userId);
            measures = measures.Where(measure => things.Select(thing => thing.ThingId).Contains(measure.ThingId));

            if (ThingId.HasValue)
            {
                measures = measures.Where(measure => measure.ThingId.Equals(ThingId.Value));
            }

            if (Type != null)
            {
                if (Type.Any())
                {
                    measures = measures.Where(measure => Type.Contains(measure.Type));
                }
            }

            if (MinDate.HasValue)
            {
                measures = measures.Where(measure => measure.Date.CompareTo(MinDate.Value) >= 0);
            }
            if (MaxDate.HasValue)
            {
                measures = measures.Where(measure => measure.Date.CompareTo(MaxDate.Value) <= 0);
            }

            if (MinValue.HasValue)
            {
                measures = measures.Where(measure => measure.Value >= MinValue.Value);
            }
            if (MaxValue.HasValue)
            {
                measures = measures.Where(measure => measure.Value <= MaxValue.Value);
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByMeasure.Id:
                        measures = measures.OrderBy(measures => measures.Id);
                        break;
                    case OrderByMeasure.Date:
                        measures = measures.OrderBy(measures => measures.Date);
                        break;
                    case OrderByMeasure.ThingId:
                        measures = measures.OrderBy(measures => measures.ThingId);
                        break;
                    case OrderByMeasure.Type:
                        measures = measures.OrderBy(measures => measures.Type);
                        break;
                    case OrderByMeasure.Value:
                        measures = measures.OrderBy(measures => measures.Value);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByMeasure.Id:
                        measures = measures.OrderByDescending(measures => measures.Id);
                        break;
                    case OrderByMeasure.Date:
                        measures = measures.OrderByDescending(measures => measures.Date);
                        break;
                    case OrderByMeasure.ThingId:
                        measures = measures.OrderByDescending(measures => measures.ThingId);
                        break;
                    case OrderByMeasure.Type:
                        measures = measures.OrderByDescending(measures => measures.Type);
                        break;
                    case OrderByMeasure.Value:
                        measures = measures.OrderByDescending(measures => measures.Value);
                        break;
                }
            }

            var count = await measures.CountAsync();

            measures = measures.Skip((PageNumber - 1) * PageSize).Take(PageSize);

            return (measures, count);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            var results = new List<ValidationResult>();

            if (MinDate > MaxDate)
            {
                results.Add(new ValidationResult($"MaxDate must be greater than MinDate", new string[] { "MinDate", "MaxDate" }));
            }

            if (MinValue > MaxValue)
            {
                results.Add(new ValidationResult($"MaxValue must be greater than MinValue", new string[] { "MinValue", "MaxValue" }));
            }

            return results;
        }
    }

    public enum OrderByMeasure
    {
        Id,
        Date,
        ThingId,
        Type,
        Value
    }
}
