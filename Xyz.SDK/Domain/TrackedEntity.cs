namespace Xyz.SDK.Domain
{
    public abstract class TrackedEntity<TPk> : EntityBase<TPk>
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
