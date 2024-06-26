﻿using System.Text.Json.Serialization;

namespace RollercoasterDataAnalytics.Models
{
    public class WaitingTime
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? Code { get; set; }
        public Status Status { get; set; }
        public required string Name { get; set; }
        [JsonPropertyName("waitingtime")]
        public int Duration { get; set; }
    }

    public enum Status
    {
        Opened,
        Closed,
        Maintenance,
        ClosedWeather
    }
}
