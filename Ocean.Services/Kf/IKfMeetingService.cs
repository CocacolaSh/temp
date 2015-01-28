﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Entity;
using Ocean.Core;
using Ocean.Entity.DTO;

namespace Ocean.Services
{
    public interface IKfMeetingService : IService<KfMeeting>
    {
        PagedList<KfMeeting> GetPageList(int pageIndex, int pageSize, KfMeetingDTO kfMeetingDTO);
    }
}