using IoTSuite.Shared.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTSuite.Shared
{
    public class Thing
    {
        [Key, Required]
        public Guid ThingId { get; set; }

        [Required]
        public Guid Owner { get; set; }

        public List<Policy> Users { get; set; }

        [Required]
        public ThingType Type { get; set; }

        [Required]
        public FeatureType Features { get; set; }

        public Thing MapFromDTO(ThingDTO thingDTO)
        {
            Type = thingDTO.Type;
            Features = thingDTO.Features;

            Users.ToList().ForEach(policy =>
            {
                if (!thingDTO.Users.Contains(policy.User))
                {
                    Users.Remove(policy);
                }
            });

            thingDTO.Users.ForEach(user =>
            {
                if (!Users.Select(policy => policy.User).Contains(user))
                {
                    Users.Add(new Policy
                    {
                        User = user
                    });
                }
            });

            return this;
        }

        public static IQueryable<Thing> Filter(DbSet<Thing> context, Guid userId)
        {
            return context.Include(thing => thing.Users)
                .Where(thing => thing.Users.Select(user => user.User).Contains(userId) || thing.Owner.Equals(userId));
        }
    }

    public class ThingDTO
    {
        public List<Guid> Users { get; set; }

        [Required]
        public ThingType Type { get; set; }

        [Required]
        public FeatureType Features { get; set; }
    }

    public class Policy
    {
        [Key, Required]
        public Guid PolicyId { get; set; }

        [Required]
        public Guid User { get; set; }

        [ForeignKey("ThingId")]
        public Guid ThingId { get; set; }
    }

    public enum ThingType
    {
        Vehicle,
        Alarm,
        EnergyMonitor,
        PowerMeter
    }

    [Flags]
    public enum FeatureType
    {
        None = 0,
        Tracker = 1 << 0,
        Alarm = 1 << 1,
        EnergyConsumption = 1 << 2,
        Flow = 1 << 3,
        Temperature = 1 << 4,
        Humidity = 1 << 5,
        WaterTemperature = 1 << 6
    }

    public class ThingPaginationFilter : PaginationFilter
    {
        public Guid? ThingId { get; set; }

        public Guid? Owner { get; set; }

        public Guid? User { get; set; }

        public ThingType? Type { get; set; }

        public FeatureType? Feature { get; set; }

        public OrderByThing OrderBy { get; set; }

        public async Task<(IQueryable<Thing>, int)> Process(IQueryable<Thing> things)
        {
            if (ThingId.HasValue)
            {
                things = things.Where(thing => thing.ThingId.Equals(ThingId.Value));
            }

            if (Owner.HasValue)
            {
                things = things.Where(thing => thing.Owner.Equals(Owner.Value));
            }

            if (Type.HasValue)
            {
                things = things.Where(thing => thing.Type.Equals(Type.Value));
            }

            if (Feature.HasValue)
            {
                things = things.Where(thing => thing.Features.HasFlag(Feature.Value));
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByThing.ThingId:
                        things = things.OrderBy(thing => thing.ThingId);
                        break;
                    case OrderByThing.Owner:
                        things = things.OrderBy(thing => thing.Owner);
                        break;
                    case OrderByThing.Type:
                        things = things.OrderBy(thing => thing.Type);
                        break;
                    case OrderByThing.Features:
                        things = things.OrderBy(thing => thing.Features);
                        break;
                    default:
                        things = things.OrderBy(thing => thing.ThingId);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByThing.ThingId:
                        things = things.OrderByDescending(thing => thing.ThingId);
                        break;
                    case OrderByThing.Owner:
                        things = things.OrderByDescending(thing => thing.Owner);
                        break;
                    case OrderByThing.Type:
                        things = things.OrderByDescending(thing => thing.Type);
                        break;
                    case OrderByThing.Features:
                        things = things.OrderByDescending(thing => thing.Features);
                        break;
                    default:
                        things = things.OrderByDescending(thing => thing.ThingId);
                        break;
                }
            }

            var count = await things.CountAsync();

            things = things.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (things, count);
        }
    }

    public enum OrderByThing
    {
        ThingId,
        Owner,
        Type,
        Features
    }

    public enum ThingActions
    {
        Start,
        Restart,
        Stop
    }
}
