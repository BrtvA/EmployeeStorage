namespace EmployeeStorage.Tests
{
    internal class MockTransactionException : MockTransaction
    {
        public override void Commit()
        {
            throw new InvalidOperationException();
        }
    }
}
