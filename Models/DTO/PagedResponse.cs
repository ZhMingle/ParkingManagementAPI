using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagementAPI.Models.DTO
{
  public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalCount { get; set; }

    public PagedResponse(IEnumerable<T> data, int totalCount)
    {
        Data = data;
        TotalCount = totalCount;
    }
}
}