using IoTSuite.Shared.Extensions;
using IoTSuite.Shared.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTSuite.Shared
{
    public class Alarm
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
        public AlarmType Type { get; set; }

        public string UID { get; set; }

        [JsonIgnore]
        public RFIDTag RFIDTag { get; set; }

        public Alarm() { }

        public Alarm(AlarmDTO alarmDTO)
        {
            ThingId = alarmDTO.ThingId;
            Type = alarmDTO.Type;
            UID = alarmDTO.UID;
        }

        public Alarm MapFromDTO(AlarmDTO alarmDTO)
        {
            ThingId = alarmDTO.ThingId;
            Type = alarmDTO.Type;
            UID = alarmDTO.UID;

            return this;
        }
    }

    public class AlarmDTO
    {

        [Required]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        [Required]
        public AlarmType Type { get; set; }

        public string UID { get; set; }

        [JsonIgnore]
        public RFIDTag RFIDTag { get; set; }
    }

    public class AlarmPaginationFilter : PaginationFilter
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public Guid ThingId { get; set; }

        public AlarmType Type { get; set; }

        public OrderByAlarm OrderBy { get; set; }

        public async Task<(IQueryable<Alarm>, int)> Process(IQueryable<Alarm> alarms)
        {
            

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByAlarm.Id:
                        alarms = alarms.OrderBy(alarm => alarm.Id);
                        break;
                    case OrderByAlarm.Date:
                        alarms = alarms.OrderBy(alarm => alarm.Date);
                        break;
                    case OrderByAlarm.ThingId:
                        alarms = alarms.OrderBy(alarm => alarm.ThingId);
                        break;
                    case OrderByAlarm.Type:
                        alarms = alarms.OrderBy(alarm => alarm.Type);
                        break;
                    default:
                        alarms = alarms.OrderBy(alarm => alarm.Id);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByAlarm.Id:
                        alarms = alarms.OrderByDescending(alarm => alarm.Id);
                        break;
                    case OrderByAlarm.Date:
                        alarms = alarms.OrderByDescending(alarm => alarm.Date);
                        break;
                    case OrderByAlarm.ThingId:
                        alarms = alarms.OrderByDescending(alarm => alarm.ThingId);
                        break;
                    case OrderByAlarm.Type:
                        alarms = alarms.OrderByDescending(alarm => alarm.Type);
                        break;
                    default:
                        alarms = alarms.OrderByDescending(alarm => alarm.Id);
                        break;
                }
            }

            var count = await alarms.CountAsync();

            alarms = alarms.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (alarms, count);
        }
    }

    public enum OrderByAlarm
    {
        Id,
        Date,
        ThingId,
        Type
    }

    public class AlarmUser
    {
        [Key, Required]
        public long Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public List<RFIDTag> RFIDTags { get; set; }
    }

    public class AlarmUserDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public List<string> RFIDTags { get; set; }
    }

    public class AlarmUserPaginationFilter : PaginationFilter
    {        
        public string Username { get; set; }

        public string RFIDTags { get; set; }

        public OrderByAlarmUser OrderBy { get; set; }

        public async Task<(IQueryable<AlarmUser>, int)> Process(IQueryable<AlarmUser> alarmUsers)
        {
            if (!string.IsNullOrEmpty(Username))
            {
                alarmUsers = alarmUsers.Where(user => user.Username.Equals(Username));
            }

            if (!string.IsNullOrEmpty(RFIDTags))
            {
                alarmUsers = alarmUsers.Where(user => user.RFIDTags.Select(tag => tag.UID).Contains(RFIDTags));
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByAlarmUser.Id:
                        alarmUsers = alarmUsers.OrderBy(user => user.Id);
                        break;
                    case OrderByAlarmUser.Username:
                        alarmUsers = alarmUsers.OrderBy(user => user.Username);
                        break;
                    case OrderByAlarmUser.RFIDTag:
                        alarmUsers = alarmUsers.OrderBy(user => user.RFIDTags);
                        break;
                    default:
                        alarmUsers = alarmUsers.OrderBy(user => user.Id);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByAlarmUser.Id:
                        alarmUsers = alarmUsers.OrderByDescending(user => user.Id);
                        break;
                    case OrderByAlarmUser.Username:
                        alarmUsers = alarmUsers.OrderByDescending(user => user.Username);
                        break;
                    case OrderByAlarmUser.RFIDTag:
                        alarmUsers = alarmUsers.OrderByDescending(user => user.RFIDTags);
                        break;
                    default:
                        alarmUsers = alarmUsers.OrderByDescending(user => user.Id);
                        break;
                }
            }

            var count = await alarmUsers.CountAsync();

            alarmUsers = alarmUsers.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (alarmUsers, count);
        }
    }

    public enum OrderByAlarmUser
    {
        Id,
        Username,
        RFIDTag
    }

    public class RFIDTag
    {        
        [Key, Required]
        public string UID { get; set; }
        
        [ForeignKey("AlarmUserId")]
        public long AlarmUserId { get; set; }
    }

    public enum AlarmType
    {
        Open,
        Close,
        Read,
        Intruder
    }
}
