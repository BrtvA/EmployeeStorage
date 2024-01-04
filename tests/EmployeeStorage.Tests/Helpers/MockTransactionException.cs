namespace EmployeeStorage.Tests.Helpers
{
    internal class MockTransactionException : MockTransaction
    {
        public override void Commit()
        {
            throw new InvalidOperationException();
        }
    }
}
