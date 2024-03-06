using System.ComponentModel.DataAnnotations;

namespace Models.OperationQueue
{
    public record ApiOperation
    {
        [Key]
        public int Id { get; set; }

        public required string Content { get; set; }

      //  public required string Url { get; set; }

       // public Responses.RequestsTypes RequestType { get; set; }

        public required ObjectType ObjectType { get; set; }

        public required ExecutionType ExecutionType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public OperationStatus Status { get; set; }

        public required string ObjectId { get; set; }
    }

    public enum OperationStatus { Pending, Success, Failure }

    public enum ObjectType
    {
        Book,
        //BookHistoric 
    }

    public enum ExecutionType { Insert, Update, Delete }
}
