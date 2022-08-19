using Newtonsoft.Json;
using System;

namespace BlogPost.Repository.Entities;

public abstract class BaseEntity
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
