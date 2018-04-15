namespace Stocks.Data.Csv.Test.Mocks
{
    public class MockPoco
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is MockPoco casted)) return false;
            return casted.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
