using System.Collections;
using System.Collections.Generic;


public class PropertyDesTableData : IDataConfig
{
    public List<PDesDataInfo> Data;
    public IList GetDataInfoList()
    {
        return Data;
    }
}

public class PDesDataInfo : BaseDataInfo
{
    public string PropertyName;
    public string Description;
    public List<string> Values;
    public List<string> ValueDes;
}
