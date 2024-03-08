using TheaAdmin.Domain;

namespace Thea;

public class QueryPageRequest
{
    private int _pageIndex;
    private int _pageSize;
    public string QueryText { get; set; }
    public DataStatus? Status { get; set; }
    public int PageIndex
    {
        get => this._pageIndex;
        set
        {
            if (value < 0)
                this._pageIndex = 0;
            if (value > 0)
                this._pageIndex = value - 1;
        }
    }
    public int PageSize
    {
        get => this._pageSize;
        set
        {
            if (value < 0)
                this._pageSize = 20;
            if (value > 100)
                this._pageSize = 100;
            this._pageSize = value;
        }
    }
}
