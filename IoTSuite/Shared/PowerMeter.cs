using IoTSuite.Shared.Extensions;
using IoTSuite.Shared.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTSuite.Shared
{
    public class PowerMeterTotal
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
        public double A_In { get; set; }

        public double? A_Out { get; set; }

        [Required]
        public double Ri_In { get; set; }

        [Required]
        public double Rc_In { get; set; }

        public double? Ri_Out { get; set; }

        public double? Rc_Out { get; set; }

        public double? A_L1_In { get; set; }

        public double? A_L2_In { get; set; }

        public double? A_L3_In { get; set; }

        public double? A_L1_Out { get; set; }

        public double? A_L2_Out { get; set; }

        public double? A_L3_Out { get; set; }

        [Required]
        public AveragePower QI_QIV_In { get; set; }

        public AveragePower QII_QIII_Out { get; set; }

        public PowerMeterTotal() { }

        public PowerMeterTotal(PowerMeterTotalDTO powermetertotalDTO)
        {
            ThingId = powermetertotalDTO.ThingId;

            A_In = powermetertotalDTO.A_In;
            if (powermetertotalDTO.A_Out.HasValue)
            {
                A_Out = powermetertotalDTO.A_Out.Value;
            }

            Ri_In = powermetertotalDTO.Ri_In;
            if (powermetertotalDTO.Ri_Out.HasValue)
            {
                Ri_Out = powermetertotalDTO.Ri_Out.Value;
            }

            Rc_In = powermetertotalDTO.Rc_In;
            if (powermetertotalDTO.Rc_Out.HasValue)
            {
                Rc_Out = powermetertotalDTO.Rc_Out.Value;
            }

            if (powermetertotalDTO.A_L1_In.HasValue)
            {
                A_L1_In = powermetertotalDTO.A_L1_In.Value;
            }
            if (powermetertotalDTO.A_L2_In.HasValue)
            {
                A_L2_In = powermetertotalDTO.A_L2_In.Value;
            }
            if (powermetertotalDTO.A_L3_In.HasValue)
            {
                A_L3_In = powermetertotalDTO.A_L3_In.Value;
            }

            if (powermetertotalDTO.A_L1_Out.HasValue)
            {
                A_L1_Out = powermetertotalDTO.A_L1_Out.Value;
            }
            if (powermetertotalDTO.A_L2_Out.HasValue)
            {
                A_L2_Out = powermetertotalDTO.A_L2_Out.Value;
            }
            if (powermetertotalDTO.A_L3_Out.HasValue)
            {
                A_L3_Out = powermetertotalDTO.A_L3_Out.Value;
            }

            if (powermetertotalDTO.QI_QIV_In != null)
            {
                QI_QIV_In = new AveragePower(powermetertotalDTO.QI_QIV_In);
            }
            if (powermetertotalDTO.QII_QIII_Out != null)
            {
                QII_QIII_Out = new AveragePower(powermetertotalDTO.QII_QIII_Out);
            }
        }

        public PowerMeterTotal MapFromDTO(PowerMeterTotalDTO powerMeterTotalDTO)
        {
            ThingId = powerMeterTotalDTO.ThingId;

            A_In = powerMeterTotalDTO.A_In;
            if (powerMeterTotalDTO.A_Out.HasValue)
            {
                A_Out = powerMeterTotalDTO.A_Out.Value;
            }
            else
            {
                A_Out = null;
            }

            Ri_In = powerMeterTotalDTO.Ri_In;
            if (powerMeterTotalDTO.Ri_Out.HasValue)
            {
                Ri_Out = powerMeterTotalDTO.Ri_Out.Value;
            }
            else
            {
                Ri_Out = null;
            }

            Rc_In = powerMeterTotalDTO.Rc_In;
            if (powerMeterTotalDTO.Rc_Out.HasValue)
            {
                Rc_Out = powerMeterTotalDTO.Rc_Out.Value;
            }
            else
            {
                Rc_Out = null;
            }

            if (powerMeterTotalDTO.A_L1_In.HasValue)
            {
                A_L1_In = powerMeterTotalDTO.A_L1_In.Value;
            }
            else
            {
                A_L1_In = null;
            }
            if (powerMeterTotalDTO.A_L2_In.HasValue)
            {
                A_L2_In = powerMeterTotalDTO.A_L2_In.Value;
            }
            else
            {
                A_L2_In = null;
            }
            if (powerMeterTotalDTO.A_L3_In.HasValue)
            {
                A_L3_In = powerMeterTotalDTO.A_L3_In.Value;
            }
            else
            {
                A_L3_In = null;
            }

            if (powerMeterTotalDTO.A_L1_Out.HasValue)
            {
                A_L1_Out = powerMeterTotalDTO.A_L1_Out.Value;
            }
            else
            {
                A_L1_Out = null;
            }
            if (powerMeterTotalDTO.A_L2_Out.HasValue)
            {
                A_L2_Out = powerMeterTotalDTO.A_L2_Out.Value;
            }
            else
            {
                A_L2_Out = null;
            }
            if (powerMeterTotalDTO.A_L3_Out.HasValue)
            {
                A_L3_Out = powerMeterTotalDTO.A_L3_Out.Value;
            }
            else
            {
                A_L3_Out = null;
            }

            if (powerMeterTotalDTO.QI_QIV_In != null)
            {
                QI_QIV_In = new AveragePower(powerMeterTotalDTO.QI_QIV_In);
            }
            else
            {
                QI_QIV_In = null;
            }
            if (powerMeterTotalDTO.QII_QIII_Out != null)
            {
                QII_QIII_Out = new AveragePower(powerMeterTotalDTO.QII_QIII_Out);
            }
            else
            {
                QII_QIII_Out = null;
            }

            return this;
        }
    }

    public class PowerMeterTotalDTO
    {
        [Required]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        [Required]
        public double A_In { get; set; }

        public double? A_Out { get; set; }

        [Required]
        public double Ri_In { get; set; }

        [Required]
        public double Rc_In { get; set; }

        public double? Ri_Out { get; set; }

        public double? Rc_Out { get; set; }

        public double? A_L1_In { get; set; }

        public double? A_L2_In { get; set; }

        public double? A_L3_In { get; set; }

        public double? A_L1_Out { get; set; }

        public double? A_L2_Out { get; set; }

        public double? A_L3_Out { get; set; }

        public AveragePowerDTO QI_QIV_In { get; set; }

        public AveragePowerDTO QII_QIII_Out { get; set; }
    }

    public class PowerMeterTotalPaginationFilter : PaginationFilter, IValidatableObject
    {
        public Guid? ThingId { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public OrderByPowerMeterTotal OrderBy { get; set; }

        public async Task<(IQueryable<PowerMeterTotal>, int)> Process(IQueryable<PowerMeterTotal> powerMeterTotals, DbSet<Thing> thingsContext, Guid userId)
        {
            var things = Thing.Filter(thingsContext, userId);
            powerMeterTotals = powerMeterTotals.Where(powerMeterTotals => things.Select(thing => thing.ThingId).Contains(powerMeterTotals.ThingId));

            if (ThingId.HasValue)
            {
                powerMeterTotals = powerMeterTotals.Where(powerMeterTotal => powerMeterTotal.ThingId.Equals(ThingId.Value));
            }

            if (MinDate.HasValue)
            {
                powerMeterTotals = powerMeterTotals.Where(powerMeterTotal => powerMeterTotal.Date.CompareTo(MinDate.Value) >= 0);
            }
            if (MaxDate.HasValue)
            {
                powerMeterTotals = powerMeterTotals.Where(powerMeterTotal => powerMeterTotal.Date.CompareTo(MaxDate.Value) <= 0);
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByPowerMeterTotal.Id:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.Id);
                        break;
                    case OrderByPowerMeterTotal.Date:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.Date);
                        break;
                    case OrderByPowerMeterTotal.ThingId:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.ThingId);
                        break;
                    case OrderByPowerMeterTotal.A_In:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_In);
                        break;
                    case OrderByPowerMeterTotal.A_Out:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_Out);
                        break;
                    case OrderByPowerMeterTotal.Ri_In:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.Ri_In);
                        break;
                    case OrderByPowerMeterTotal.Rc_In:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.Rc_In);
                        break;
                    case OrderByPowerMeterTotal.Ri_Out:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.Ri_Out);
                        break;
                    case OrderByPowerMeterTotal.Rc_Out:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.Rc_Out);
                        break;
                    case OrderByPowerMeterTotal.A_L1_In:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_L1_In);
                        break;
                    case OrderByPowerMeterTotal.A_L2_In:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_L2_In);
                        break;
                    case OrderByPowerMeterTotal.A_L3_In:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_L3_In);
                        break;
                    case OrderByPowerMeterTotal.A_L1_Out:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_L1_Out);
                        break;
                    case OrderByPowerMeterTotal.A_L2_Out:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_L2_Out);
                        break;
                    case OrderByPowerMeterTotal.A_L3_Out:
                        powerMeterTotals = powerMeterTotals.OrderBy(powerMeterTotal => powerMeterTotal.A_L3_Out);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByPowerMeterTotal.Id:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.Id);
                        break;
                    case OrderByPowerMeterTotal.Date:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.Date);
                        break;
                    case OrderByPowerMeterTotal.ThingId:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.ThingId);
                        break;
                    case OrderByPowerMeterTotal.A_In:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_In);
                        break;
                    case OrderByPowerMeterTotal.A_Out:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_Out);
                        break;
                    case OrderByPowerMeterTotal.Ri_In:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.Ri_In);
                        break;
                    case OrderByPowerMeterTotal.Rc_In:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.Rc_In);
                        break;
                    case OrderByPowerMeterTotal.Ri_Out:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.Ri_Out);
                        break;
                    case OrderByPowerMeterTotal.Rc_Out:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.Rc_Out);
                        break;
                    case OrderByPowerMeterTotal.A_L1_In:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_L1_In);
                        break;
                    case OrderByPowerMeterTotal.A_L2_In:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_L2_In);
                        break;
                    case OrderByPowerMeterTotal.A_L3_In:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_L3_In);
                        break;
                    case OrderByPowerMeterTotal.A_L1_Out:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_L1_Out);
                        break;
                    case OrderByPowerMeterTotal.A_L2_Out:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_L2_Out);
                        break;
                    case OrderByPowerMeterTotal.A_L3_Out:
                        powerMeterTotals = powerMeterTotals.OrderByDescending(powerMeterTotal => powerMeterTotal.A_L3_Out);
                        break;
                }
            }

            var count = await powerMeterTotals.CountAsync();

            powerMeterTotals = powerMeterTotals.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (powerMeterTotals, count);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MinDate > MaxDate)
            {
                results.Add(new ValidationResult($"MaxDate must be greater than MinDate", new string[] { "MinDate", "MaxDate" }));
            }

            return results;
        }
    }

    public class PowerMeterTariff
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
        public double A_In_1 { get; set; }
        [Required]
        public double A_In_2 { get; set; }
        [Required]
        public double A_In_3 { get; set; }
        public double? A_In_4 { get; set; }
        public double? A_In_5 { get; set; }
        public double? A_In_6 { get; set; }
        [Required]
        public double A_In_T { get; set; }

        public double? A_Out_1 { get; set; }
        public double? A_Out_2 { get; set; }
        public double? A_Out_3 { get; set; }
        public double? A_Out_4 { get; set; }
        public double? A_Out_5 { get; set; }
        public double? A_Out_6 { get; set; }
        public double? A_Out_T { get; set; }

        [Required]
        public double Ri_In_1 { get; set; }
        [Required]
        public double Ri_In_2 { get; set; }
        [Required]
        public double Ri_In_3 { get; set; }
        public double? Ri_In_4 { get; set; }
        public double? Ri_In_5 { get; set; }
        public double? Ri_In_6 { get; set; }
        [Required]
        public double Ri_In_T { get; set; }

        [Required]
        public double Rc_In_1 { get; set; }
        [Required]
        public double Rc_In_2 { get; set; }
        [Required]
        public double Rc_In_3 { get; set; }
        public double? Rc_In_4 { get; set; }
        public double? Rc_In_5 { get; set; }
        public double? Rc_In_6 { get; set; }
        [Required]
        public double Rc_In_T { get; set; }

        public double? Ri_Out_1 { get; set; }
        public double? Ri_Out_2 { get; set; }
        public double? Ri_Out_3 { get; set; }
        public double? Ri_Out_4 { get; set; }
        public double? Ri_Out_5 { get; set; }
        public double? Ri_Out_6 { get; set; }
        public double? Ri_Out_T { get; set; }

        public double? Rc_Out_1 { get; set; }
        public double? Rc_Out_2 { get; set; }
        public double? Rc_Out_3 { get; set; }
        public double? Rc_Out_4 { get; set; }
        public double? Rc_Out_5 { get; set; }
        public double? Rc_Out_6 { get; set; }
        public double? Rc_Out_T { get; set; }

        [Required]
        public AveragePower P_In_MAX_1 { get; set; }
        [Required]
        public AveragePower P_In_MAX_2 { get; set; }
        [Required]
        public AveragePower P_In_MAX_3 { get; set; }
        public AveragePower P_In_MAX_4 { get; set; }
        public AveragePower P_In_MAX_5 { get; set; }
        public AveragePower P_In_MAX_6 { get; set; }
        [Required]
        public AveragePower P_In_MAX_T { get; set; }

        public AveragePower P_Out_MAX_1 { get; set; }
        public AveragePower P_Out_MAX_2 { get; set; }
        public AveragePower P_Out_MAX_3 { get; set; }
        public AveragePower P_Out_MAX_4 { get; set; }
        public AveragePower P_Out_MAX_5 { get; set; }
        public AveragePower P_Out_MAX_6 { get; set; }
        public AveragePower P_Out_MAX_T { get; set; }

        public PowerMeterTariff() { }

        public PowerMeterTariff(PowerMeterTariffDTO powerMeterTariffDTO)
        {
            ThingId = powerMeterTariffDTO.ThingId;

            A_In_1 = powerMeterTariffDTO.A_In_1;
            A_In_2 = powerMeterTariffDTO.A_In_2;
            A_In_3 = powerMeterTariffDTO.A_In_3;
            if (powerMeterTariffDTO.A_In_4.HasValue)
            {
                A_In_4 = powerMeterTariffDTO.A_In_4.Value;
            }
            if (powerMeterTariffDTO.A_In_5.HasValue)
            {
                A_In_5 = powerMeterTariffDTO.A_In_5.Value;
            }
            if (powerMeterTariffDTO.A_In_6.HasValue)
            {
                A_In_6 = powerMeterTariffDTO.A_In_6.Value;
            }
            A_In_T = powerMeterTariffDTO.A_In_T;

            Ri_In_1 = powerMeterTariffDTO.Ri_In_1;
            Ri_In_2 = powerMeterTariffDTO.Ri_In_2;
            Ri_In_3 = powerMeterTariffDTO.Ri_In_3;
            if (powerMeterTariffDTO.Ri_In_4.HasValue)
            {
                Ri_In_4 = powerMeterTariffDTO.Ri_In_4.Value;
            }
            if (powerMeterTariffDTO.Ri_In_5.HasValue)
            {
                Ri_In_5 = powerMeterTariffDTO.Ri_In_5.Value;
            }
            if (powerMeterTariffDTO.Ri_In_6.HasValue)
            {
                Ri_In_6 = powerMeterTariffDTO.Ri_In_6.Value;
            }
            Ri_In_T = powerMeterTariffDTO.Ri_In_T;

            Rc_In_1 = powerMeterTariffDTO.Rc_In_1;
            Rc_In_2 = powerMeterTariffDTO.Rc_In_2;
            Rc_In_3 = powerMeterTariffDTO.Rc_In_3;
            if (powerMeterTariffDTO.Rc_In_4.HasValue)
            {
                Rc_In_4 = powerMeterTariffDTO.Rc_In_4.Value;
            }
            if (powerMeterTariffDTO.Rc_In_5.HasValue)
            {
                Rc_In_5 = powerMeterTariffDTO.Rc_In_5.Value;
            }
            if (powerMeterTariffDTO.Rc_In_6.HasValue)
            {
                Rc_In_6 = powerMeterTariffDTO.Rc_In_6.Value;
            }
            Rc_In_T = powerMeterTariffDTO.Rc_In_T;

            if (powerMeterTariffDTO.Ri_Out_1.HasValue)
            {
                Ri_Out_1 = powerMeterTariffDTO.Ri_Out_1.Value;
            }
            if (powerMeterTariffDTO.Ri_Out_2.HasValue)
            {
                Ri_Out_2 = powerMeterTariffDTO.Ri_Out_2.Value;
            }
            if (powerMeterTariffDTO.Ri_Out_3.HasValue)
            {
                Ri_Out_3 = powerMeterTariffDTO.Ri_Out_3.Value;
            }
            if (powerMeterTariffDTO.Ri_Out_4.HasValue)
            {
                Ri_Out_4 = powerMeterTariffDTO.Ri_Out_4.Value;
            }
            if (powerMeterTariffDTO.Ri_Out_5.HasValue)
            {
                Ri_Out_5 = powerMeterTariffDTO.Ri_Out_5.Value;
            }
            if (powerMeterTariffDTO.Ri_Out_6.HasValue)
            {
                Ri_Out_6 = powerMeterTariffDTO.Ri_Out_6.Value;
            }
            Ri_Out_T = powerMeterTariffDTO.Ri_Out_T;

            if (powerMeterTariffDTO.Rc_Out_1.HasValue)
            {
                Rc_Out_1 = powerMeterTariffDTO.Rc_Out_1.Value;
            }
            if (powerMeterTariffDTO.Rc_Out_2.HasValue)
            {
                Rc_Out_2 = powerMeterTariffDTO.Rc_Out_2.Value;
            }
            if (powerMeterTariffDTO.Rc_Out_3.HasValue)
            {
                Rc_Out_3 = powerMeterTariffDTO.Rc_Out_3.Value;
            }
            if (powerMeterTariffDTO.Rc_Out_4.HasValue)
            {
                Rc_Out_4 = powerMeterTariffDTO.Rc_Out_4.Value;
            }
            if (powerMeterTariffDTO.Rc_Out_5.HasValue)
            {
                Rc_Out_5 = powerMeterTariffDTO.Rc_Out_5.Value;
            }
            if (powerMeterTariffDTO.Rc_Out_6.HasValue)
            {
                Rc_Out_6 = powerMeterTariffDTO.Rc_Out_6.Value;
            }
            Rc_Out_T = powerMeterTariffDTO.Rc_Out_T;

            P_In_MAX_1 = new AveragePower(powerMeterTariffDTO.P_In_MAX_1);
            P_In_MAX_2 = new AveragePower(powerMeterTariffDTO.P_In_MAX_2);
            P_In_MAX_3 = new AveragePower(powerMeterTariffDTO.P_In_MAX_3);
            if (powerMeterTariffDTO.P_In_MAX_4 != null)
            {
                P_In_MAX_4 = new AveragePower(powerMeterTariffDTO.P_In_MAX_4);
            }
            if (powerMeterTariffDTO.P_In_MAX_5 != null)
            {
                P_In_MAX_5 = new AveragePower(powerMeterTariffDTO.P_In_MAX_5);
            }
            if (powerMeterTariffDTO.P_In_MAX_6 != null)
            {
                P_In_MAX_6 = new AveragePower(powerMeterTariffDTO.P_In_MAX_6);
            }
            P_In_MAX_T = new AveragePower(powerMeterTariffDTO.P_In_MAX_T);

            if (powerMeterTariffDTO.P_Out_MAX_1 != null)
            {
                P_Out_MAX_1 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_1);
            }
            if (powerMeterTariffDTO.P_Out_MAX_2 != null)
            {
                P_Out_MAX_2 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_2);
            }
            if (powerMeterTariffDTO.P_Out_MAX_3 != null)
            {
                P_Out_MAX_3 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_3);
            }
            if (powerMeterTariffDTO.P_Out_MAX_4 != null)
            {
                P_Out_MAX_4 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_4);
            }
            if (powerMeterTariffDTO.P_Out_MAX_5 != null)
            {
                P_Out_MAX_5 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_5);
            }
            if (powerMeterTariffDTO.P_Out_MAX_6 != null)
            {
                P_Out_MAX_6 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_6);
            }
            if (powerMeterTariffDTO.P_Out_MAX_T != null)
            {
                P_Out_MAX_T = new AveragePower(powerMeterTariffDTO.P_Out_MAX_T);
            }
        }

        public PowerMeterTariff MapFromDTO(PowerMeterTariffDTO powerMeterTariffDTO)
        {
            ThingId = powerMeterTariffDTO.ThingId;

            A_In_1 = powerMeterTariffDTO.A_In_1;
            A_In_2 = powerMeterTariffDTO.A_In_2;
            A_In_3 = powerMeterTariffDTO.A_In_3;
            if (powerMeterTariffDTO.A_In_4.HasValue)
            {
                A_In_4 = powerMeterTariffDTO.A_In_4.Value;
            }
            else
            {
                A_In_4 = null;
            }
            if (powerMeterTariffDTO.A_In_5.HasValue)
            {
                A_In_5 = powerMeterTariffDTO.A_In_5.Value;
            }
            else
            {
                A_In_5 = null;
            }
            if (powerMeterTariffDTO.A_In_6.HasValue)
            {
                A_In_6 = powerMeterTariffDTO.A_In_6.Value;
            }
            else
            {
                A_In_6 = null;
            }
            A_In_T = powerMeterTariffDTO.A_In_T;

            Ri_In_1 = powerMeterTariffDTO.Ri_In_1;
            Ri_In_2 = powerMeterTariffDTO.Ri_In_2;
            Ri_In_3 = powerMeterTariffDTO.Ri_In_3;
            if (powerMeterTariffDTO.Ri_In_4.HasValue)
            {
                Ri_In_4 = powerMeterTariffDTO.Ri_In_4.Value;
            }
            else
            {
                Ri_In_4 = null;
            }
            if (powerMeterTariffDTO.Ri_In_5.HasValue)
            {
                Ri_In_5 = powerMeterTariffDTO.Ri_In_5.Value;
            }
            else
            {
                Ri_In_5 = null;
            }
            if (powerMeterTariffDTO.Ri_In_6.HasValue)
            {
                Ri_In_6 = powerMeterTariffDTO.Ri_In_6.Value;
            }
            else
            {
                Ri_In_6 = null;
            }
            Ri_In_T = powerMeterTariffDTO.Ri_In_T;

            Rc_In_1 = powerMeterTariffDTO.Rc_In_1;
            Rc_In_2 = powerMeterTariffDTO.Rc_In_2;
            Rc_In_3 = powerMeterTariffDTO.Rc_In_3;
            if (powerMeterTariffDTO.Rc_In_4.HasValue)
            {
                Rc_In_4 = powerMeterTariffDTO.Rc_In_4.Value;
            }
            else
            {
                Rc_In_4 = null;
            }
            if (powerMeterTariffDTO.Rc_In_5.HasValue)
            {
                Rc_In_5 = powerMeterTariffDTO.Rc_In_5.Value;
            }
            else
            {
                Rc_In_5 = null;
            }
            if (powerMeterTariffDTO.Rc_In_6.HasValue)
            {
                Rc_In_6 = powerMeterTariffDTO.Rc_In_6.Value;
            }
            else
            {
                Rc_In_6 = null;
            }
            Rc_In_T = powerMeterTariffDTO.Rc_In_T;

            if (powerMeterTariffDTO.Ri_Out_1.HasValue)
            {
                Ri_Out_1 = powerMeterTariffDTO.Ri_Out_1.Value;
            }
            else
            {
                Ri_Out_1 = null;
            }
            if (powerMeterTariffDTO.Ri_Out_2.HasValue)
            {
                Ri_Out_2 = powerMeterTariffDTO.Ri_Out_2.Value;
            }
            else
            {
                Ri_Out_2 = null;
            }
            if (powerMeterTariffDTO.Ri_Out_3.HasValue)
            {
                Ri_Out_3 = powerMeterTariffDTO.Ri_Out_3.Value;
            }
            else
            {
                Ri_Out_3 = null;
            }
            if (powerMeterTariffDTO.Ri_Out_4.HasValue)
            {
                Ri_Out_4 = powerMeterTariffDTO.Ri_Out_4.Value;
            }
            else
            {
                Ri_Out_4 = null;
            }
            if (powerMeterTariffDTO.Ri_Out_5.HasValue)
            {
                Ri_Out_5 = powerMeterTariffDTO.Ri_Out_5.Value;
            }
            else
            {
                Ri_Out_5 = null;
            }
            if (powerMeterTariffDTO.Ri_Out_6.HasValue)
            {
                Ri_Out_6 = powerMeterTariffDTO.Ri_Out_6.Value;
            }
            else
            {
                Ri_Out_6 = null;
            }
            Ri_Out_T = powerMeterTariffDTO.Ri_Out_T;

            if (powerMeterTariffDTO.Rc_Out_1.HasValue)
            {
                Rc_Out_1 = powerMeterTariffDTO.Rc_Out_1.Value;
            }
            else
            {
                Rc_Out_1 = null;
            }
            if (powerMeterTariffDTO.Rc_Out_2.HasValue)
            {
                Rc_Out_2 = powerMeterTariffDTO.Rc_Out_2.Value;
            }
            else
            {
                Rc_Out_2 = null;
            }
            if (powerMeterTariffDTO.Rc_Out_3.HasValue)
            {
                Rc_Out_3 = powerMeterTariffDTO.Rc_Out_3.Value;
            }
            else
            {
                Rc_Out_3 = null;
            }
            if (powerMeterTariffDTO.Rc_Out_4.HasValue)
            {
                Rc_Out_4 = powerMeterTariffDTO.Rc_Out_4.Value;
            }
            else
            {
                Rc_Out_4 = null;
            }
            if (powerMeterTariffDTO.Rc_Out_5.HasValue)
            {
                Rc_Out_5 = powerMeterTariffDTO.Rc_Out_5.Value;
            }
            else
            {
                Rc_Out_5 = null;
            }
            if (powerMeterTariffDTO.Rc_Out_6.HasValue)
            {
                Rc_Out_6 = powerMeterTariffDTO.Rc_Out_6.Value;
            }
            else
            {
                Rc_Out_6 = null;
            }
            Rc_Out_T = powerMeterTariffDTO.Rc_Out_T;

            P_In_MAX_1 = new AveragePower(powerMeterTariffDTO.P_In_MAX_1);
            P_In_MAX_2 = new AveragePower(powerMeterTariffDTO.P_In_MAX_2);
            P_In_MAX_3 = new AveragePower(powerMeterTariffDTO.P_In_MAX_3);
            if (powerMeterTariffDTO.P_In_MAX_4 != null)
            {
                P_In_MAX_4 = new AveragePower(powerMeterTariffDTO.P_In_MAX_4);
            }
            else
            {
                P_In_MAX_4 = null;
            }
            if (powerMeterTariffDTO.P_In_MAX_5 != null)
            {
                P_In_MAX_5 = new AveragePower(powerMeterTariffDTO.P_In_MAX_5);
            }
            else
            {
                P_In_MAX_5 = null;
            }
            if (powerMeterTariffDTO.P_In_MAX_6 != null)
            {
                P_In_MAX_6 = new AveragePower(powerMeterTariffDTO.P_In_MAX_6);
            }
            else
            {
                P_In_MAX_6 = null;
            }
            P_In_MAX_T = new AveragePower(powerMeterTariffDTO.P_In_MAX_T);

            if (powerMeterTariffDTO.P_Out_MAX_1 != null)
            {
                P_Out_MAX_1 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_1);
            }
            else
            {
                P_Out_MAX_1 = null;
            }
            if (powerMeterTariffDTO.P_Out_MAX_2 != null)
            {
                P_Out_MAX_2 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_2);
            }
            else
            {
                P_Out_MAX_2 = null;
            }
            if (powerMeterTariffDTO.P_Out_MAX_3 != null)
            {
                P_Out_MAX_3 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_3);
            }
            else
            {
                P_Out_MAX_3 = null;
            }
            if (powerMeterTariffDTO.P_Out_MAX_4 != null)
            {
                P_Out_MAX_4 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_4);
            }
            else
            {
                P_Out_MAX_4 = null;
            }
            if (powerMeterTariffDTO.P_Out_MAX_5 != null)
            {
                P_Out_MAX_5 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_5);
            }
            else
            {
                P_Out_MAX_5 = null;
            }
            if (powerMeterTariffDTO.P_Out_MAX_6 != null)
            {
                P_Out_MAX_6 = new AveragePower(powerMeterTariffDTO.P_Out_MAX_6);
            }
            else
            {
                P_Out_MAX_6 = null;
            }
            if (powerMeterTariffDTO.P_Out_MAX_T != null)
            {
                P_Out_MAX_T = new AveragePower(powerMeterTariffDTO.P_Out_MAX_T);
            }
            else
            {
                P_Out_MAX_T = null;
            }

            return this;
        }
    }

    public class PowerMeterTariffDTO
    {

        [Required]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        [Required]
        public double A_In_1 { get; set; }
        [Required]
        public double A_In_2 { get; set; }
        [Required]
        public double A_In_3 { get; set; }
        public double? A_In_4 { get; set; }
        public double? A_In_5 { get; set; }
        public double? A_In_6 { get; set; }
        [Required]
        public double A_In_T { get; set; }

        public double? A_Out_1 { get; set; }
        public double? A_Out_2 { get; set; }
        public double? A_Out_3 { get; set; }
        public double? A_Out_4 { get; set; }
        public double? A_Out_5 { get; set; }
        public double? A_Out_6 { get; set; }
        public double? A_Out_T { get; set; }

        [Required]
        public double Ri_In_1 { get; set; }
        [Required]
        public double Ri_In_2 { get; set; }
        [Required]
        public double Ri_In_3 { get; set; }
        public double? Ri_In_4 { get; set; }
        public double? Ri_In_5 { get; set; }
        public double? Ri_In_6 { get; set; }
        [Required]
        public double Ri_In_T { get; set; }

        [Required]
        public double Rc_In_1 { get; set; }
        [Required]
        public double Rc_In_2 { get; set; }
        [Required]
        public double Rc_In_3 { get; set; }
        public double? Rc_In_4 { get; set; }
        public double? Rc_In_5 { get; set; }
        public double? Rc_In_6 { get; set; }
        [Required]
        public double Rc_In_T { get; set; }

        public double? Ri_Out_1 { get; set; }
        public double? Ri_Out_2 { get; set; }
        public double? Ri_Out_3 { get; set; }
        public double? Ri_Out_4 { get; set; }
        public double? Ri_Out_5 { get; set; }
        public double? Ri_Out_6 { get; set; }
        public double? Ri_Out_T { get; set; }

        public double? Rc_Out_1 { get; set; }
        public double? Rc_Out_2 { get; set; }
        public double? Rc_Out_3 { get; set; }
        public double? Rc_Out_4 { get; set; }
        public double? Rc_Out_5 { get; set; }
        public double? Rc_Out_6 { get; set; }
        public double? Rc_Out_T { get; set; }

        [Required]
        public AveragePowerDTO P_In_MAX_1 { get; set; }
        [Required]
        public AveragePowerDTO P_In_MAX_2 { get; set; }
        [Required]
        public AveragePowerDTO P_In_MAX_3 { get; set; }
        public AveragePowerDTO P_In_MAX_4 { get; set; }
        public AveragePowerDTO P_In_MAX_5 { get; set; }
        public AveragePowerDTO P_In_MAX_6 { get; set; }
        [Required]
        public AveragePowerDTO P_In_MAX_T { get; set; }

        public AveragePowerDTO P_Out_MAX_1 { get; set; }
        public AveragePowerDTO P_Out_MAX_2 { get; set; }
        public AveragePowerDTO P_Out_MAX_3 { get; set; }
        public AveragePowerDTO P_Out_MAX_4 { get; set; }
        public AveragePowerDTO P_Out_MAX_5 { get; set; }
        public AveragePowerDTO P_Out_MAX_6 { get; set; }
        public AveragePowerDTO P_Out_MAX_T { get; set; }
    }

    public class PowerMeterTariffPaginationFilter : PaginationFilter, IValidatableObject
    {
        public Guid? ThingId { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public OrderByPowerMeterTariff OrderBy { get; set; }

        public async Task<(IQueryable<PowerMeterTariff>, int)> Process(IQueryable<PowerMeterTariff> powerMeterTariffs, DbSet<Thing> thingsContext, Guid userId)
        {
            var things = Thing.Filter(thingsContext, userId);
            powerMeterTariffs = powerMeterTariffs.Where(powerMeterTariffs => things.Select(thing => thing.ThingId).Contains(powerMeterTariffs.ThingId));

            if (ThingId.HasValue)
            {
                powerMeterTariffs = powerMeterTariffs.Where(powerMeterTariff => powerMeterTariff.ThingId.Equals(ThingId.Value));
            }

            if (MinDate.HasValue)
            {
                powerMeterTariffs = powerMeterTariffs.Where(powerMeterTariff => powerMeterTariff.Date.CompareTo(MinDate.Value) >= 0);
            }
            if (MaxDate.HasValue)
            {
                powerMeterTariffs = powerMeterTariffs.Where(powerMeterTariff => powerMeterTariff.Date.CompareTo(MaxDate.Value) <= 0);
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByPowerMeterTariff.Id:
                        powerMeterTariffs = powerMeterTariffs.OrderBy(powerMeterTariff => powerMeterTariff.Id);
                        break;
                    case OrderByPowerMeterTariff.Date:
                        powerMeterTariffs = powerMeterTariffs.OrderBy(powerMeterTariff => powerMeterTariff.Date);
                        break;
                    case OrderByPowerMeterTariff.ThingId:
                        powerMeterTariffs = powerMeterTariffs.OrderBy(powerMeterTariff => powerMeterTariff.ThingId);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByPowerMeterTariff.Id:
                        powerMeterTariffs = powerMeterTariffs.OrderByDescending(powerMeterTariff => powerMeterTariff.Id);
                        break;
                    case OrderByPowerMeterTariff.Date:
                        powerMeterTariffs = powerMeterTariffs.OrderByDescending(powerMeterTariff => powerMeterTariff.Date);
                        break;
                    case OrderByPowerMeterTariff.ThingId:
                        powerMeterTariffs = powerMeterTariffs.OrderByDescending(powerMeterTariff => powerMeterTariff.ThingId);
                        break;
                }
            }

            var count = await powerMeterTariffs.CountAsync();

            powerMeterTariffs = powerMeterTariffs.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (powerMeterTariffs, count);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MinDate > MaxDate)
            {
                results.Add(new ValidationResult($"MaxDate must be greater than MinDate", new string[] { "MinDate", "MaxDate" }));
            }

            return results;
        }
    }

    public class PowerMeterInstantaneous
    {
        [Key, Required]
        public long Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required, ForeignKey("ThingId")]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        [Unit("V")]
        public double V_L1 { get; set; }
        [Unit("A")]
        public double I_L1 { get; set; }
        [Unit("V")]
        public double? V_L2 { get; set; }
        [Unit("A")]
        public double? I_L2 { get; set; }
        [Unit("V")]
        public double? V_L3 { get; set; }
        [Unit("A")]
        public double? I_L3 { get; set; }
        [Unit("W")]
        public double? P_L1_In { get; set; }
        [Unit("W")]
        public double? P_L1_Out { get; set; }
        [Unit("W")]
        public double? P_L2_In { get; set; }
        [Unit("W")]
        public double? P_L2_Out { get; set; }
        [Unit("W")]
        public double? P_L3_In { get; set; }
        [Unit("W")]
        public double? P_L3_Out { get; set; }
        [Unit("W")]
        public double P_In { get; set; }
        [Unit("W")]
        public double? P_Out { get; set; }
        
        public double PF_T { get; set; }

        public double? PF_L1 { get; set; }

        public double? PF_L2 { get; set; }

        public double? PF_L3 { get; set; }
        [Unit("Hz")]
        public double F { get; set; }

        public PowerMeterInstantaneous() { }

        public PowerMeterInstantaneous(PowerMeterInstantaneousDTO powerMeterInstantaneousDTO)
        {
            ThingId = powerMeterInstantaneousDTO.ThingId;
            V_L1 = powerMeterInstantaneousDTO.V_L1;
            I_L1 = powerMeterInstantaneousDTO.I_L1;

            if (powerMeterInstantaneousDTO.V_L2.HasValue)
            {
                V_L2 = powerMeterInstantaneousDTO.V_L2.Value;
            }
            if (powerMeterInstantaneousDTO.I_L2.HasValue)
            {
                I_L2 = powerMeterInstantaneousDTO.I_L2.Value;
            }

            if (powerMeterInstantaneousDTO.V_L3.HasValue)
            {
                V_L3 = powerMeterInstantaneousDTO.V_L3.Value;
            }
            if (powerMeterInstantaneousDTO.I_L3.HasValue)
            {
                I_L3 = powerMeterInstantaneousDTO.I_L3.Value;
            }

            if (powerMeterInstantaneousDTO.P_L1_In.HasValue)
            {
                P_L1_In = powerMeterInstantaneousDTO.P_L1_In.Value;
            }
            if (powerMeterInstantaneousDTO.P_L1_Out.HasValue)
            {
                P_L1_Out = powerMeterInstantaneousDTO.P_L1_Out.Value;
            }

            if (powerMeterInstantaneousDTO.P_L2_In.HasValue)
            {
                P_L2_In = powerMeterInstantaneousDTO.P_L2_In.Value;
            }
            if (powerMeterInstantaneousDTO.P_L2_Out.HasValue)
            {
                P_L2_Out = powerMeterInstantaneousDTO.P_L2_Out.Value;
            }

            if (powerMeterInstantaneousDTO.P_L3_In.HasValue)
            {
                P_L3_In = powerMeterInstantaneousDTO.P_L3_In.Value;
            }
            if (powerMeterInstantaneousDTO.P_L3_Out.HasValue)
            {
                P_L3_Out = powerMeterInstantaneousDTO.P_L3_Out.Value;
            }

            P_In = powerMeterInstantaneousDTO.P_In;
            if (powerMeterInstantaneousDTO.P_Out.HasValue)
            {
                P_Out = powerMeterInstantaneousDTO.P_Out.Value;
            }

            PF_T = powerMeterInstantaneousDTO.PF_T;
            if (powerMeterInstantaneousDTO.PF_L1.HasValue)
            {
                PF_L1 = powerMeterInstantaneousDTO.PF_L1.Value;
            }
            if (powerMeterInstantaneousDTO.PF_L2.HasValue)
            {
                PF_L2 = powerMeterInstantaneousDTO.PF_L2.Value;
            }
            if (powerMeterInstantaneousDTO.PF_L3.HasValue)
            {
                PF_L3 = powerMeterInstantaneousDTO.PF_L3.Value;
            }

            F = powerMeterInstantaneousDTO.F;
        }

        public PowerMeterInstantaneous MapFromDTO(PowerMeterInstantaneousDTO powerMeterInstantaneousDTO)
        {
            ThingId = powerMeterInstantaneousDTO.ThingId;
            V_L1 = powerMeterInstantaneousDTO.V_L1;
            I_L1 = powerMeterInstantaneousDTO.I_L1;

            if (powerMeterInstantaneousDTO.V_L2.HasValue)
            {
                V_L2 = powerMeterInstantaneousDTO.V_L2.Value;
            }
            else
            {
                V_L2 = null;
            }
            if (powerMeterInstantaneousDTO.I_L2.HasValue)
            {
                I_L2 = powerMeterInstantaneousDTO.I_L2.Value;
            }
            else
            {
                I_L2 = null;
            }

            if (powerMeterInstantaneousDTO.V_L3.HasValue)
            {
                V_L3 = powerMeterInstantaneousDTO.V_L3.Value;
            }
            else
            {
                V_L3 = null;
            }
            if (powerMeterInstantaneousDTO.I_L3.HasValue)
            {
                I_L3 = powerMeterInstantaneousDTO.I_L3.Value;
            }
            else
            {
                I_L3 = null;
            }

            if (powerMeterInstantaneousDTO.P_L1_In.HasValue)
            {
                P_L1_In = powerMeterInstantaneousDTO.P_L1_In.Value;
            }
            else
            {
                P_L1_In = null;
            }
            if (powerMeterInstantaneousDTO.P_L1_Out.HasValue)
            {
                P_L1_Out = powerMeterInstantaneousDTO.P_L1_Out.Value;
            }
            else
            {
                P_L1_Out = null;
            }

            if (powerMeterInstantaneousDTO.P_L2_In.HasValue)
            {
                P_L2_In = powerMeterInstantaneousDTO.P_L2_In.Value;
            }
            else
            {
                P_L2_In = null;
            }
            if (powerMeterInstantaneousDTO.P_L2_Out.HasValue)
            {
                P_L2_Out = powerMeterInstantaneousDTO.P_L2_Out.Value;
            }
            else
            {
                P_L2_Out = null;
            }

            if (powerMeterInstantaneousDTO.P_L3_In.HasValue)
            {
                P_L3_In = powerMeterInstantaneousDTO.P_L3_In.Value;
            }
            else
            {
                P_L3_In = null;
            }
            if (powerMeterInstantaneousDTO.P_L3_Out.HasValue)
            {
                P_L3_Out = powerMeterInstantaneousDTO.P_L3_Out.Value;
            }
            else
            {
                P_L3_Out = null;
            }

            P_In = powerMeterInstantaneousDTO.P_In;
            if (powerMeterInstantaneousDTO.P_Out.HasValue)
            {
                P_Out = powerMeterInstantaneousDTO.P_Out.Value;
            }
            else
            {
                P_Out = null;
            }

            PF_T = powerMeterInstantaneousDTO.PF_T;
            if (powerMeterInstantaneousDTO.PF_L1.HasValue)
            {
                PF_L1 = powerMeterInstantaneousDTO.PF_L1.Value;
            }
            else
            {
                PF_L1 = null;
            }
            if (powerMeterInstantaneousDTO.PF_L2.HasValue)
            {
                PF_L2 = powerMeterInstantaneousDTO.PF_L2.Value;
            }
            else
            {
                PF_L2 = null;
            }
            if (powerMeterInstantaneousDTO.PF_L3.HasValue)
            {
                PF_L3 = powerMeterInstantaneousDTO.PF_L3.Value;
            }
            else
            {
                PF_L3 = null;
            }

            F = powerMeterInstantaneousDTO.F;

            return this;
        }
    }

    public class PowerMeterInstantaneousDTO
    {
        [Required]
        public Guid ThingId { get; set; }

        [JsonIgnore]
        public Thing Thing { get; private set; }

        public double V_L1 { get; set; }

        public double I_L1 { get; set; }

        public double? V_L2 { get; set; }

        public double? I_L2 { get; set; }

        public double? V_L3 { get; set; }

        public double? I_L3 { get; set; }

        public double? P_L1_In { get; set; }

        public double? P_L1_Out { get; set; }

        public double? P_L2_In { get; set; }

        public double? P_L2_Out { get; set; }

        public double? P_L3_In { get; set; }

        public double? P_L3_Out { get; set; }

        public double P_In { get; set; }

        public double? P_Out { get; set; }

        public double PF_T { get; set; }

        public double? PF_L1 { get; set; }

        public double? PF_L2 { get; set; }

        public double? PF_L3 { get; set; }

        public double F { get; set; }

    }

    public class PowerMeterInstantaneousPaginationFilter : PaginationFilter, IValidatableObject
    {
        public Guid? ThingId { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public OrderByPowerMeterInstantaneous OrderBy { get; set; }

        public async Task<(IQueryable<PowerMeterInstantaneous>, int)> Process(IQueryable<PowerMeterInstantaneous> powerMeterInstantaneous, DbSet<Thing> thingsContext, Guid userId)
        {
            var things = Thing.Filter(thingsContext, userId);
            powerMeterInstantaneous = powerMeterInstantaneous.Where(powerMeterInstantaneous => things.Select(thing => thing.ThingId).Contains(powerMeterInstantaneous.ThingId));

            if (ThingId.HasValue)
            {
                powerMeterInstantaneous = powerMeterInstantaneous.Where(powerMeterInstantaneous => powerMeterInstantaneous.ThingId.Equals(ThingId.Value));
            }

            if (MinDate.HasValue)
            {
                powerMeterInstantaneous = powerMeterInstantaneous.Where(powerMeterInstantaneous => powerMeterInstantaneous.Date.CompareTo(MinDate.Value) >= 0);
            }
            if (MaxDate.HasValue)
            {
                powerMeterInstantaneous = powerMeterInstantaneous.Where(powerMeterInstantaneous => powerMeterInstantaneous.Date.CompareTo(MaxDate.Value) <= 0);
            }

            if (Order.Equals(Order.Asc))
            {
                switch (OrderBy)
                {
                    case OrderByPowerMeterInstantaneous.Id:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.Id);
                        break;
                    case OrderByPowerMeterInstantaneous.Date:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.Date);
                        break;
                    case OrderByPowerMeterInstantaneous.ThingId:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.ThingId);
                        break;
                    case OrderByPowerMeterInstantaneous.V_L1:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.V_L1);
                        break;
                    case OrderByPowerMeterInstantaneous.I_L1:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.I_L1);
                        break;
                    case OrderByPowerMeterInstantaneous.V_L2:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.V_L2);
                        break;
                    case OrderByPowerMeterInstantaneous.I_L2:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.I_L2);
                        break;
                    case OrderByPowerMeterInstantaneous.V_L3:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.V_L3);
                        break;
                    case OrderByPowerMeterInstantaneous.I_L3:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.I_L3);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L1_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_L1_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L2_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_L2_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L3_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_L3_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L1_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_L1_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L2_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_L2_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L3_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_L3_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.P_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.P_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_L1:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.PF_L1);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_L2:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.PF_L2);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_L3:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.PF_L3);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_T:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.PF_T);
                        break;
                    case OrderByPowerMeterInstantaneous.F:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderBy(powerMeterInstantaneous => powerMeterInstantaneous.F);
                        break;
                }
            }
            else if (Order.Equals(Order.Desc))
            {
                switch (OrderBy)
                {
                    case OrderByPowerMeterInstantaneous.Id:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.Id);
                        break;
                    case OrderByPowerMeterInstantaneous.Date:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.Date);
                        break;
                    case OrderByPowerMeterInstantaneous.ThingId:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.ThingId);
                        break;
                    case OrderByPowerMeterInstantaneous.V_L1:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.V_L1);
                        break;
                    case OrderByPowerMeterInstantaneous.I_L1:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.I_L1);
                        break;
                    case OrderByPowerMeterInstantaneous.V_L2:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.V_L2);
                        break;
                    case OrderByPowerMeterInstantaneous.I_L2:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.I_L2);
                        break;
                    case OrderByPowerMeterInstantaneous.V_L3:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.V_L3);
                        break;
                    case OrderByPowerMeterInstantaneous.I_L3:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.I_L3);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L1_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_L1_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L2_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_L2_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L3_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_L3_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L1_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_L1_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L2_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_L2_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.P_L3_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_L3_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.P_In:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_In);
                        break;
                    case OrderByPowerMeterInstantaneous.P_Out:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.P_Out);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_L1:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.PF_L1);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_L2:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.PF_L2);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_L3:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.PF_L3);
                        break;
                    case OrderByPowerMeterInstantaneous.PF_T:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.PF_T);
                        break;
                    case OrderByPowerMeterInstantaneous.F:
                        powerMeterInstantaneous = powerMeterInstantaneous.OrderByDescending(powerMeterInstantaneous => powerMeterInstantaneous.F);
                        break;
                }
            }

            var count = await powerMeterInstantaneous.CountAsync();

            powerMeterInstantaneous = powerMeterInstantaneous.Skip((PageNumber - 1) * PageSize).Take(PageSize); ;

            return (powerMeterInstantaneous, count);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MinDate > MaxDate)
            {
                results.Add(new ValidationResult($"MaxDate must be greater than MinDate", new string[] { "MinDate", "MaxDate" }));
            }

            return results;
        }
    }

    public class AveragePower
    {
        [Key, Required]
        public long Id { get; set; }

        [Required]
        public long P { get; set; }

        [Required]
        public PowerMeterClock Clock { get; set; }

        public AveragePower() { }

        public AveragePower(AveragePowerDTO averagePowerDTO)
        {
            P = averagePowerDTO.P;
            Clock = new PowerMeterClock(averagePowerDTO.Clock);
        }
    }

    public class AveragePowerDTO
    {
        [Required]
        public long P { get; set; }

        [Required]
        public PowerMeterClockDTO Clock { get; set; }
    }

    public class PowerMeterClock
    {
        [Key, Required]
        public long Id { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public WeekDay Weekday { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        public int Minute { get; set; }

        [Required]
        public int Second { get; set; }

        [Required]
        public int Millisecond { get; set; }

        [Required]
        public int TimeZone { get; set; }

        [Required]
        public Season Season { get; set; }

        public PowerMeterClock() { }

        public PowerMeterClock(PowerMeterClockDTO powerMeterClockDTO)
        {
            Year = powerMeterClockDTO.Year;
            Month = powerMeterClockDTO.Month;
            Day = powerMeterClockDTO.Day;
            Weekday = powerMeterClockDTO.Weekday;
            Hour = powerMeterClockDTO.Hour;
            Minute = powerMeterClockDTO.Minute;
            Second = powerMeterClockDTO.Second;
            Millisecond = powerMeterClockDTO.Millisecond;
            TimeZone = powerMeterClockDTO.TimeZone;
            Season = powerMeterClockDTO.Season;
        }
    }

    public class PowerMeterClockDTO
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public WeekDay Weekday { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        public int Minute { get; set; }

        [Required]
        public int Second { get; set; }

        [Required]
        public int Millisecond { get; set; }

        [Required]
        public int TimeZone { get; set; }

        [Required]
        public Season Season { get; set; }
    }

    public enum WeekDay
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7,
    }

    public enum Season
    {
        Winter = 0x00,
        Summer = 0x80
    }

    public enum PowerMeterMeasureType
    {
        Information,
        Total,
        Tariff,
        Instantaneous
    }

    public enum OrderByPowerMeterTotal
    {
        Id,
        Date,
        ThingId,
        A_In,
        A_Out,
        Ri_In,
        Rc_In,
        Ri_Out,
        Rc_Out,
        A_L1_In,
        A_L2_In,
        A_L3_In,
        A_L1_Out,
        A_L2_Out,
        A_L3_Out,
    }

    public enum OrderByPowerMeterTariff
    {
        Id,
        Date,
        ThingId
    }

    public enum OrderByPowerMeterInstantaneous
    {
        Id,
        Date,
        ThingId,
        V_L1,
        I_L1,
        V_L2,
        I_L2,
        V_L3,
        I_L3,
        P_L1_In,
        P_L1_Out,
        P_L2_In,
        P_L2_Out,
        P_L3_In,
        P_L3_Out,
        P_In,
        P_Out,
        PF_T,
        PF_L1,
        PF_L2,
        PF_L3,
        F,
    }
}
