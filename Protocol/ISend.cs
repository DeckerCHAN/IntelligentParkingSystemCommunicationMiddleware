﻿using System;
using System.Collections.Generic;

namespace IPSCM.Protocol
{
    public interface ISend
    {
        String Send(String url, Dictionary<String, String> textData, Dictionary<String, Byte[]> rowData);
    }
}
