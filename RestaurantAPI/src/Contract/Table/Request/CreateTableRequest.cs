namespace RestaurantAPI.src.Contract.Table.Request {
    public class CreateTableRequest {
        public string TableCode { get; set; } = string.Empty;

        public int  SeatCount { get; set; } 
    }
}
