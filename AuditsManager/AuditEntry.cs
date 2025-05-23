﻿using HelpDeskSystem.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace HelpDeskSystem.AuditsManager
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
        }

        public EntityEntry Entry { get; set; }

        public string UserId { get; set; }

        public string TableName { get; set; }

        public string IpAddress { get; set; }

        public string Module { get; set; }

        public Dictionary<string, object> KeyValues { get; set; } = new();
        public Dictionary<string, object> OldValues { get; set; } = new();
        public Dictionary<string, object> NewValues { get; set; } = new();

        public AuditType AuditType { get; set; }

        public List<string> ChangedColumns { get; set; }

        public AuditTrail ToAudit()
        {
            var audit = new AuditTrail();

            audit.UserId = UserId;
            audit.Action = AuditType.ToString();
            audit.AffectedTable = TableName;
            audit.TimeStamp = DateTime.Now;
            audit.PrimaryKeys = JsonConvert.SerializeObject(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns);
            audit.Module = Module;
            //audit.IpAddress = IpAddress;

            return audit;
        }
    }
}