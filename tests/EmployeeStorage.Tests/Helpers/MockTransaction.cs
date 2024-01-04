using System.Data;

namespace EmployeeStorage.Tests.Helpers;

internal class MockTransaction : IDbTransaction
{
    public IDbConnection? Connection => throw new NotImplementedException();

    public IsolationLevel IsolationLevel => throw new NotImplementedException();

    public virtual void Commit()
    {
        //throw new NotImplementedException();
    }

    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    public void Rollback()
    {
        //throw new NotImplementedException();
    }
}
