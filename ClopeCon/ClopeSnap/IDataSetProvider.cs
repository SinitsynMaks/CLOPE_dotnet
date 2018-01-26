namespace ClopeCon.ClopeSnap
{
    public interface IDataSetProvider
    {
        bool GetTransaction(out Transaction transaction);
    }
}